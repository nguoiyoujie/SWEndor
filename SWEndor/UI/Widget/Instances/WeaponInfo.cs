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

    public WidgetWeaponInfo() : base("weapon") { }

    public override bool Visible
    {
      get
      {
        return (!Globals.Engine.Screen2D.ShowPage
            && Globals.Engine.PlayerInfo.Actor != null
            && Globals.Engine.PlayerInfo.Actor.ActorState != ActorState.DEAD
            && Globals.Engine.PlayerInfo.Actor.ActorState != ActorState.DYING
            && !(Globals.Engine.PlayerInfo.Actor.TypeInfo is InvisibleCameraATI)
            && Globals.Engine.Screen2D.ShowUI);
      }
    }

    public override void Draw()
    {
      ActorInfo p = Globals.Engine.PlayerInfo.Actor;
      if (p == null || p.CreationState != CreationState.ACTIVE)
        return;

      TV_COLOR pcolor = (p.Faction == null) ? new TV_COLOR(1, 1, 1, 1) : p.Faction.Color;

      Globals.Engine.TVScreen2DImmediate.Action_Begin2D();
      Globals.Engine.TVScreen2DImmediate.Draw_FilledBox(leftinfo_left - 5
                                    , leftinfo_weapontop - 5
                                    , leftinfo_left + leftinfo_weaponwidth * 2 + 5
                                    , leftinfo_weapontop + leftinfo_weaponheight + 5
                                    , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());

      Globals.Engine.TVScreen2DImmediate.Draw_Box(leftinfo_left - 5
                            , leftinfo_weapontop - 5
                            , leftinfo_left + leftinfo_weaponwidth
                            , leftinfo_weapontop + leftinfo_weaponheight + 5
                            , pcolor.GetIntColor());

      Globals.Engine.TVScreen2DImmediate.Draw_Box(leftinfo_left + leftinfo_weaponwidth
                            , leftinfo_weapontop - 5
                            , leftinfo_left + leftinfo_weaponwidth * 2 + 5
                            , leftinfo_weapontop + leftinfo_weaponheight + 5
                            , pcolor.GetIntColor());
      Globals.Engine.TVScreen2DImmediate.Action_End2D();


      Globals.Engine.TVScreen2DText.Action_BeginText();

      Globals.Engine.TVScreen2DText.TextureFont_DrawText(Globals.Engine.PlayerInfo.PrimaryWeapon.ToUpper() //.Replace("A", "").Replace("E", "").Replace("I", "").Replace("O", "").Replace("U", "")
      , leftinfo_left
      , leftinfo_weapontop + 20
      , pcolor.GetIntColor()
      , Font.Factory.Get("Text_16").ID
      );

      Globals.Engine.TVScreen2DText.TextureFont_DrawText(Globals.Engine.PlayerInfo.SecondaryWeapon.ToUpper() //.Replace("A", "").Replace("E", "").Replace("I", "").Replace("O", "").Replace("U", "")
      , leftinfo_left + leftinfo_weaponwidth + 5
      , leftinfo_weapontop + 20
      , pcolor.GetIntColor()
      , Font.Factory.Get("Text_16").ID
      );
      Globals.Engine.TVScreen2DText.Action_EndText();
    }
  }
}
