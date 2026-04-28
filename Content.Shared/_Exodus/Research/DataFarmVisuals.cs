using Robust.Shared.Serialization;

namespace Content.Shared._Exodus.Research.Visuals;


[Serializable, NetSerializable]
public enum DataFarmVisuals : byte
{
    State
}
[Serializable, NetSerializable]
public enum DataFarmState : byte
{
    Off,
    Hypothermia,
    On,
    Overheat
}
