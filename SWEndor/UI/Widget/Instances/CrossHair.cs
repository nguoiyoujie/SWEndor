using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Traits;
using SWEndor.ActorTypes;
using SWEndor.Weapons;
using System.IO;

namespace SWEndor.UI.Widgets
{
  public class CrossHair : Widget
  {
    public CrossHair(Screen2D owner) : base(owner, "crosshair")
    {
      tex = Engine.TrueVision.TVTextureFactory.LoadTexture(Path.Combine(Globals.ImagePath, "crosshair/Crosshair.png")
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
        using (var v = ActorFactory.Get(PlayerInfo.ActorID))
        {
          if (v == null)
            return false;

          ActorInfo p = v.Value;
          return (!Owner.ShowPage
          && !p.StateModel.IsDyingOrDead
          && Owner.ShowUI);
        }
      }
    }

    public override void Draw()
    {
      using (var v = ActorFactory.Get(PlayerInfo.ActorID))
      {
        if (v == null)
          return;

        ActorInfo p = v.Value;
        if (!p.Active)
          return;

        WeaponInfo weap = null;
        int burst = 1;

        p.TypeInfo.InterpretWeapon(p, PlayerInfo.PrimaryWeapon, out weap, out burst);
        if (weap != null)
        {
          TVScreen2DImmediate.Action_Begin2D();

          TVScreen2DImmediate.Draw_Texture(tex
                                         , Engine.ScreenWidth / 2 - 32
                                         , Engine.ScreenHeight / 2 - 32
                                         , Engine.ScreenWidth / 2 + 32
                                         , Engine.ScreenHeight / 2 + 32
                                         , pcolor.GetIntColor()
                                         , pcolor.GetIntColor()
                                         , pcolor.GetIntColor()
                                         , pcolor.GetIntColor());

          for (int i = 0; i < weap.UIFirePositions.Length; i++)
          {
            if (weap.UIFirePositions[i].x != 0 || weap.UIFirePositions[i].y != 0)
            {
              int wpi = i - weap.CurrentPositionIndex;
              if (wpi < 0)
                wpi += weap.FirePositions.Length;
              bool highlighted = (wpi >= 0 && wpi < burst);

              float x = weap.UIFirePositions[i].x;
              float y = -weap.UIFirePositions[i].y;

              if (highlighted)
              {
                TVScreen2DImmediate.Draw_FilledBox(x - 2 + Engine.ScreenWidth / 2
                                                , y - 2 + Engine.ScreenHeight / 2
                                                , x + 2 + Engine.ScreenWidth / 2
                                                , y + 2 + Engine.ScreenHeight / 2
                                                , pcolor.GetIntColor());
              }
              else
              {
                TVScreen2DImmediate.Draw_Box(x - 2 + Engine.ScreenWidth / 2
                                                , y - 2 + Engine.ScreenHeight / 2
                                                , x + 2 + Engine.ScreenWidth / 2
                                                , y + 2 + Engine.ScreenHeight / 2
                                                , new TV_COLOR(0.4f, 0.5f, 1f, 0.4f).GetIntColor());
              }
            }
          }
          TVScreen2DImmediate.Action_End2D();
        }

        p.TypeInfo.InterpretWeapon(p, PlayerInfo.SecondaryWeapon, out weap, out burst);
        if (weap != null)
        {
          if (PlayerInfo.SecondaryWeapon.Contains("torp"))
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
                TVScreen2DImmediate.Draw_FilledBox(p1_x + Engine.ScreenWidth / 2
                                                      , p1_y + Engine.ScreenHeight / 2
                                                      , p2_x + Engine.ScreenWidth / 2
                                                      , p2_y + Engine.ScreenHeight / 2
                                                      , pcolor.GetIntColor());
              }
              else
              {
                TVScreen2DImmediate.Draw_Box(p1_x + Engine.ScreenWidth / 2
                                      , p1_y + Engine.ScreenHeight / 2
                                      , p2_x + Engine.ScreenWidth / 2
                                      , p2_y + Engine.ScreenHeight / 2
                                      , pcolor.GetIntColor());
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
          else if (PlayerInfo.SecondaryWeapon.Contains("misl"))
          {
            float p1_x = -40;
            float p1_y = 28;
            float p2_x = -38;
            float p2_y = 36;
            float p3_x = 4;
            float p3_y = 10;
            float tremain = weap.Ammo;
            float tmax = weap.MaxAmmo;
            int t = 0;

            while (t < tremain) //|| t < tmax)
            {
              if (t < tremain)
              {
                TVScreen2DImmediate.Draw_FilledBox(p1_x + Engine.ScreenWidth / 2
                                                      , p1_y + Engine.ScreenHeight / 2
                                                      , p2_x + Engine.ScreenWidth / 2
                                                      , p2_y + Engine.ScreenHeight / 2
                                                      , pcolor.GetIntColor());
              }
              /*else
              {
                TVScreen2DImmediate.Draw_Box(p1_x + Engine.ScreenWidth / 2
                                      , p1_y + Engine.ScreenHeight / 2
                                      , p2_x + Engine.ScreenWidth / 2
                                      , p2_y + Engine.ScreenHeight / 2
                                      , pcolor.GetIntColor());
              }*/

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
          }
          else if (weap.MaxAmmo > 0)
          {
            float p1_x = -25;
            float p1_y = 28;
            float p2_x = 25;
            float p2_y = 33;
            float tremain = (float)weap.Ammo / weap.MaxAmmo;

            TVScreen2DImmediate.Draw_FilledBox(p1_x + Engine.ScreenWidth / 2
                                                      , p1_y + Engine.ScreenHeight / 2
                                                      , p1_x + (p2_x - p1_x) * tremain + Engine.ScreenWidth / 2
                                                      , p2_y + Engine.ScreenHeight / 2
                                                      , new TV_COLOR(0.6f, 0.6f, 1, 1).GetIntColor());

            TVScreen2DImmediate.Draw_Box(p1_x + Engine.ScreenWidth / 2
                                                      , p1_y + Engine.ScreenHeight / 2
                                                      , p2_x + Engine.ScreenWidth / 2
                                                      , p2_y + Engine.ScreenHeight / 2
                                                      , new TV_COLOR(1, 0.5f, 0, 1).GetIntColor());
          }
          TVScreen2DImmediate.Action_End2D();
        }
      }
    }
  }
}
