using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.Player;
using SWEndor.Weapons;

namespace SWEndor.UI.Widgets
{
  public class CrossHair : Widget
  {
    public CrossHair(Screen2D owner) : base(owner, "crosshair") { }

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

      WeaponInfo weap = null;
      int burst = 1;
      TV_COLOR pcolor = (p.Faction == null) ? new TV_COLOR(1, 1, 1, 1) : p.Faction.Color;

      p.TypeInfo.InterpretWeapon(p.ID, Owner.Engine.PlayerInfo.PrimaryWeapon, out weap, out burst);
      if (weap != null)
      {
        TVScreen2DImmediate.Action_Begin2D();

        float xs = 1;
        float ys = 1.5f;
        float k = 16;
        float l = 4;
        float xmin = 10;
        float ymin = 10;

        for (int i = 0; i < weap.FirePositions.Length; i++)
        {
          TV_2DVECTOR vec0 = new TV_2DVECTOR(weap.FirePositions[i].x * xs, -weap.FirePositions[i].y * ys);
          if (vec0.x < -xmin) vec0.x = -xmin;
          if (vec0.x > xmin) vec0.x = xmin;
          if (vec0.y < -ymin) vec0.y = -ymin;
          if (vec0.y > ymin) vec0.y = ymin;

          TV_2DVECTOR vec1 = new TV_2DVECTOR();
          TV_2DVECTOR vec2 = new TV_2DVECTOR();
          int wpi = i - weap.CurrentPositionIndex;
          if (wpi < 0)
            wpi += weap.FirePositions.Length;
          bool highlighted = (wpi >= 0 && wpi < burst);
          if (vec0.x != 0)
          {
            float m = vec0.y / vec0.x;
            if (m < 0) { m = -m; }
            vec1 = new TV_2DVECTOR(vec0.x + (vec0.x > 0 ? 1 : -1) * (k + l * m) / (1 + m)
                                 , vec0.y + (vec0.y > 0 ? 1 : -1) * (k * m - l) / (1 + m));
            vec2 = new TV_2DVECTOR(vec0.x + (vec0.x > 0 ? 1 : -1) * (k - l * m) / (1 + m)
                                 , vec0.y + (vec0.y > 0 ? 1 : -1) * (k * m + l) / (1 + m));


            TVScreen2DImmediate.Draw_Triangle(vec0.x + Owner.Engine.ScreenWidth / 2
                                                  , vec0.y + Owner.Engine.ScreenHeight / 2
                                                  , vec1.x + Owner.Engine.ScreenWidth / 2
                                                  , vec1.y + Owner.Engine.ScreenHeight / 2
                                                  , vec2.x + Owner.Engine.ScreenWidth / 2
                                                  , vec2.y + Owner.Engine.ScreenHeight / 2
                                                  , highlighted ? new TV_COLOR(1, 0.5f, 0, 1).GetIntColor() : pcolor.GetIntColor());
          }
          else if (vec0.y != 0)
          {
            vec1 = new TV_2DVECTOR(k
                                 , vec0.y + (vec0.y > 0 ? l : -l));
            vec2 = new TV_2DVECTOR(-vec1.x, vec1.y);


            TVScreen2DImmediate.Draw_Triangle(vec0.x + Owner.Engine.ScreenWidth / 2
                                                  , vec0.y + Owner.Engine.ScreenHeight / 2
                                                  , vec1.x + Owner.Engine.ScreenWidth / 2
                                                  , vec1.y + Owner.Engine.ScreenHeight / 2
                                                  , vec2.x + Owner.Engine.ScreenWidth / 2
                                                  , vec2.y + Owner.Engine.ScreenHeight / 2
                                                  , highlighted ? new TV_COLOR(1, 0.5f, 0, 1).GetIntColor() : pcolor.GetIntColor());
          }
          else
          {
            TVScreen2DImmediate.Draw_Line(vec0.x + l + Owner.Engine.ScreenWidth / 2
                                                          , vec0.y + Owner.Engine.ScreenHeight / 2
                                                          , vec0.x + Owner.Engine.ScreenWidth / 2
                                                          , vec0.y + l + Owner.Engine.ScreenHeight / 2
                                                          , highlighted ? new TV_COLOR(1, 0.5f, 0, 1).GetIntColor() : pcolor.GetIntColor());

            TVScreen2DImmediate.Draw_Line(vec0.x - l + Owner.Engine.ScreenWidth / 2
                                                          , vec0.y + Owner.Engine.ScreenHeight / 2
                                                          , vec0.x + Owner.Engine.ScreenWidth / 2
                                                          , vec0.y + l + Owner.Engine.ScreenHeight / 2
                                                          , highlighted ? new TV_COLOR(1, 0.5f, 0, 1).GetIntColor() : pcolor.GetIntColor());

            TVScreen2DImmediate.Draw_Line(vec0.x + l + Owner.Engine.ScreenWidth / 2
                                                          , vec0.y + Owner.Engine.ScreenHeight / 2
                                                          , vec0.x + Owner.Engine.ScreenWidth / 2
                                                          , vec0.y - l + Owner.Engine.ScreenHeight / 2
                                                          , highlighted ? new TV_COLOR(1, 0.5f, 0, 1).GetIntColor() : pcolor.GetIntColor());

            TVScreen2DImmediate.Draw_Line(vec0.x - l + Owner.Engine.ScreenWidth / 2
                                                          , vec0.y + Owner.Engine.ScreenHeight / 2
                                                          , vec0.x + Owner.Engine.ScreenWidth / 2
                                                          , vec0.y - l + Owner.Engine.ScreenHeight / 2
                                                          , highlighted ? new TV_COLOR(1, 0.5f, 0, 1).GetIntColor() : pcolor.GetIntColor());

          }
        }
        TVScreen2DImmediate.Action_End2D();
      }

      p.TypeInfo.InterpretWeapon(p.ID, Owner.Engine.PlayerInfo.SecondaryWeapon, out weap, out burst);
      if (weap != null)
      {
        if (Owner.Engine.PlayerInfo.SecondaryWeapon.Contains("torp"))
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
              TVScreen2DImmediate.Draw_FilledBox(p1_x + Owner.Engine.ScreenWidth / 2
                                                    , p1_y + Owner.Engine.ScreenHeight / 2
                                                    , p2_x + Owner.Engine.ScreenWidth / 2
                                                    , p2_y + Owner.Engine.ScreenHeight / 2
                                                    , pcolor.GetIntColor());
            }
            else
            {
              TVScreen2DImmediate.Draw_Box(p1_x + Owner.Engine.ScreenWidth / 2
                                    , p1_y + Owner.Engine.ScreenHeight / 2
                                    , p2_x + Owner.Engine.ScreenWidth / 2
                                    , p2_y + Owner.Engine.ScreenHeight / 2
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
        else if (Owner.Engine.PlayerInfo.SecondaryWeapon.Contains("misl"))
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
              TVScreen2DImmediate.Draw_FilledBox(p1_x + Owner.Engine.ScreenWidth / 2
                                                    , p1_y + Owner.Engine.ScreenHeight / 2
                                                    , p2_x + Owner.Engine.ScreenWidth / 2
                                                    , p2_y + Owner.Engine.ScreenHeight / 2
                                                    , pcolor.GetIntColor());
            }
            /*else
            {
              TVScreen2DImmediate.Draw_Box(p1_x + Owner.Engine.ScreenWidth / 2
                                    , p1_y + Owner.Engine.ScreenHeight / 2
                                    , p2_x + Owner.Engine.ScreenWidth / 2
                                    , p2_y + Owner.Engine.ScreenHeight / 2
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

          TVScreen2DImmediate.Draw_FilledBox(p1_x + Owner.Engine.ScreenWidth / 2
                                                    , p1_y + Owner.Engine.ScreenHeight / 2
                                                    , p1_x + (p2_x - p1_x) * tremain + Owner.Engine.ScreenWidth / 2
                                                    , p2_y + Owner.Engine.ScreenHeight / 2
                                                    , new TV_COLOR(0.6f, 0.6f, 1, 1).GetIntColor());

          TVScreen2DImmediate.Draw_Box(p1_x + Owner.Engine.ScreenWidth / 2
                                                    , p1_y + Owner.Engine.ScreenHeight / 2
                                                    , p2_x + Owner.Engine.ScreenWidth / 2
                                                    , p2_y + Owner.Engine.ScreenHeight / 2
                                                    , new TV_COLOR(1, 0.5f, 0, 1).GetIntColor());
        }
        TVScreen2DImmediate.Action_End2D();
      }
    }
  }
}
