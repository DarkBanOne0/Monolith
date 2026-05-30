using Content.Shared._Exodus.Research.Components;
using Content.Shared._Exodus.Research.Visuals;
using Robust.Client.GameObjects;

namespace Content.Client._Exodus.Research.Systems;

public sealed class DataFarmSystem : EntitySystem
{
    [Dependency] private readonly SpriteSystem _spriteSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DataFarmComponent, AppearanceChangeEvent>(OnAppearanceChanged);
    }

    public void OnAppearanceChanged(EntityUid uid, DataFarmComponent comp, AppearanceChangeEvent args)
    {
        if (!TryComp<SpriteComponent>(uid, out var spriteComp))
            return;

        if (!args.AppearanceData.TryGetValue(DataFarmVisuals.State, out var stateObj) ||
            stateObj is not DataFarmState state)
            return;

        if (!_spriteSystem.TryGetLayer((uid, spriteComp), comp.MachineLayer, out var machineLayer, logMissing: true)
            || !_spriteSystem.TryGetLayer((uid, spriteComp), comp.LightLayer, out var lightLayer, logMissing: true))
            return;

        var loop = state != DataFarmState.Proces;

        machineLayer.Loop = loop;
        lightLayer.Loop = loop;
    }
}
