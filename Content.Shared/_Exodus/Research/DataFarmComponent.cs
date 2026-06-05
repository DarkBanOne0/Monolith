// (c) Space Exodus Team - EXDS-RL
// Authors: DarkBanOne

using Content.Shared.Atmos;
using Content.Shared._Exodus.Research.Visuals;
using Robust.Shared.Audio;

namespace Content.Shared._Exodus.Research.Components;

[RegisterComponent, AutoGenerateComponentState]
public sealed partial class DataFarmComponent : Component
{
    [DataField]
    public GasMixture Buffer = new();

    [DataField, AutoNetworkedField]
    public DataFarmState CurrentState = DataFarmState.Off;

    [DataField]
    public TimeSpan DestroyTimer = TimeSpan.FromSeconds(120f);

    [DataField]
    public TimeSpan CycleAccumulator = TimeSpan.Zero;

    [DataField]
    public TimeSpan CycleDuration = TimeSpan.FromSeconds(1f);

    [DataField]
    public float DeltaT = 35f;

    [DataField]
    public float MinTemp = 268.15f;

    [DataField]
    public float MaxTemp = 323.15f;

    [DataField]
    public float MinMolesOnTile = 5f;

    [DataField]
    public float MinPressure = 20f;

    [DataField]
    public float IntakePerSecond = 10f;

    [DataField]
    public string InletName = "inlet";

    [DataField]
    public bool Enabled = true;

    [DataField]
    public float DamagePerSecond;

    [DataField]
    public bool StartupInProgress;

    [DataField]
    public TimeSpan StartupAccumulator = TimeSpan.Zero;

    [DataField]
    public TimeSpan StartupDuration = TimeSpan.FromSeconds(3f);

    [DataField, AutoNetworkedField]
    public SoundSpecifier? NormalSound = new SoundPathSpecifier("/Audio/_Exodus/Machines/DataMinerResearch/normal.ogg");

    [DataField, AutoNetworkedField]
    public SoundSpecifier? OnnSound = new SoundPathSpecifier("/Audio/_Exodus/Machines/DataMinerResearch/onn.ogg");

    [DataField, AutoNetworkedField]
    public SoundSpecifier? NoGoodSound = new SoundPathSpecifier("/Audio/_Exodus/Machines/DataMinerResearch/nogood.ogg");

    [DataField, AutoNetworkedField]
    public SoundSpecifier? ErrorSound = new SoundPathSpecifier("/Audio/_Exodus/Machines/DataMinerResearch/error.ogg");
}
