using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.Player;

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
            && Owner.Engine.PlayerInfo.Actor != null
            && Owner.Engine.PlayerInfo.Actor.ActorState != ActorState.DEAD
            && Owner.Engine.PlayerInfo.Actor.ActorState != ActorState.DYING
            && !(Owner.Engine.PlayerInfo.Actor.TypeInfo is InvisibleCameraATI)
            && Owner.ShowUI);
      }
    }

    public override void Draw()
    {
      ActorInfo p = Owner.Engine.PlayerInfo.Actor;
      if (p == null || p.CreationState != CreationState.ACTIVE)
        return;

      TV_COLOR pcolor = (p.Faction == null) ? new TV_COLOR(1, 1, 1, 1) : p.Faction.Color;

      Owner.Engine.TrueVision.TVScreen2DImmediate.Action_Begin2D();
      Owner.Engine.TrueVision.TVScreen2DImmediate.Draw_FilledBox(leftinfo_left - 5
                                    , leftinfo_weapontop - 5
                                    , leftinfo_left + leftinfo_weaponwidth * 2 + 5
                                    , leftinfo_weapontop + leftinfo_weaponheight + 5
                                    , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());

      Owner.Engine.TrueVision.TVScreen2DImmediate.Draw_Box(leftinfo_left - 5
                            , leftinfo_weapontop - 5
                            , leftinfo_left + leftinfo_weaponwidth
                            , leftinfo_weapontop + leftinfo_weaponheight + 5
                            , pcolor.GetIntColor());

      Owner.Engine.TrueVision.TVScreen2DImmediate.Draw_Box(leftinfo_left + leftinfo_weaponwidth
                            , leftinfo_weapontop - 5
                            , leftinfo_left + leftinfo_weaponwidth * 2 + 5
                            , leftinfo_weapontop + leftinfo_weaponheight + 5
                            , pcolor.GetIntColor());
      Owner.Engine.TrueVision.TVScreen2DImmediate.Action_End2D();


      Owner.Engine.TrueVision.TVScreen2DText.Action_BeginText();

      Owner.Engine.TrueVision.TVScreen2DText.TextureFont_DrawText(Owner.Engine.PlayerInfo.PrimaryWeapon.ToUpper() //.Replace("A", "").Replace("E", "").Replace("I", "").Replace("O", "").Replace("U", "")
      , leftinfo_left
      , leftinfo_weapontop + 20
      , pcolor.GetIntColor()
      , Font.Factory.Get("Text_16").ID
      );

      Owner.Engine.TrueVision.TVScreen2DText.TextureFont_DrawText(Owner.Engine.PlayerInfo.SecondaryWeapon.ToUpper() //.Replace("A", "").Replace("E", "").Replace("I", "").Replace("O", "").Replace("U", "")
      , leftinfo_left + leftinfo_weaponwidth + 5
      , leftinfo_weapontop + 20
      , pcolor.GetIntColor()
      , Font.Factory.Get("Text_16").ID
      );
      Owner.Engine.TrueVision.TVScreen2DText.Action_EndText();
    }
  }
}
