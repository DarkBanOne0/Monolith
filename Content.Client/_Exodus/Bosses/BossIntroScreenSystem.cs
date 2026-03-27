using Content.Shared._Exodus.Bosses;
using Content.Client._Exodus.Bosses.Components;
using Content.Client._Exodus.Bosses;
using Robust.Client.Graphics;

namespace Content.Client._Exodus.Bosses;

public sealed partial class BossIntroScreenSystem : EntitySystem
{
    [Dependency] private readonly IOverlayManager _overlay = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<BossIntroActiveComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<BossIntroActiveComponent, ComponentShutdown>(OnShutdown);
    }

    private void OnStartup(EntityUid uid, BossIntroActiveComponent comp, ComponentStartup args)
    {
        comp.IntroScreen.Active = true;
        _overlay.AddOverlay(new BossIntroOverlay());
    }

    private void OnShutdown(EntityUid uid, BossIntroActiveComponent comp, ComponentShutdown args)
    {
        _overlay.RemoveOverlay(new BossIntroOverlay());
    }
}
