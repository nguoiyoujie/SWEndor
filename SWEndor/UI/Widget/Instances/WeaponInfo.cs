using MTV3D65;
using SWEndor.Actors;

namespace SWEndor.UI.Widgets
{
  public class WidgetWeaponInfo : Widget
  {
    // Left Info
    private float leftinfo_left = 20;
    private float leftinfo_weapontop = 50;
    private float leftinfo_weaponwidth = 95;
    private float leftinfo_weaponheight = 40;

    public WidgetWeaponInfo(Screen2D owner) : base(owner, "weapon") { }

    public override bool Visible
    {
      get
      {
        ActorInfo p = PlayerInfo.Actor;
        return (!Owner.ShowPage
          && p != null
          && !p.IsDyingOrDead
          && Owner.ShowUI);
      }
    }

    public override void Draw()
    {
      ActorInfo p = PlayerInfo.Actor;
      if (p == null || !p.Active)
        return;

      int icolor = pcolor.Value;

      Engine.TrueVision.TVScreen2DImmediate.Action_Begin2D();
      Engine.TrueVision.TVScreen2DImmediate.Draw_FilledBox(leftinfo_left - 5
                                    , leftinfo_weapontop - 5
                                    , leftinfo_left + leftinfo_weaponwidth * 2 + 5
                                    , leftinfo_weapontop + leftinfo_weaponheight + 5
                                    , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());

      Engine.TrueVision.TVScreen2DImmediate.Draw_Box(leftinfo_left - 5
                            , leftinfo_weapontop - 5
                            , leftinfo_left + leftinfo_weaponwidth
                            , leftinfo_weapontop + leftinfo_weaponheight + 5
                            , icolor);

      Engine.TrueVision.TVScreen2DImmediate.Draw_Box(leftinfo_left + leftinfo_weaponwidth
                            , leftinfo_weapontop - 5
                            , leftinfo_left + leftinfo_weaponwidth * 2 + 5
                            , leftinfo_weapontop + leftinfo_weaponheight + 5
                            , icolor);
      Engine.TrueVision.TVScreen2DImmediate.Action_End2D();


      Engine.TrueVision.TVScreen2DText.Action_BeginText();

      Engine.TrueVision.TVScreen2DText.TextureFont_DrawText(PlayerInfo.PrimaryWeapon.ToString()
      , leftinfo_left
      , leftinfo_weapontop + 20
      , icolor
      , FontFactory.Get(Font.T16).ID
      );

      Engine.TrueVision.TVScreen2DText.TextureFont_DrawText(PlayerInfo.SecondaryWeapon.ToString()
      , leftinfo_left + leftinfo_weaponwidth + 5
      , leftinfo_weapontop + 20
      , icolor
      , FontFactory.Get(Font.T16).ID
      );
      Engine.TrueVision.TVScreen2DText.Action_EndText();
    }
  }
}
