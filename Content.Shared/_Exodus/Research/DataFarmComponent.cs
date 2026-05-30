using Content.Shared.Atmos;
using Content.Shared._Exodus.Research.Visuals;
using Robust.Shared.Audio;

namespace Content.Shared._Exodus.Research.Components;

[RegisterComponent, AutoGenerateComponentState]
public sealed partial class DataFarmComponent : Component
{
    [DataField, AutoNetworkedField]
    public GasMixture Buffer = new();

    [DataField, AutoNetworkedField]
    public DataFarmState CurrentState = DataFarmState.Off;

    [DataField, AutoNetworkedField]
    public TimeSpan DestroyTimer = TimeSpan.FromSeconds(120f);

    [DataField, AutoNetworkedField]
    public TimeSpan CycleAccumulator = TimeSpan.Zero;

    [DataField, AutoNetworkedField]
    public TimeSpan CycleDuration = TimeSpan.FromSeconds(1f);

    [DataField, AutoNetworkedField]
    public float DeltaT = 30f;

    [DataField, AutoNetworkedField]
    public float MinTemp = 268.15f;

    [DataField, AutoNetworkedField]
    public float MaxTemp = 323.15f;

    [DataField, AutoNetworkedField]
    public float MinMolesOnTile = 5f;

    [DataField, AutoNetworkedField]
    public float MinPressure = 20f;

    [DataField, AutoNetworkedField]
    public float IntakePerSecond= 50f;

    [DataField, AutoNetworkedField]
    public string InletName = "inlet";

    [DataField, AutoNetworkedField]
    public bool Enabled = true;

    [DataField, AutoNetworkedField]
    public float DamagePerSecond;

    [DataField, AutoNetworkedField]
    public bool StartupInProgress;

    [DataField, AutoNetworkedField]
    public TimeSpan StartupAccumulator = TimeSpan.Zero;

    [DataField, AutoNetworkedField]
    public TimeSpan OnnSoundDuration = TimeSpan.FromSeconds(3f);

    [DataField, AutoNetworkedField]
    public SoundSpecifier? NormalSound = new SoundPathSpecifier("/Audio/_Exodus/Machines/DataMinerResearch/normal.ogg");

    [DataField, AutoNetworkedField]
    public SoundSpecifier? OnnSound = new SoundPathSpecifier("/Audio/_Exodus/Machines/DataMinerResearch/onn.ogg");

    [DataField, AutoNetworkedField]
    public SoundSpecifier? NoGoodSound = new SoundPathSpecifier("/Audio/_Exodus/Machines/DataMinerResearch/nogood.ogg");

    [DataField, AutoNetworkedField]
    public SoundSpecifier? ErrorSound = new SoundPathSpecifier("/Audio/_Exodus/Machines/DataMinerResearch/error.ogg");


    [DataField]
    public string MachineLayer = "enum.DataFarmVisualLayers.State";

    [DataField]
    public string LightLayer = "enum.DataFarmVisualLayers.Light";
}
