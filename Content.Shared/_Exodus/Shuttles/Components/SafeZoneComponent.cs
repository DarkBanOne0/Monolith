using Robust.Shared.GameStates;

namespace Content.Shared._Exodus.Shuttles.Components;

[RegisterComponent, NetworkedComponent]
public sealed partial class SafeZoneComponent : Component
{
    [ViewVariables(VVAccess.ReadWrite), DataField]
    public float Radius = 256;

    [ViewVariables(VVAccess.ReadWrite), DataField]
    public string Text = "SAFE ZONE";
}
