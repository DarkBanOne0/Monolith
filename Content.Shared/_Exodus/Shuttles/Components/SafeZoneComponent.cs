// (c) Space Exodus Team - EXDS-RL with CLA
// Authors: DarkBanOne

using Robust.Shared.GameStates;

namespace Content.Shared._Exodus.Shuttles.Components;

[RegisterComponent, NetworkedComponent]
public sealed partial class SafeZoneComponent : Component
{
    [DataField]
    public float Radius = 256;

    [DataField]
    public string Text = "SAFE ZONE";
}
