using Content.Client._Exodus.Bosses.Components;
using Content.Shared._Exodus.Bosses;
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

        _font = new VectorFont(_cache.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Regular.ttf"), 24);
    }

    protected override void Draw(in OverlayDrawArgs args)
    {
        var handle = args.ScreenHandle;

        var vp = args.ViewportBounds;

        if (_player.LocalEntity is not { } localEntity ||
            !_entManager.TryGetComponent<BossIntroActiveComponent>(localEntity, out var introComp))
            return;

        var introScreen = introComp.IntroScreen;

        introScreen.BaseDrawPosition = new Vector2(vp.Width * 0.5f, vp.Height * 0.35f);

        int letterCounter = 0;
        Vector2 offset = -Vector2.UnitX * CalculateOffsetOfString(introScreen.Text, introScreen, handle) * 0.5f;
        for (int i = 0; i < introScreen.Text.Length; i++)
        {
            string character = introScreen.Text[i].ToString();

            Vector2 drawPosition = introScreen.BaseDrawPosition + offset;

            handle.DrawString(_font, drawPosition, character, introScreen.TextScale, introScreen.TextColor);

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
