using Content.Shared.Atmos;
using Content.Shared._Exodus.Research.Visuals;

namespace Content.Server._Exodus.Research.Components;

[RegisterComponent]
public sealed partial class DataFarmComponent : Component
{
    [DataField]
    public GasMixture Buffer = new();

    [DataField]
    public DataFarmState CurrentState = DataFarmState.Off;

    [DataField]
    public TimeSpan DestroyTimer = TimeSpan.FromSeconds(120f);

    [DataField]
    public TimeSpan CycleAccumulator = TimeSpan.FromSeconds(0f);

    [DataField]
    public TimeSpan CycleDuration = TimeSpan.FromSeconds(1f);

    [DataField]
    public float DeltaT = 30f;

    [DataField]
    public float MinTemp = 268.15f;

    [DataField]
    public float MaxTemp = 323.15f;

    [DataField]
    public float MinMolesOnTile = 5f;

    [DataField]
    public float MinPressure = 20f;

    [DataField]
    public float IntakePerSecond= 50f;

    [DataField]
    public string InletName = "inlet";

    [DataField]
    public bool Enabled = true;

    [DataField]
    public float DamagePerSecond;
}
