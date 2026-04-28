using Content.Server._Exodus.Research.Components;
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
using Content.Shared.Atmos;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Robust.Shared.Prototypes;
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


    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DataFarmComponent, AtmosDeviceUpdateEvent>(OnAtmosUpdate);
        SubscribeLocalEvent<DataFarmComponent, ComponentStartup>(OnStartup);
    }

    private void OnStartup(EntityUid uid, DataFarmComponent comp, ComponentStartup args)
    {
        if (!TryComp<DestructibleComponent>(uid, out var destrComp))
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

        if (destructionThreshold is null || comp.DestroyTimer.TotalSeconds <= 0)
            return;

        comp.DamagePerSecond = (float)(destructionThreshold.Value / comp.DestroyTimer.TotalSeconds);
    }

    private void OnAtmosUpdate(EntityUid uid, DataFarmComponent comp, ref AtmosDeviceUpdateEvent args)
    {
        if (comp.IntakePerSecond <= 0f ||
            !_nodeContainer.TryGetNode(uid, comp.InletName, out PipeNode? inlet))
        {
            SetEnabled((uid, comp), false);
            SetState((uid, comp), DataFarmState.Off);

            return;
        }

        var env = _atmos.GetContainingMixture(uid, ignoreExposed: true, excite: true);
        var powered = _power.IsPowered(uid);
        var takeNow = comp.IntakePerSecond * args.dt;

        if (!powered
            || env == null
            || env.TotalMoles < comp.MinMolesOnTile
            || env.Pressure < comp.MinPressure
            ||inlet.Air.TotalMoles < takeNow)
        {
            SetEnabled((uid, comp), false);
            SetState((uid, comp), DataFarmState.Off);
            return;
        }

        if (env.Temperature < comp.MinTemp)
        {
            SetEnabled((uid, comp), false);
            SetState(uid, comp, DataFarmState.Hypothermia);
            return;
        }

        SetEnabled((uid, comp), true);

        var removed = inlet.Air.Remove(takeNow);
        _atmos.Merge(comp.Buffer, removed);

        comp.CycleAccumulator += TimeSpan.FromSeconds(args.dt);

        if (comp.CycleAccumulator < comp.CycleDuration || comp.Buffer.TotalMoles <= 0f || !comp.Enabled)
            return;

        if (env.Temperature > comp.MaxTemp)
        {
            SetState(uid, comp, DataFarmState.Overheat);
            ApplyHeatDamage((uid, comp));
        }
        else
        {
            SetState(uid, comp, DataFarmState.On);
        }

        var c = _atmos.GetHeatCapacity(comp.Buffer, applyScaling: true);
        var dQ = c * comp.DeltaT;
        _atmos.AddHeat(comp.Buffer, dQ);

        _atmos.Merge(env, comp.Buffer);
        comp.Buffer = new GasMixture();
        comp.CycleAccumulator = TimeSpan.Zero;
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

    private void SetState(Entity<DataFarmComponent> ent, DataFarmState state)
    {
        if (ent.Comp.CurrentState == state)
            return;

        ent.Comp.CurrentState = state;

        if (TryComp<AppearanceComponent>(uid, out var appearance))
            _appearance.SetData(uid, DataFarmVisuals.State, state, appearance);
    }
}
