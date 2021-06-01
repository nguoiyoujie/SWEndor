using MTV3D65;
using SWEndor.Game.Actors;
using SWEndor.Game.Actors.Models;
using Primrose.Primitives.Extensions;
using SWEndor.Game.Weapons;
using System.IO;

namespace SWEndor.Game.UI.Widgets
{
  public class CrossHairWidget : Widget
  {
    private readonly int m_textureID;

    public CrossHairWidget(Screen2D owner) : base(owner, "crosshair")
    {
      m_textureID = Engine.TrueVision.TVTextureFactory.LoadTexture(Path.Combine(Globals.ImagePath, @"crosshair\Crosshair.png")
                                                                   , "Crosshair"
                                                                   , 64
                                                                   , 64
                                                                   , CONST_TV_COLORKEY.TV_COLORKEY_USE_ALPHA_CHANNEL);
    }

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

      COLOR pcolor = PlayerInfo.FactionColor;
      int icolor = pcolor.Value;

      TVScreen2DImmediate.Action_Begin2D();
      TVScreen2DImmediate.Draw_Texture(m_textureID
                               , Owner.ScreenCenter.x - 32
                               , Owner.ScreenCenter.y - 32
                               , Owner.ScreenCenter.x + 32
                               , Owner.ScreenCenter.y + 32
                               , icolor
                               , icolor
                               , icolor
                               , icolor);

      Weapons.WeaponInfo weap = PlayerInfo.PrimaryWeapon.Weapon;
      int burst = PlayerInfo.PrimaryWeapon.Burst;
      if (weap != null)
      {
        COLOR disabled_color = ColorLocalization.Get(ColorLocalKeys.GAME_SYSTEM_DISABLED);
        COLOR destroyed_color = ColorLocalization.Get(ColorLocalKeys.GAME_SYSTEM_DESTROYED);
        if (!p.TypeInfo.SystemData.AllowSystemDamage || p.GetStatus(SystemPart.LASER_WEAPONS) == SystemState.ACTIVE)
          DrawLaser(weap, burst, pcolor, disabled_color);
        else if (p.GetStatus(SystemPart.LASER_WEAPONS) == SystemState.DESTROYED)
          DrawLaser(weap, 0, destroyed_color, destroyed_color);
        else if (p.GetStatus(SystemPart.LASER_WEAPONS) == SystemState.DISABLED)
          DrawLaser(weap, 0, disabled_color, disabled_color);
      }

      weap = PlayerInfo.SecondaryWeapon.Weapon;
      burst = PlayerInfo.SecondaryWeapon.Burst;
      if (weap != null)
      {
        if (weap.Proj.WeaponType == WeaponType.TORPEDO
          || weap.Proj.WeaponType == WeaponType.MISSILE)
        {
          COLOR disabled_color = ColorLocalization.Get(ColorLocalKeys.GAME_SYSTEM_DISABLED);
          COLOR destroyed_color = ColorLocalization.Get(ColorLocalKeys.GAME_SYSTEM_DESTROYED);
          if (!p.TypeInfo.SystemData.AllowSystemDamage || p.GetStatus(SystemPart.PROJECTILE_LAUNCHERS) == SystemState.ACTIVE)
            DrawMissile(weap, true, weap.Ammo.Count, pcolor);
          else if (p.GetStatus(SystemPart.PROJECTILE_LAUNCHERS) == SystemState.DESTROYED)
            DrawMissile(weap, false, 0, destroyed_color);
          else if (p.GetStatus(SystemPart.PROJECTILE_LAUNCHERS) == SystemState.DISABLED)
            DrawMissile(weap, false, 0, disabled_color);
        }
        else if (weap.Proj.WeaponType == WeaponType.ION)
        {
          COLOR ion_color = ColorLocalization.Get(ColorLocalKeys.GAME_SYSTEM_ION);
          COLOR disabled_color = ColorLocalization.Get(ColorLocalKeys.GAME_SYSTEM_DISABLED);
          COLOR destroyed_color = ColorLocalization.Get(ColorLocalKeys.GAME_SYSTEM_DESTROYED);
          if (!p.TypeInfo.SystemData.AllowSystemDamage || p.GetStatus(SystemPart.LASER_WEAPONS) == SystemState.ACTIVE)
            DrawIon(weap, true, burst, ion_color, ion_color);
          else if (p.GetStatus(SystemPart.LASER_WEAPONS) == SystemState.DESTROYED)
            DrawIon(weap, false, 0, destroyed_color, destroyed_color);
          else if (p.GetStatus(SystemPart.LASER_WEAPONS) == SystemState.DISABLED)
            DrawIon(weap, false, 0, disabled_color, disabled_color);
        }
        TVScreen2DImmediate.Action_End2D();
      }
    }

    private void DrawLaser(Weapons.WeaponInfo weap, int burst, COLOR pcolor, COLOR color2)
    {
      for (int i = 0; i < weap?.Port.UIPos?.Length; i++)
      {
        if (weap.Port.UIPos[i].x != 0 || weap.Port.UIPos[i].y != 0)
        {
          int wpi = i - weap.Port.Index;
          if (wpi < 0)
            wpi += weap.Port.FirePositions.Length;
          bool highlighted = (wpi >= 0 && wpi < burst);

          float x = weap.Port.UIPos[i].x;
          float y = -weap.Port.UIPos[i].y;

          if (highlighted)
          {
            TVScreen2DImmediate.Draw_FilledBox(x - 2 + Owner.ScreenCenter.x
                                            , y - 2 + Owner.ScreenCenter.y
                                            , x + 2 + Owner.ScreenCenter.x
                                            , y + 2 + Owner.ScreenCenter.y
                                            , pcolor.Value);
          }
          else
          {
            TVScreen2DImmediate.Draw_Box(x - 2 + Owner.ScreenCenter.x
                                            , y - 2 + Owner.ScreenCenter.y
                                            , x + 2 + Owner.ScreenCenter.x
                                            , y + 2 + Owner.ScreenCenter.y
                                            , color2.Value);
          }
        }
      }
    }

    private void DrawIon(Weapons.WeaponInfo weap, bool enabled, int burst, COLOR pcolor, COLOR color2)
    {
      for (int i = 0; i < weap?.Port.UIPos?.Length; i++)
      {
        if (weap.Port.UIPos[i].x != 0 || weap.Port.UIPos[i].y != 0)
        {
          int wpi = i - weap.Port.Index;
          if (wpi < 0)
            wpi += weap.Port.FirePositions.Length;
          bool highlighted = (wpi >= 0 && wpi < burst);

          float x = weap.Port.UIPos[i].x;
          float y = -weap.Port.UIPos[i].y;

          if (highlighted)
          {
            TVScreen2DImmediate.Draw_FilledBox(x - 2 + Owner.ScreenCenter.x
                                            , y - 2 + Owner.ScreenCenter.y
                                            , x + 2 + Owner.ScreenCenter.x
                                            , y + 2 + Owner.ScreenCenter.y
                                            , pcolor.Value);
          }
          else
          {
            TVScreen2DImmediate.Draw_Box(x - 2 + Owner.ScreenCenter.x
                                            , y - 2 + Owner.ScreenCenter.y
                                            , x + 2 + Owner.ScreenCenter.x
                                            , y + 2 + Owner.ScreenCenter.y
                                            , color2.Value);
          }
        }
      }

      float p1_x = -40;
      float p1_y = 42;
      float p2_x = 40;
      float p2_y = 60;

      TVScreen2DImmediate.Draw_FilledBox(p1_x + Owner.ScreenCenter.x
                          , p1_y + Owner.ScreenCenter.y
                          , p2_x + Owner.ScreenCenter.x
                          , p2_y + Owner.ScreenCenter.y
                          , new TV_COLOR(0, 0, 0, 0.4f).GetIntColor());

      TVScreen2DImmediate.Draw_Box(p1_x + Owner.ScreenCenter.x
                                      , p1_y + Owner.ScreenCenter.y
                                      , p2_x + Owner.ScreenCenter.x
                                      , p2_y + Owner.ScreenCenter.y
                                      , pcolor.Value);

      TVScreen2DImmediate.Action_End2D();
      TVScreen2DText.Action_BeginText();

      if (weap.Ammo.Count > -1)
      {
        TVScreen2DText.TextureFont_DrawText("{0}: {1}".F(weap.DisplayName, (enabled ? weap.Ammo.Count.ToString() : "---"))
                                  , p1_x + Owner.ScreenCenter.x + 5
                                  , p1_y + Owner.ScreenCenter.y + 2
                                  , pcolor.Value
                                  , FontFactory.Get(Font.T10).ID);
      }

      TVScreen2DText.Action_EndText();
      TVScreen2DImmediate.Action_Begin2D();
    }

    private void DrawMissile(Weapons.WeaponInfo weap, bool enabled, int ammo, COLOR pcolor)
    {
      float p1_x = -40;
      float p1_y = 42;
      float p2_x = 40;
      float p2_y = 60;

      TVScreen2DImmediate.Draw_FilledBox(p1_x + Owner.ScreenCenter.x
                                , p1_y + Owner.ScreenCenter.y
                                , p2_x + Owner.ScreenCenter.x
                                , p2_y + Owner.ScreenCenter.y
                                , new TV_COLOR(0, 0, 0, 0.4f).GetIntColor());

      TVScreen2DImmediate.Draw_Box(p1_x + Owner.ScreenCenter.x
                                      , p1_y + Owner.ScreenCenter.y
                                      , p2_x + Owner.ScreenCenter.x
                                      , p2_y + Owner.ScreenCenter.y
                                      , pcolor.Value);

      TVScreen2DImmediate.Action_End2D();
      TVScreen2DText.Action_BeginText();

      TVScreen2DText.TextureFont_DrawText("{0}: {1}".F(weap.DisplayName, (enabled ? ammo.ToString() : "---"))
                                , p1_x + Owner.ScreenCenter.x + 5
                                , p1_y + Owner.ScreenCenter.y + 2
                                , pcolor.Value
                                , FontFactory.Get(Font.T10).ID);

      TVScreen2DText.Action_EndText();
      TVScreen2DImmediate.Action_Begin2D();
    }
  }
}
