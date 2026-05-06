using Robust.Shared.Serialization;

namespace Content.Shared._Exodus.Research.Visuals;


[Serializable, NetSerializable]
public enum DataFarmVisuals : byte
{
    State
}

[Serializable, NetSerializable]
public enum DataFarmVisualLayers : byte
{
    State,
    Light
}

[Serializable, NetSerializable]
public enum DataFarmState : byte
{
    Off,
    Proces,
    Normal,
    NotGood,
    Destract
}
