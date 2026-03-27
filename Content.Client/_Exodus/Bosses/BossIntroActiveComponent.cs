using Content.Shared._Exodus.Bosses;

namespace Content.Client._Exodus.Bosses.Components;

[RegisterComponent]
public sealed partial class BossIntroActiveComponent : Component
{
    [DataField]
    public BaseIntroScreen IntroScreen;
}
