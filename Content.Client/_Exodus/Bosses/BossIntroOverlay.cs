using Content.Client._Exodus.Bosses.Components;
using Robust.Client.Player;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Timing;
using Robust.Shared.Enums;
using System.Numerics;
using System.Text;

namespace Content.Client._Exodus.Bosses;

public sealed class BossIntroOverlay : Overlay
{
    [Dependency] private readonly IEntityManager _entManager = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IPlayerManager _player = default!;
    [Dependency] private readonly IResourceCache _cache = default!;

    public override OverlaySpace Space => OverlaySpace.ScreenSpace;

    private Font _font;

    public BossIntroOverlay()
    {
        IoCManager.InjectDependencies(this);

        _font = new VectorFont(_cache.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Regular.ttf"), 120);
    }

    protected override void Draw(in OverlayDrawArgs args)
    {
        var handle = args.ScreenHandle;

        var vp = args.ViewportBounds;

        if (_player.LocalEntity is not { } localEntity ||
            !_entManager.TryGetComponent<BossIntroActiveComponent>(localEntity, out var introComp))
            return;

        var introScreen = introComp.IntroScreen;

        introScreen.BaseDrawPosition = new Vector2(vp.Width * 0.5f, vp.Height * 0.15f);

        float opacity = (introScreen.AnimationCompletion - introScreen.TextDelay) / (introScreen.TextDelay + 0.05f - introScreen.TextDelay)
            * (introScreen.AnimationCompletion - 1f) / (0.77f - 1f);
        opacity = MathHelper.Clamp(opacity, 0f, 1f);

        int letterCounter = 0;
        Vector2 offset = -Vector2.UnitX * CalculateOffsetOfString(introScreen.Text, introScreen, handle) * 0.5f;

        for (int i = 0; i < introScreen.Text.Length; i++)
        {
            string character = introScreen.Text[i].ToString();

            float letterCompletionRatio = i / (float)(introScreen.Text.Length -1f);

            Vector2 drawPosition = introScreen.BaseDrawPosition + offset;

            Color textColor = introScreen.TextColor.Calculate(letterCompletionRatio);
            textColor.R *= opacity;
            textColor.G *= opacity;
            textColor.B *= opacity;
            textColor.A *= opacity;

            handle.DrawString(_font, drawPosition, character, introScreen.TextScale, textColor);

            offset += CalculateOffsetOfString(character, introScreen, handle);

            letterCounter++;
        }
    }

    protected override void FrameUpdate(FrameEventArgs args)
    {
        if (_player.LocalEntity is not { } localEntity ||
            !_entManager.TryGetComponent<BossIntroActiveComponent>(localEntity, out var introComp))
            return;

        if (introComp.IntroScreen.AnimationTimer < introComp.IntroScreen.AnimationDuration)
            introComp.IntroScreen.AnimationTimer += args.DeltaSeconds;
        else
        {
            _entManager.RemoveComponent<BossIntroActiveComponent>(localEntity);
        }
    }

    private Vector2 CalculateOffsetOfString(string str, BaseIntroScreen introScreen, DrawingHandleScreen handle)
    {
        return Vector2.UnitX * handle.GetDimensions(_font, str, introScreen.TextScale).X;
    }
}
