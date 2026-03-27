using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using System.Numerics;

namespace Content.Client._Exodus.Bosses;

public class TestIntroScreen : BaseIntroScreen
{
    public TextColorData TextColor => new(ratio =>
    {
        float colorInterpolant = (float)Math.Sin(AnimationCompletion * 8f + ratio * Math.PI * 3f) * 0.5f + 0.5f;
        return Color.InterpolateBetween(Color.Cyan, Color.Fuchsia, colorInterpolant);
    });
}
