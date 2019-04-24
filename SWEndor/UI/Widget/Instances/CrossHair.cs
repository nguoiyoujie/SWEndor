using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.Player;
using SWEndor.Weapons;

namespace SWEndor.UI.Widgets
{
  public class CrossHair : Widget
  {
    public CrossHair() : base("crosshair") { }

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

      WeaponInfo weap = null;
      int burst = 1;
      TV_COLOR pcolor = (p.Faction == null) ? new TV_COLOR(1, 1, 1, 1) : p.Faction.Color;

      p.TypeInfo.InterpretWeapon(p.ID, Globals.Engine.PlayerInfo.PrimaryWeapon, out weap, out burst);
      if (weap != null)
      {
        Globals.Engine.TVScreen2DImmediate.Action_Begin2D();

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


            Globals.Engine.TVScreen2DImmediate.Draw_Triangle(vec0.x + Globals.Engine.ScreenWidth / 2
                                                  , vec0.y + Globals.Engine.ScreenHeight / 2
                                                  , vec1.x + Globals.Engine.ScreenWidth / 2
                                                  , vec1.y + Globals.Engine.ScreenHeight / 2
                                                  , vec2.x + Globals.Engine.ScreenWidth / 2
                                                  , vec2.y + Globals.Engine.ScreenHeight / 2
                                                  , highlighted ? new TV_COLOR(1, 0.5f, 0, 1).GetIntColor() : pcolor.GetIntColor());
          }
          else if (vec0.y != 0)
          {
            vec1 = new TV_2DVECTOR(k
                                 , vec0.y + (vec0.y > 0 ? l : -l));
            vec2 = new TV_2DVECTOR(-vec1.x, vec1.y);


            Globals.Engine.TVScreen2DImmediate.Draw_Triangle(vec0.x + Globals.Engine.ScreenWidth / 2
                                                  , vec0.y + Globals.Engine.ScreenHeight / 2
                                                  , vec1.x + Globals.Engine.ScreenWidth / 2
                                                  , vec1.y + Globals.Engine.ScreenHeight / 2
                                                  , vec2.x + Globals.Engine.ScreenWidth / 2
                                                  , vec2.y + Globals.Engine.ScreenHeight / 2
                                                  , highlighted ? new TV_COLOR(1, 0.5f, 0, 1).GetIntColor() : pcolor.GetIntColor());
          }
          else
          {
            Globals.Engine.TVScreen2DImmediate.Draw_Line(vec0.x + l + Globals.Engine.ScreenWidth / 2
                                                          , vec0.y + Globals.Engine.ScreenHeight / 2
                                                          , vec0.x + Globals.Engine.ScreenWidth / 2
                                                          , vec0.y + l + Globals.Engine.ScreenHeight / 2
                                                          , highlighted ? new TV_COLOR(1, 0.5f, 0, 1).GetIntColor() : pcolor.GetIntColor());

            Globals.Engine.TVScreen2DImmediate.Draw_Line(vec0.x - l + Globals.Engine.ScreenWidth / 2
                                                          , vec0.y + Globals.Engine.ScreenHeight / 2
                                                          , vec0.x + Globals.Engine.ScreenWidth / 2
                                                          , vec0.y + l + Globals.Engine.ScreenHeight / 2
                                                          , highlighted ? new TV_COLOR(1, 0.5f, 0, 1).GetIntColor() : pcolor.GetIntColor());

            Globals.Engine.TVScreen2DImmediate.Draw_Line(vec0.x + l + Globals.Engine.ScreenWidth / 2
                                                          , vec0.y + Globals.Engine.ScreenHeight / 2
                                                          , vec0.x + Globals.Engine.ScreenWidth / 2
                                                          , vec0.y - l + Globals.Engine.ScreenHeight / 2
                                                          , highlighted ? new TV_COLOR(1, 0.5f, 0, 1).GetIntColor() : pcolor.GetIntColor());

            Globals.Engine.TVScreen2DImmediate.Draw_Line(vec0.x - l + Globals.Engine.ScreenWidth / 2
                                                          , vec0.y + Globals.Engine.ScreenHeight / 2
                                                          , vec0.x + Globals.Engine.ScreenWidth / 2
                                                          , vec0.y - l + Globals.Engine.ScreenHeight / 2
                                                          , highlighted ? new TV_COLOR(1, 0.5f, 0, 1).GetIntColor() : pcolor.GetIntColor());

          }
        }
        Globals.Engine.TVScreen2DImmediate.Action_End2D();
      }

      p.TypeInfo.InterpretWeapon(p.ID, Globals.Engine.PlayerInfo.SecondaryWeapon, out weap, out burst);
      if (weap != null)
      {
        if (Globals.Engine.PlayerInfo.SecondaryWeapon.Contains("torp"))
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
              Globals.Engine.TVScreen2DImmediate.Draw_FilledBox(p1_x + Globals.Engine.ScreenWidth / 2
                                                    , p1_y + Globals.Engine.ScreenHeight / 2
                                                    , p2_x + Globals.Engine.ScreenWidth / 2
                                                    , p2_y + Globals.Engine.ScreenHeight / 2
                                                    , pcolor.GetIntColor());
            }
            else
            {
              Globals.Engine.TVScreen2DImmediate.Draw_Box(p1_x + Globals.Engine.ScreenWidth / 2
                                    , p1_y + Globals.Engine.ScreenHeight / 2
                                    , p2_x + Globals.Engine.ScreenWidth / 2
                                    , p2_y + Globals.Engine.ScreenHeight / 2
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
        else if (Globals.Engine.PlayerInfo.SecondaryWeapon.Contains("misl"))
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
              Globals.Engine.TVScreen2DImmediate.Draw_FilledBox(p1_x + Globals.Engine.ScreenWidth / 2
                                                    , p1_y + Globals.Engine.ScreenHeight / 2
                                                    , p2_x + Globals.Engine.ScreenWidth / 2
                                                    , p2_y + Globals.Engine.ScreenHeight / 2
                                                    , pcolor.GetIntColor());
            }
            /*else
            {
              Globals.Engine.TVScreen2DImmediate.Draw_Box(p1_x + Globals.Engine.ScreenWidth / 2
                                    , p1_y + Globals.Engine.ScreenHeight / 2
                                    , p2_x + Globals.Engine.ScreenWidth / 2
                                    , p2_y + Globals.Engine.ScreenHeight / 2
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

          Globals.Engine.TVScreen2DImmediate.Draw_FilledBox(p1_x + Globals.Engine.ScreenWidth / 2
                                                    , p1_y + Globals.Engine.ScreenHeight / 2
                                                    , p1_x + (p2_x - p1_x) * tremain + Globals.Engine.ScreenWidth / 2
                                                    , p2_y + Globals.Engine.ScreenHeight / 2
                                                    , new TV_COLOR(0.6f, 0.6f, 1, 1).GetIntColor());

          Globals.Engine.TVScreen2DImmediate.Draw_Box(p1_x + Globals.Engine.ScreenWidth / 2
                                                    , p1_y + Globals.Engine.ScreenHeight / 2
                                                    , p2_x + Globals.Engine.ScreenWidth / 2
                                                    , p2_y + Globals.Engine.ScreenHeight / 2
                                                    , new TV_COLOR(1, 0.5f, 0, 1).GetIntColor());
        }
        Globals.Engine.TVScreen2DImmediate.Action_End2D();
      }


      /*
      float p1_x = 5;
      float p1_y = 5;
      float p2_x = 22;
      float p2_y = 8;
      float p3_x = 16;
      float p3_y = 14;
      TV_COLOR pcolor = (p.Faction == null) ? new TV_COLOR(1, 1, 1, 1) : p.Faction.Color;

      Globals.Engine.TVScreen2DImmediate.Action_Begin2D();
      if (p.TypeInfo is XWingATI)
      {
        p1_x = 5;
        p1_y = 5;
        p2_x = 22;
        p2_y = 8;
        p3_x = 16;
        p3_y = 14;

        for (int i = -1; i <= 1; i += 2)
        {
          for (int j = -1; j <= 1; j += 2)
          {
            int n = 0;
            if (i > 0)
            {
              if (j > 0)
              {
                n = 2;
              }
              else
              {
                n = 0;
              }
            }
            else
            {
              if (j > 0)
              {
                n = 1;
              }
              else
              {
                n = 3;
              }
            }

            Globals.Engine.TVScreen2DImmediate.Draw_Triangle(p1_x * i + Globals.Engine.ScreenWidth / 2
                                                                , p1_y * j + Globals.Engine.ScreenHeight / 2
                                                                , p2_x * i + Globals.Engine.ScreenWidth / 2
                                                                , p2_y * j + Globals.Engine.ScreenHeight / 2
                                                                , p3_x * i + Globals.Engine.ScreenWidth / 2
                                                                , p3_y * j + Globals.Engine.ScreenHeight / 2
                                                                , (p.TypeInfo.Globals.Engine.Player.PrimaryWeapon.IsStateFDefined("LaserPosition") && p.GetStateF("LaserPosition") == n) ? new TV_COLOR(1, 0.5f, 0, 1).GetIntColor() : pcolor.GetIntColor());
          }
        }
      }
      else if (p.TypeInfo is YWingATI)
      {
        p1_x = 0;
        p1_y = 0;
        p2_x = 6;
        p2_y = 12;
        p3_x = -6;
        p3_y = 12;

        Globals.Engine.TVScreen2DImmediate.Draw_Triangle(p1_x + Globals.Engine.ScreenWidth / 2
                                                            , p1_y + Globals.Engine.ScreenHeight / 2
                                                            , p2_x + Globals.Engine.ScreenWidth / 2
                                                            , p2_y + Globals.Engine.ScreenHeight / 2
                                                            , p3_x + Globals.Engine.ScreenWidth / 2
                                                            , p3_y + Globals.Engine.ScreenHeight / 2
                                                            , new TV_COLOR(1, 0.5f, 0, 1).GetIntColor());

        Globals.Engine.TVScreen2DImmediate.Draw_Line(0 + Globals.Engine.ScreenWidth / 2
                                                            , 0 + Globals.Engine.ScreenHeight / 2
                                                            , 0 + Globals.Engine.ScreenWidth / 2
                                                            , -25 + Globals.Engine.ScreenHeight / 2
                                                            , new TV_COLOR(1, 0.5f, 0, 1).GetIntColor());

        Globals.Engine.TVScreen2DImmediate.Draw_Line(-10 + Globals.Engine.ScreenWidth / 2
                                                            , -25 + Globals.Engine.ScreenHeight / 2
                                                            , 10 + Globals.Engine.ScreenWidth / 2
                                                            , -25 + Globals.Engine.ScreenHeight / 2
                                                            , new TV_COLOR(1, 0.5f, 0, 1).GetIntColor());


        Globals.Engine.TVScreen2DImmediate.Draw_Line(0 + Globals.Engine.ScreenWidth / 2
                                                            , 12 + Globals.Engine.ScreenHeight / 2
                                                            , 0 + Globals.Engine.ScreenWidth / 2
                                                            , 25 + Globals.Engine.ScreenHeight / 2
                                                            , new TV_COLOR(1, 0.5f, 0, 1).GetIntColor());

        Globals.Engine.TVScreen2DImmediate.Draw_Line(-25 + Globals.Engine.ScreenWidth / 2
                                                            , 25 + Globals.Engine.ScreenHeight / 2
                                                            , 25 + Globals.Engine.ScreenWidth / 2
                                                            , 25 + Globals.Engine.ScreenHeight / 2
                                                            , new TV_COLOR(1, 0.5f, 0, 1).GetIntColor());

        if ((Globals.Engine.Player.PrimaryWeapon.Contains("photon") || Globals.Engine.Player.SecondaryWeapon.Contains("photon")) && p.GetStateB("EnablePhoton"))
        {
          p1_x = -25;
          p1_y = 28;
          p2_x = 25;
          p2_y = 33;

          float tremain = p.GetStateF("PhotonRemaining") / p.GetStateF("PhotonMax");


          Globals.Engine.TVScreen2DImmediate.Draw_FilledBox(p1_x + Globals.Engine.ScreenWidth / 2
                                                    , p1_y + Globals.Engine.ScreenHeight / 2
                                                    , p1_x + (p2_x - p1_x) * tremain + Globals.Engine.ScreenWidth / 2
                                                    , p2_y + Globals.Engine.ScreenHeight / 2
                                                    , new TV_COLOR(0.6f, 0.6f, 1, 1).GetIntColor());

          Globals.Engine.TVScreen2DImmediate.Draw_Box(p1_x + Globals.Engine.ScreenWidth / 2
                                                    , p1_y + Globals.Engine.ScreenHeight / 2
                                                    , p2_x + Globals.Engine.ScreenWidth / 2
                                                    , p2_y + Globals.Engine.ScreenHeight / 2
                                                    , new TV_COLOR(1, 0.5f, 0, 1).GetIntColor());
        }
      }
      if (p.TypeInfo is AWingATI)
      {
        p1_x = 10;
        p1_y = 0;
        p2_x = 20;
        p2_y = 5;
        p3_x = 20;
        p3_y = -5;

        for (int i = -1; i <= 1; i += 2)
        {
          int n = 0;
          if (i > 0)
          {
            n = 0;
          }
          else
          {
            n = 2;
          }

          Globals.Engine.TVScreen2DImmediate.Draw_Triangle(p1_x * i + Globals.Engine.ScreenWidth / 2
                                                              , p1_y + Globals.Engine.ScreenHeight / 2
                                                              , p2_x * i + Globals.Engine.ScreenWidth / 2
                                                              , p2_y + Globals.Engine.ScreenHeight / 2
                                                              , p3_x * i + Globals.Engine.ScreenWidth / 2
                                                              , p3_y + Globals.Engine.ScreenHeight / 2
                                                              , (p.IsStateFDefined("LaserPosition") && p.GetStateF("LaserPosition") == n) ? new TV_COLOR(1, 0.5f, 0, 1).GetIntColor() : pcolor.GetIntColor());

          Globals.Engine.TVScreen2DImmediate.Draw_Circle(Globals.Engine.ScreenWidth / 2
                                                              , Globals.Engine.ScreenHeight / 2
                                                              , 5
                                                              , 24
                                                              , (p.IsStateFDefined("LaserPosition") && p.GetStateF("LaserPosition") != 0 && p.GetStateF("LaserPosition") != 2) ? new TV_COLOR(1, 0.5f, 0, 1).GetIntColor() : pcolor.GetIntColor());

        }
      }
      if (p.TypeInfo is BWingATI)
      {
        p1_x = 10;
        p1_y = 0;
        p2_x = 22;
        p2_y = 5;
        p3_x = 22;
        p3_y = -5;

        Globals.Engine.TVScreen2DImmediate.Draw_Triangle(p1_x + Globals.Engine.ScreenWidth / 2
                                                            , p1_y + Globals.Engine.ScreenHeight / 2
                                                            , p2_x + Globals.Engine.ScreenWidth / 2
                                                            , p2_y + Globals.Engine.ScreenHeight / 2
                                                            , p3_x + Globals.Engine.ScreenWidth / 2
                                                            , p3_y + Globals.Engine.ScreenHeight / 2
                                                            , (p.IsStateFDefined("LaserPosition") && p.GetStateF("LaserPosition") == 3) ? new TV_COLOR(1, 0.5f, 0, 1).GetIntColor() : pcolor.GetIntColor());

        Globals.Engine.TVScreen2DImmediate.Draw_Triangle(-p1_x + Globals.Engine.ScreenWidth / 2
                                                            , -p1_y + Globals.Engine.ScreenHeight / 2
                                                            , -p2_x + Globals.Engine.ScreenWidth / 2
                                                            , -p2_y + Globals.Engine.ScreenHeight / 2
                                                            , -p3_x + Globals.Engine.ScreenWidth / 2
                                                            , -p3_y + Globals.Engine.ScreenHeight / 2
                                                            , (p.IsStateFDefined("LaserPosition") && p.GetStateF("LaserPosition") == 2) ? new TV_COLOR(1, 0.5f, 0, 1).GetIntColor() : pcolor.GetIntColor());

        Globals.Engine.TVScreen2DImmediate.Draw_Triangle(p1_y + Globals.Engine.ScreenWidth / 2
                                                            , p1_x + Globals.Engine.ScreenHeight / 2
                                                            , p2_y + Globals.Engine.ScreenWidth / 2
                                                            , p2_x + Globals.Engine.ScreenHeight / 2
                                                            , p3_y + Globals.Engine.ScreenWidth / 2
                                                            , p3_x + Globals.Engine.ScreenHeight / 2
                                                            , (p.IsStateFDefined("LaserPosition") && p.GetStateF("LaserPosition") == 0) ? new TV_COLOR(1, 0.5f, 0, 1).GetIntColor() : pcolor.GetIntColor());

        Globals.Engine.TVScreen2DImmediate.Draw_Triangle(-p1_y + Globals.Engine.ScreenWidth / 2
                                                            , -p1_x + Globals.Engine.ScreenHeight / 2
                                                            , -p2_y + Globals.Engine.ScreenWidth / 2
                                                            , -p2_x + Globals.Engine.ScreenHeight / 2
                                                            , -p3_y + Globals.Engine.ScreenWidth / 2
                                                            , -p3_x + Globals.Engine.ScreenHeight / 2
                                                            , (p.IsStateFDefined("LaserPosition") && p.GetStateF("LaserPosition") == 1) ? new TV_COLOR(1, 0.5f, 0, 1).GetIntColor() : pcolor.GetIntColor());

        Globals.Engine.TVScreen2DImmediate.Draw_Line(0 + Globals.Engine.ScreenWidth / 2
                                                  , 22 + Globals.Engine.ScreenHeight / 2
                                                  , 0 + Globals.Engine.ScreenWidth / 2
                                                  , 25 + Globals.Engine.ScreenHeight / 2
                                                  , pcolor.GetIntColor());

        Globals.Engine.TVScreen2DImmediate.Draw_Line(-15 + Globals.Engine.ScreenWidth / 2
                                                            , 25 + Globals.Engine.ScreenHeight / 2
                                                            , 15 + Globals.Engine.ScreenWidth / 2
                                                            , 25 + Globals.Engine.ScreenHeight / 2
                                                            , pcolor.GetIntColor());

        if ((Globals.Engine.Player.PrimaryWeapon.Contains("photon") || Globals.Engine.Player.SecondaryWeapon.Contains("photon")) && p.GetStateB("EnablePhoton"))
        {
          p1_x = -15;
          p1_y = 28;
          p2_x = 15;
          p2_y = 33;

          float tremain = p.GetStateF("PhotonRemaining") / p.GetStateF("PhotonMax");


          Globals.Engine.TVScreen2DImmediate.Draw_FilledBox(p1_x + Globals.Engine.ScreenWidth / 2
                                                    , p1_y + Globals.Engine.ScreenHeight / 2
                                                    , p1_x + (p2_x - p1_x) * tremain + Globals.Engine.ScreenWidth / 2
                                                    , p2_y + Globals.Engine.ScreenHeight / 2
                                                    , new TV_COLOR(0.6f, 0.6f, 1, 1).GetIntColor());

          Globals.Engine.TVScreen2DImmediate.Draw_Box(p1_x + Globals.Engine.ScreenWidth / 2
                                                    , p1_y + Globals.Engine.ScreenHeight / 2
                                                    , p2_x + Globals.Engine.ScreenWidth / 2
                                                    , p2_y + Globals.Engine.ScreenHeight / 2
                                                    , new TV_COLOR(1, 0.5f, 0, 1).GetIntColor());
        }
      }

      if (Globals.Engine.Player.IsTorpedoMode & p.GetStateB("EnableTorp"))
      {
        p1_x = 19;
        p1_y = 17;
        p2_x = 23;
        p2_y = 25;
        p3_x = 6;
        p3_y = 10;
        float tremain = p.GetStateF("TorpRemaining");
        float tmax = p.GetStateF("TorpMax");
        int t = 0;

        while (t < tremain || t < tmax)
        {
          if (t < tremain)
          {
            Globals.Engine.TVScreen2DImmediate.Draw_FilledBox(p1_x + Globals.Engine.ScreenWidth / 2
                                                  , p1_y + Globals.Engine.ScreenHeight / 2
                                                  , p2_x + Globals.Engine.ScreenWidth / 2
                                                  , p2_y + Globals.Engine.ScreenHeight / 2
                                                  , pcolor.GetIntColor());
          }
          else
          {
            Globals.Engine.TVScreen2DImmediate.Draw_Box(p1_x + Globals.Engine.ScreenWidth / 2
                                  , p1_y + Globals.Engine.ScreenHeight / 2
                                  , p2_x + Globals.Engine.ScreenWidth / 2
                                  , p2_y + Globals.Engine.ScreenHeight / 2
                                  , pcolor.GetIntColor());
          }

          p1_x += p3_x;
          p2_x += p3_x;

          t++;
          if (t % 10 == 0)
          {
            p1_x = 19;
            p2_x = 23;
            p1_y += p3_y;
            p2_y += p3_y;
          }
        }
      }

      if (p.TypeInfo is FalconATI)
      {
        p1_x = 5;
        p1_y = 25;
        p2_x = -60;
        p2_y = -5;
        p3_x = 5;
        p3_y = 60;

        Globals.Engine.TVScreen2DImmediate.Draw_Line(0 + Globals.Engine.ScreenWidth / 2
                                                        , p1_x + Globals.Engine.ScreenHeight / 2
                                                        , 0 + Globals.Engine.ScreenWidth / 2
                                                        , p1_y + Globals.Engine.ScreenHeight / 2
                                                        , pcolor.GetIntColor());

        Globals.Engine.TVScreen2DImmediate.Draw_Line(p2_x + Globals.Engine.ScreenWidth / 2
                                                        , 0 + Globals.Engine.ScreenHeight / 2
                                                        , p2_y + Globals.Engine.ScreenWidth / 2
                                                        , 0 + Globals.Engine.ScreenHeight / 2
                                                        , pcolor.GetIntColor());

        Globals.Engine.TVScreen2DImmediate.Draw_Line(p3_x + Globals.Engine.ScreenWidth / 2
                                                        , 0 + Globals.Engine.ScreenHeight / 2
                                                        , p3_y + Globals.Engine.ScreenWidth / 2
                                                        , 0 + Globals.Engine.ScreenHeight / 2
                                                        , pcolor.GetIntColor());
      }
      Globals.Engine.TVScreen2DImmediate.Action_End2D();
      */
    }
  }
}
