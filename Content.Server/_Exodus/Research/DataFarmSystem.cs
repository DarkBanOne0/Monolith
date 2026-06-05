// (c) Space Exodus Team - EXDS-RL
// Authors: DarkBanOne

using Content.Server.Atmos.EntitySystems;
using Content.Server.Atmos.Piping.Components;
using Content.Server.Destructible;
using Content.Server.Destructible.Thresholds;
using Content.Server.Destructible.Thresholds.Behaviors;
using Content.Server.Destructible.Thresholds.Triggers;
using Content.Server.NodeContainer.EntitySystems;
using Content.Server.NodeContainer.Nodes;
using Content.Server.Power.EntitySystems;
using Content.Server.Research.Components;
using Content.Shared._Exodus.Research.Visuals;
using Content.Shared._Exodus.Research.Components;
using Content.Shared.Audio;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Audio;
using System.Linq;

namespace Content.Server._Exodus.Research.Systems;

public sealed class DataFarmSystem : EntitySystem
{
    [Dependency] private readonly NodeContainerSystem _nodeContainer = default!;
    [Dependency] private readonly AtmosphereSystem _atmos = default!;
    [Dependency] private readonly PowerReceiverSystem _power = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly SharedAmbientSoundSystem _ambient = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DataFarmComponent, AtmosDeviceUpdateEvent>(OnAtmosUpdate);
        SubscribeLocalEvent<DataFarmComponent, ComponentStartup>(OnStartup);
    }

    private void OnStartup(Entity<DataFarmComponent> ent, ref ComponentStartup args)
    {
        if (!TryComp<DestructibleComponent>(ent.Owner, out var destrComp))
            return;

        int? destructionThreshold = null;

        foreach (var threshold in destrComp.Thresholds)
        {
            if (threshold.Trigger is not DamageTrigger dmgTrigger)
                continue;

            var hasDestructionAct = threshold.Behaviors
                .OfType<DoActsBehavior>()
                .Any(b => b.HasAct(ThresholdActs.Destruction));

            if (!hasDestructionAct)
                continue;

            destructionThreshold = destructionThreshold is null
                ? dmgTrigger.Damage
                : Math.Min(destructionThreshold.Value, dmgTrigger.Damage);
        }

        if (destructionThreshold is null || ent.Comp.DestroyTimer.TotalSeconds <= 0)
            return;

        ent.Comp.DamagePerSecond = (float)(destructionThreshold.Value / ent.Comp.DestroyTimer.TotalSeconds);
    }

    private void OnAtmosUpdate(Entity<DataFarmComponent> ent, ref AtmosDeviceUpdateEvent args)
    {
        if (ent.Comp.IntakePerSecond <= 0f ||
            !_nodeContainer.TryGetNode(ent.Owner, ent.Comp.InletName, out PipeNode? inlet))
        {
            SetEnabled((ent.Owner, ent.Comp), false);
            SetSound((ent.Owner, ent.Comp), DataFarmState.Off);
            SetState((ent.Owner, ent.Comp), DataFarmState.Off);

            return;
        }

        var env = _atmos.GetContainingMixture(ent.Owner, ignoreExposed: true, excite: true);
        var powered = _power.IsPowered(ent.Owner);
        var takeNow = ent.Comp.IntakePerSecond * args.dt;

        if (!powered)
        {
            ent.Comp.StartupAccumulator = TimeSpan.Zero;
            ent.Comp.StartupInProgress = false;

            SetEnabled((ent.Owner, ent.Comp), false);
            SetSound((ent.Owner, ent.Comp), DataFarmState.Off);
            SetState((ent.Owner, ent.Comp), DataFarmState.Off);

            return;
        }

        if (ent.Comp.StartupInProgress)
        {
            ent.Comp.StartupAccumulator += TimeSpan.FromSeconds(args.dt);

            if (ent.Comp.StartupAccumulator <= ent.Comp.StartupDuration)
            {
                SetEnabled((ent.Owner, ent.Comp), false);
                SetSound((ent.Owner, ent.Comp), DataFarmState.Proces);
                SetState((ent.Owner, ent.Comp), DataFarmState.Proces);

                return;
            }

            ent.Comp.StartupInProgress = false;
            ent.Comp.StartupAccumulator = TimeSpan.Zero;
        }

        if (ent.Comp.CurrentState == DataFarmState.Off)
        {
            ent.Comp.StartupInProgress = true;

            SetSound((ent.Owner, ent.Comp), DataFarmState.Proces);
            SetState((ent.Owner, ent.Comp), DataFarmState.Proces);

            return;
        }

        if (env == null
            || env.Temperature < ent.Comp.MinTemp
            || env.TotalMoles < ent.Comp.MinMolesOnTile
            || env.Pressure < ent.Comp.MinPressure
            || inlet.Air.TotalMoles < takeNow)
        {
            SetEnabled((ent.Owner, ent.Comp), false);
            SetSound((ent.Owner, ent.Comp), DataFarmState.NotGood);
            SetState((ent.Owner, ent.Comp), DataFarmState.NotGood);

            return;
        }

        SetEnabled((ent.Owner, ent.Comp), true);

        var removed = inlet.Air.Remove(takeNow);
        _atmos.Merge(ent.Comp.Buffer, removed);

        ent.Comp.CycleAccumulator += TimeSpan.FromSeconds(args.dt);

        if (ent.Comp.CycleAccumulator < ent.Comp.CycleDuration || ent.Comp.Buffer.TotalMoles <= 0f || !ent.Comp.Enabled)
            return;

        if (env.Temperature > ent.Comp.MaxTemp)
        {
            SetSound((ent.Owner, ent.Comp), DataFarmState.Destract);
            SetState((ent.Owner, ent.Comp), DataFarmState.Destract);
            ApplyDamage((ent.Owner, ent.Comp));
        }
        else
        {
            SetSound((ent.Owner, ent.Comp), DataFarmState.Normal);
            SetState((ent.Owner, ent.Comp), DataFarmState.Normal);
        }

        var c = _atmos.GetHeatCapacity(ent.Comp.Buffer, applyScaling: true);
        var dQ = c * ent.Comp.DeltaT;
        _atmos.AddHeat(ent.Comp.Buffer, dQ);

        _atmos.Merge(env, ent.Comp.Buffer);
        ent.Comp.Buffer.Clear();
        ent.Comp.CycleAccumulator = TimeSpan.Zero;
    }

    public void SetEnabled(Entity<DataFarmComponent> ent, bool value)
    {
        if (!TryComp<ResearchPointSourceComponent>(ent.Owner, out var sourceComp))
            return;

        if (ent.Comp.Enabled == value)
            return;

        ent.Comp.Enabled = value;

        sourceComp.Active = ent.Comp.Enabled;
    }

    public void ApplyDamage(Entity<DataFarmComponent> ent)
    {
        var heatType = _prototypeManager.Index<DamageTypePrototype>("Heat");
        var damage = new DamageSpecifier(heatType, ent.Comp.DamagePerSecond);

        _damageable.TryChangeDamage(ent.Owner, damage, ignoreResistances: true, interruptsDoAfters: false);
    }

    private void SetSound(Entity<DataFarmComponent> ent, DataFarmState state)
    {
        if (state == ent.Comp.CurrentState)
            return;

        if (!HasComp<AmbientSoundComponent>(ent.Owner))
            AddComp<AmbientSoundComponent>(ent.Owner);

        SoundSpecifier? sound = state switch
        {
            DataFarmState.Off => null,
            DataFarmState.Proces => ent.Comp.OnnSound,
            DataFarmState.Normal => ent.Comp.NormalSound,
            DataFarmState.NotGood => ent.Comp.NoGoodSound,
            DataFarmState.Destract => ent.Comp.ErrorSound,
            _ => null
        };

        _ambient.SetAmbience(ent.Owner, sound != null);

        if (sound != null)
            _ambient.SetSound(ent.Owner, sound);
    }

    public void SetState(Entity<DataFarmComponent> ent, DataFarmState state)
    {
        if (ent.Comp.CurrentState == state)
            return;

        ent.Comp.CurrentState = state;

        if (TryComp<AppearanceComponent>(ent.Owner, out var appearance))
            _appearance.SetData(ent.Owner, DataFarmVisuals.State, state, appearance);
    }
}
