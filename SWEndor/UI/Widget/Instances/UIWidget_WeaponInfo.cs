using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.Input;
using SWEndor.Player;

namespace SWEndor.UI
{
  public class UIWidget_WeaponInfo : UIWidget
  {
    // Left Info
    private float leftinfo_left = 20;
    private float leftinfo_weapontop = 50;
    private float leftinfo_weaponwidth = 95;
    private float leftinfo_weaponheight = 40;

    public UIWidget_WeaponInfo() : base("weapon") { }

    public override bool Visible
    {
      get
      {
        return (!Screen2D.Instance().ShowPage
            && PlayerInfo.Instance().Actor != null
            && PlayerInfo.Instance().Actor.ActorState != ActorState.DEAD
            && PlayerInfo.Instance().Actor.ActorState != ActorState.DYING
            && !(PlayerInfo.Instance().Actor.TypeInfo is InvisibleCameraATI)
            && Screen2D.Instance().ShowUI);
      }
    }

    public override void Draw()
    {
      ActorInfo p = PlayerInfo.Instance().Actor;
      if (p == null || p.CreationState != CreationState.ACTIVE)
        return;

      TV_COLOR pcolor = (p.Faction == null) ? new TV_COLOR(1, 1, 1, 1) : p.Faction.Color;

      Engine.Instance().TVScreen2DImmediate.Action_Begin2D();
      Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(leftinfo_left - 5
                                    , leftinfo_weapontop - 5
                                    , leftinfo_left + leftinfo_weaponwidth * 2 + 5
                                    , leftinfo_weapontop + leftinfo_weaponheight + 5
                                    , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());

      Engine.Instance().TVScreen2DImmediate.Draw_Box(leftinfo_left - 5
                            , leftinfo_weapontop - 5
                            , leftinfo_left + leftinfo_weaponwidth
                            , leftinfo_weapontop + leftinfo_weaponheight + 5
                            , pcolor.GetIntColor());

      Engine.Instance().TVScreen2DImmediate.Draw_Box(leftinfo_left + leftinfo_weaponwidth
                            , leftinfo_weapontop - 5
                            , leftinfo_left + leftinfo_weaponwidth * 2 + 5
                            , leftinfo_weapontop + leftinfo_weaponheight + 5
                            , pcolor.GetIntColor());
      Engine.Instance().TVScreen2DImmediate.Action_End2D();


      Engine.Instance().TVScreen2DText.Action_BeginText();

      /*
      Engine.Instance().TVScreen2DText.TextureFont_DrawText(string.Format("SWITCH: {0} {1}"
        , ((CONST_TV_KEY)InputKeyMap.GetFnKey("g_weap1mode+")).ToString().Replace("TV_KEY_", "").Replace("-1", "").PadLeft(1)
        , ((CONST_TV_KEY)InputKeyMap.GetFnKey("g_weap1mode-")).ToString().Replace("TV_KEY_", "").Replace("-1", "").PadLeft(1)
        )
      , leftinfo_left
      , leftinfo_weapontop
      , pcolor.GetIntColor()
      , Font.GetFont("Text_08").ID
      );
      */

      Engine.Instance().TVScreen2DText.TextureFont_DrawText(PlayerInfo.Instance().PrimaryWeapon.ToUpper() //.Replace("A", "").Replace("E", "").Replace("I", "").Replace("O", "").Replace("U", "")
      , leftinfo_left
      , leftinfo_weapontop + 20
      , pcolor.GetIntColor()
      , Font.GetFont("Text_16").ID
      );

      /*
      Engine.Instance().TVScreen2DText.TextureFont_DrawText(string.Format("SWITCH: {0} {1}"
        , ((CONST_TV_KEY)InputKeyMap.GetFnKey("g_weap2mode+")).ToString().Replace("TV_KEY_", "").Replace("-1", "").PadLeft(1)
        , ((CONST_TV_KEY)InputKeyMap.GetFnKey("g_weap2mode-")).ToString().Replace("TV_KEY_", "").Replace("-1", "").PadLeft(1)
        )
      , leftinfo_left + leftinfo_weaponwidth + 5
      , leftinfo_weapontop
      , pcolor.GetIntColor()
      , Font.GetFont("Text_08").ID
      );
      */

      Engine.Instance().TVScreen2DText.TextureFont_DrawText(PlayerInfo.Instance().SecondaryWeapon.ToUpper() //.Replace("A", "").Replace("E", "").Replace("I", "").Replace("O", "").Replace("U", "")
      , leftinfo_left + leftinfo_weaponwidth + 5
      , leftinfo_weapontop + 20
      , pcolor.GetIntColor()
      , Font.GetFont("Text_16").ID
      );
      Engine.Instance().TVScreen2DText.Action_EndText();
    }
  }
}
