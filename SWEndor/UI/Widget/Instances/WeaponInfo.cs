using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Traits;
using SWEndor.ActorTypes;

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
        return (!Owner.ShowPage
            && PlayerInfo.Actor != null
            && !PlayerInfo.Actor.StateModel.IsDyingOrDead
            && Owner.ShowUI);
      }
    }

    public override void Draw()
    {
      ActorInfo p = PlayerInfo.Actor;
      if (p == null || !p.Active)
        return;

      TV_COLOR pcolor = (p.Faction == null) ? new TV_COLOR(1, 1, 1, 1) : p.Faction.Color;

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
                            , pcolor.GetIntColor());

      Engine.TrueVision.TVScreen2DImmediate.Draw_Box(leftinfo_left + leftinfo_weaponwidth
                            , leftinfo_weapontop - 5
                            , leftinfo_left + leftinfo_weaponwidth * 2 + 5
                            , leftinfo_weapontop + leftinfo_weaponheight + 5
                            , pcolor.GetIntColor());
      Engine.TrueVision.TVScreen2DImmediate.Action_End2D();


      Engine.TrueVision.TVScreen2DText.Action_BeginText();

      Engine.TrueVision.TVScreen2DText.TextureFont_DrawText(PlayerInfo.PrimaryWeapon.ToUpper() //.Replace("A", "").Replace("E", "").Replace("I", "").Replace("O", "").Replace("U", "")
      , leftinfo_left
      , leftinfo_weapontop + 20
      , pcolor.GetIntColor()
      , FontFactory.Get(Font.T16).ID
      );

      Engine.TrueVision.TVScreen2DText.TextureFont_DrawText(PlayerInfo.SecondaryWeapon.ToUpper() //.Replace("A", "").Replace("E", "").Replace("I", "").Replace("O", "").Replace("U", "")
      , leftinfo_left + leftinfo_weaponwidth + 5
      , leftinfo_weapontop + 20
      , pcolor.GetIntColor()
      , FontFactory.Get(Font.T16).ID
      );
      Engine.TrueVision.TVScreen2DText.Action_EndText();
    }
  }
}
