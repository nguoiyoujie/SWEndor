using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Models;
using Primrose.Primitives.Extensions;
using SWEndor.Weapons;
using System.IO;

namespace SWEndor.UI.Widgets
{
  public class CrossHair : Widget
  {
    public CrossHair(Screen2D owner) : base(owner, "crosshair")
    {
      tex = Engine.TrueVision.TVTextureFactory.LoadTexture(Path.Combine(Globals.ImagePath, @"crosshair\Crosshair.png")
                                                                   , "Crosshair"
                                                                   , 64
                                                                   , 64
                                                                   , CONST_TV_COLORKEY.TV_COLORKEY_USE_ALPHA_CHANNEL);
    }
    int tex;

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
      TVScreen2DImmediate.Draw_Texture(tex
                               , Owner.ScreenCenter.x - 32
                               , Owner.ScreenCenter.y - 32
                               , Owner.ScreenCenter.x + 32
                               , Owner.ScreenCenter.y + 32
                               , icolor
                               , icolor
                               , icolor
                               , icolor);

      WeaponInfo weap = PlayerInfo.PrimaryWeapon.Weapon;
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
        if (weap.Proj.Type == WeaponType.TORPEDO
          || weap.Proj.Type == WeaponType.MISSILE)
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
        else if (weap.Proj.Type == WeaponType.ION)
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

    private void DrawLaser(WeaponInfo weap, int burst, COLOR pcolor, COLOR color2)
    {
      for (int i = 0; i < weap?.Port.UIPos?.Length; i++)
      {
        if (weap.Port.UIPos[i].x != 0 || weap.Port.UIPos[i].y != 0)
        {
          int wpi = i - weap.Port.Index;
          if (wpi < 0)
            wpi += weap.Port.FirePos.Length;
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

    private void DrawIon(WeaponInfo weap, bool enabled, int burst, COLOR pcolor, COLOR color2)
    {
      for (int i = 0; i < weap?.Port.UIPos?.Length; i++)
      {
        if (weap.Port.UIPos[i].x != 0 || weap.Port.UIPos[i].y != 0)
        {
          int wpi = i - weap.Port.Index;
          if (wpi < 0)
            wpi += weap.Port.FirePos.Length;
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

      /*
      float p1_x = -25;
      float p1_y = 28;
      float p2_x = 25;
      float p2_y = 33;
      float tremain = (float)weap.Ammo / weap.MaxAmmo;

      TVScreen2DImmediate.Draw_FilledBox(p1_x + Owner.ScreenCenter.x
                                                , p1_y + Owner.ScreenCenter.y
                                                , p1_x + (p2_x - p1_x) * tremain + Owner.ScreenCenter.x
                                                , p2_y + Owner.ScreenCenter.y
                                                , new TV_COLOR(0.6f, 0.6f, 1, 1).GetIntColor());

      TVScreen2DImmediate.Draw_Box(p1_x + Owner.ScreenCenter.x
                                                , p1_y + Owner.ScreenCenter.y
                                                , p2_x + Owner.ScreenCenter.x
                                                , p2_y + Owner.ScreenCenter.y
                                                , new TV_COLOR(1, 0.5f, 0, 1).GetIntColor());
                                                */
    }

    private void DrawMissile(WeaponInfo weap, bool enabled, int ammo, COLOR pcolor)
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

      /*
      float p1_x = -40;
      float p1_y = 28;
      float p2_x = -38;
      float p2_y = 36;
      float p3_x = 4;
      float p3_y = 10;
      float tremain = weap.Ammo;
      float tmax = weap.MaxAmmo;
      int t = 0;

      while (t < tremain)
      {
        TVScreen2DImmediate.Draw_FilledBox(p1_x + Owner.ScreenCenter.x
                                              , p1_y + Owner.ScreenCenter.y
                                              , p2_x + Owner.ScreenCenter.x
                                              , p2_y + Owner.ScreenCenter.y
                                              , pcolor);

        p1_x += p3_x;
        p2_x += p3_x;

        t++;
        if (t % 20 == 0)
        {
          p1_x = -40;
          p2_x = -38;
          p1_y += p3_y;
          p2_y += p3_y;
        }
      }
      */
    }

    /*
    private void DrawTorp(WeaponInfo weap, int burst, int pcolor)
    {
      float p1_x = -40;
      float p1_y = 28;
      float p2_x = -36;
      float p2_y = 36;
      float p3_x = 6;
      float p3_y = 10;
      float tremain = weap.Ammo;
      float tmax = weap.MaxAmmo;
      int t = 0;

      while (t < tremain || t < tmax)
      {
        if (t < tremain)
        {
          TVScreen2DImmediate.Draw_FilledBox(p1_x + Owner.ScreenCenter.x
                                                , p1_y + Owner.ScreenCenter.y
                                                , p2_x + Owner.ScreenCenter.x
                                                , p2_y + Owner.ScreenCenter.y
                                                , pcolor);
        }
        else
        {
          TVScreen2DImmediate.Draw_Box(p1_x + Owner.ScreenCenter.x
                                , p1_y + Owner.ScreenCenter.y
                                , p2_x + Owner.ScreenCenter.x
                                , p2_y + Owner.ScreenCenter.y
                                , pcolor);
        }

        p1_x += p3_x;
        p2_x += p3_x;

        t++;
        if (t % 10 == 0)
        {
          p1_x = -40;
          p2_x = -36;
          p1_y += p3_y;
          p2_y += p3_y;
        }
      }
    }
    */
  }
}
