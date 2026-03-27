using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using System.Numerics;

namespace Content.Client._Exodus.Bosses;

public abstract class BaseIntroScreen
{
    public string Text = "Джонни Бородинский";
    public float AnimationDuration = 6.0f;

    public float AnimationTimer = 0f;

    public bool Active;

    public float TextScale = 1f;

    public virtual TextColorData TextColor => Color.White;

    public virtual float AnimationCompletion => Math.Clamp(AnimationTimer / AnimationDuration, 0f, 1f);


    public Vector2 BaseDrawPosition;

    public float TextDelay = 0.1f;
}
