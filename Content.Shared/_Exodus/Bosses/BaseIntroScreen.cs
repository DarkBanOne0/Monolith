using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System.Numerics;

namespace Content.Shared._Exodus.Bosses;

[Serializable, NetSerializable]
public class BaseIntroScreen
{
    public string Text = "Тест";
    public float AnimationDuration = 6.0f;

    public float AnimationTimer = 0;

    public bool Active;

    public float AnimationCompletion => Math.Clamp(AnimationTimer / AnimationDuration, 0f, 1f);

    public float TextScale = 1f;

    public Color TextColor = Color.Red;

    public Vector2 BaseDrawPosition;
}
