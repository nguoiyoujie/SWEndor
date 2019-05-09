using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using System;
using System.Collections.Generic;

namespace SWEndor.UI.Widgets
{
  public class Radar : Widget
  {
    private TV_2DVECTOR radar_center;
    private float radar_radius;
    private float radar_range;
    private float radar_blinkfreq;
    private float radar_bigshiprenderunit;
    private TV_2DVECTOR targetingradar_center;
    private float targetingradar_radius;

    private TV_COLOR pcolor { get { return (PlayerInfo.Actor?.Faction == null) ? new TV_COLOR(1, 1, 1, 1) : PlayerInfo.Actor.Faction.Color; } }

    public Radar(Screen2D owner) : base(owner, "radar")
    {
      radar_center = new TV_2DVECTOR(Engine.ScreenWidth * 0.34f, Engine.ScreenHeight * 0.24f) + new TV_2DVECTOR(Engine.ScreenWidth / 2, Engine.ScreenHeight / 2);
      radar_radius = Engine.ScreenHeight * 0.16f; //100;
      radar_range = 4000;
      radar_blinkfreq = 2.5f;
      radar_bigshiprenderunit = 50;
      targetingradar_center = new TV_2DVECTOR(Engine.ScreenWidth / 2, Engine.ScreenHeight * 0.28f);
      targetingradar_radius = Engine.ScreenHeight * 0.12f; //100;
    }

  public override bool Visible
    {
      get
      {
        return (!Owner.ShowPage
            && PlayerInfo.Actor != null
            && PlayerInfo.Actor.ActorState != ActorState.DEAD
            && PlayerInfo.Actor.ActorState != ActorState.DYING
            && !(PlayerInfo.Actor.TypeInfo is InvisibleCameraATI)
            && Owner.ShowUI
            && Owner.ShowRadar);
      }
    }

    public override void Draw()
    {
      ActorInfo p = PlayerInfo.Actor;
      if (p == null || p.CreationState != CreationState.ACTIVE)
        return;

      if (Owner.OverrideTargetingRadar)
        DrawTargetingRadar();
      else
        DrawRadar();
    }

    // We should move this to a separate class
    private void DrawTargetingRadar()
    {
      float posX = targetingradar_center.x;
      float posY = targetingradar_center.y;
      float left = posX - targetingradar_radius;
      float right = posX + targetingradar_radius;
      float top = posY - targetingradar_radius;
      float bottom = posY + targetingradar_radius;
      float timefactor = Engine.Game.GameTime % 1;
      float divisor = 1.75f;
      while (timefactor + 1 / divisor < 1)
      {
        timefactor += 1 / divisor;
      }

      TVScreen2DImmediate.Action_Begin2D();
      TVScreen2DImmediate.Draw_FilledBox(left, top, right, bottom, new TV_COLOR(0, 0, 0, 0.25f).GetIntColor());
      TVScreen2DImmediate.Draw_Box(left, top, right, bottom, pcolor.GetIntColor());
      TVScreen2DImmediate.Draw_Box(posX - targetingradar_radius * 0.1f, posY - targetingradar_radius * 0.2f, posX + targetingradar_radius * 0.1f, posY + targetingradar_radius * 0.2f, pcolor.GetIntColor());
      TVScreen2DImmediate.Draw_Line(left, top, posX - targetingradar_radius * 0.1f, posY - targetingradar_radius * 0.2f, pcolor.GetIntColor());
      TVScreen2DImmediate.Draw_Line(left, bottom, posX - targetingradar_radius * 0.1f, posY + targetingradar_radius * 0.2f, pcolor.GetIntColor());
      TVScreen2DImmediate.Draw_Line(right, top, posX + targetingradar_radius * 0.1f, posY - targetingradar_radius * 0.2f, pcolor.GetIntColor());
      TVScreen2DImmediate.Draw_Line(right, bottom, posX + targetingradar_radius * 0.1f, posY + targetingradar_radius * 0.2f, pcolor.GetIntColor());

      while (timefactor / divisor > 0.1f)
      {
        float x = 0.1f + timefactor * 0.9f;
        float y = 0.2f + timefactor * 0.8f;
        TVScreen2DImmediate.Draw_Box(posX - targetingradar_radius * x, posY - targetingradar_radius * y, posX - targetingradar_radius * x, posY + targetingradar_radius * y, pcolor.GetIntColor());
        TVScreen2DImmediate.Draw_Box(posX + targetingradar_radius * x, posY - targetingradar_radius * y, posX + targetingradar_radius * x, posY + targetingradar_radius * y, pcolor.GetIntColor());
        TVScreen2DImmediate.Draw_Box(posX - targetingradar_radius * x, posY + targetingradar_radius * y, posX + targetingradar_radius * x, posY + targetingradar_radius * y, pcolor.GetIntColor());
        timefactor /= divisor;
      }
      TVScreen2DImmediate.Action_End2D();

      TVScreen2DText.Action_BeginText();
      float letter_size = 4.5f;
      TVScreen2DText.TextureFont_DrawText(Owner.TargetingRadar_text, posX - letter_size * 8, top + 5, pcolor.GetIntColor(), FontFactory.Get(Font.T12).ID);
      TVScreen2DText.Action_EndText();
    }

    private void DrawRadar()
    {
      TVScreen2DImmediate.Action_Begin2D();
      TVScreen2DImmediate.Draw_FilledCircle(radar_center.x, radar_center.y, radar_radius + 2, 300, new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
      TVScreen2DImmediate.Draw_Circle(radar_center.x, radar_center.y, radar_radius + 2, 300, pcolor.GetIntColor());
      TVScreen2DImmediate.Draw_Circle(radar_center.x, radar_center.y, radar_radius - 2, 300, pcolor.GetIntColor());

      ActorFactory.DoEach(DrawElement);

      TVScreen2DImmediate.Action_End2D();
    }

    private int GetColor(ActorInfo actor)
    {
      if (actor.TypeInfo is ActorTypes.Groups.Explosion)
        return new TV_COLOR(0.75f, 0.75f, 0, 0.75f).GetIntColor();

      else if (actor.TypeInfo is ActorTypes.Groups.Projectile)
        return new TV_COLOR(0.8f, 0.5f, 0, 0.6f).GetIntColor();

      else if (actor.Faction != null)
        return actor.Faction.Color.GetIntColor();

      else
        return new TV_COLOR(1, 1, 1, 1).GetIntColor();
    }

    public struct PolarCoord
    {
      public float Dist;
      public float Angle;

      public XYCoord ToXYCoord
      {
        get
        {
          return new XYCoord
          {
            X = -Dist * (float)Math.Sin(Angle * Globals.PI / 180),
            Y = Dist * (float)Math.Cos(Angle * Globals.PI / 180)
          };
        }
      }
    }

    public struct XYCoord
    {
      public float X;
      public float Y;

      public PolarCoord ToPolarCoord
      {
        get
        {
          return new PolarCoord
          {
            Angle = (float)Math.Atan2(X, Y) * 180 / Globals.PI + 180,
            Dist = (float)Math.Sqrt(X * X + Y * Y)
          };
        }
      }
    }


    private void DrawElement(Engine engine, int actorID)
    {
      ActorInfo p = PlayerInfo.Actor;
      ActorInfo a = ActorFactory.Get(actorID);
      if (a != null)
      {
        TV_3DVECTOR ppos = p.GetPosition();
        TV_3DVECTOR apos = a.GetPosition();

        if (a.CreationState == CreationState.ACTIVE
          && a.TypeInfo.RadarSize > 0
          && (a.TypeInfo.AlwaysShowInRadar || ActorDistanceInfo.GetRoughDistance(new TV_3DVECTOR(ppos.x, 0, ppos.z), new TV_3DVECTOR(apos.x, 0, apos.z)) < radar_range * 2))
        {
          int acolor = GetColor(a);
          
          float proty = p.GetRotation().y;

          XYCoord posvec = new XYCoord { X = ppos.x - apos.x, Y = ppos.z - apos.z };
          PolarCoord polar = posvec.ToPolarCoord;
          polar.Angle -= proty;
          if (polar.Dist > radar_range)
            polar.Dist = radar_range;

          XYCoord xy = polar.ToXYCoord;
          float x = radar_center.x - radar_radius * xy.X / radar_range;
          float y = radar_center.y - radar_radius * xy.Y / radar_range;
          
          /*
          TV_2DVECTOR posvec = new TV_2DVECTOR(ppos.x, ppos.z) - new TV_2DVECTOR(apos.x, apos.z);
          float proty = p.GetRotation().y;

          float dist = TrueVision.TVMathLibrary.GetDistanceVec2D(new TV_2DVECTOR(), posvec);
          float angl = TrueVision.TVMathLibrary.Direction2Ang(posvec.x, posvec.y) - proty;
          if (dist > radar_range)
            dist = radar_range;

          float x = radar_center.x - radar_radius * dist / radar_range * (float)Math.Sin(angl * Globals.PI / 180);
          float y = radar_center.y + radar_radius * dist / radar_range * (float)Math.Cos(angl * Globals.PI / 180);
          */
          switch (a.TypeInfo.RadarType)
          {
            case RadarType.TRAILLINE:
              TV_2DVECTOR prevtemp = new TV_2DVECTOR(ppos.x, ppos.z) - new TV_2DVECTOR(a.CoordData.PrevPosition.x, a.CoordData.PrevPosition.z);
              float prevdist = Engine.TrueVision.TVMathLibrary.GetDistanceVec2D(new TV_2DVECTOR(), prevtemp);
              float prevangl = Engine.TrueVision.TVMathLibrary.Direction2Ang(prevtemp.x, prevtemp.y) - proty;
              if (polar.Dist < radar_range && prevdist < radar_range)
              {
                float px = radar_center.x - radar_radius * prevdist / radar_range * (float)Math.Sin(prevangl * Globals.PI / 180);
                float py = radar_center.y + radar_radius * prevdist / radar_range * (float)Math.Cos(prevangl * Globals.PI / 180);

                DrawLine(x, y, px, py, acolor);
              }
              break;
            case RadarType.HOLLOW_SQUARE:
              DrawHollowSquare(x, y, a.TypeInfo.RadarSize, acolor);
              break;
            case RadarType.FILLED_SQUARE:
              DrawFilledSquare(x, y, a.TypeInfo.RadarSize, acolor);
              break;
            case RadarType.HOLLOW_CIRCLE_S:
              DrawHollowCircle(x, y, a.TypeInfo.RadarSize, 4, acolor);
              break;
            case RadarType.HOLLOW_CIRCLE_M:
              DrawHollowCircle(x, y, a.TypeInfo.RadarSize, 12, acolor);
              break;
            case RadarType.HOLLOW_CIRCLE_L:
              DrawHollowCircle(x, y, a.TypeInfo.RadarSize, 36, acolor);
              break;
            case RadarType.FILLED_CIRCLE_S:
              DrawFilledCircle(x, y, a.TypeInfo.RadarSize, 4, acolor);
              break;
            case RadarType.FILLED_CIRCLE_M:
              DrawFilledCircle(x, y, a.TypeInfo.RadarSize, 12, acolor);
              break;
            case RadarType.FILLED_CIRCLE_L:
              DrawFilledCircle(x, y, a.TypeInfo.RadarSize, 36, acolor);
              break;
            case RadarType.RECTANGLE_GIANT:
              {
                float gt = Engine.Game.GameTime * radar_blinkfreq;
                if (Engine.SysDataSet.StrengthFrac_get(actorID) > 0.1f || (gt - (int)gt > 0.5f))
                {
                  DrawRectGiant(a, posvec.X, posvec.Y, proty, acolor);
                }
              }
              break;
            case RadarType.TRIANGLE_GIANT:
              {
                float gt = Engine.Game.GameTime * radar_blinkfreq;
                if (Engine.SysDataSet.StrengthFrac_get(actorID) > 0.1f || (gt - (int)gt > 0.5f))
                {
                  DrawTriangleGiant(a, posvec.X, posvec.Y, proty, acolor);
                }
              }
              break;
          }
        }
      }
    }

    private void DrawLine(float x0, float y0, float x1, float y1, int color)
    {
      TVScreen2DImmediate.Draw_Line(x0, y0, x1, y1, color);
    }

    private void DrawHollowSquare(float x, float y, float size, int color)
    {
      TVScreen2DImmediate.Draw_Box(x - size, y - size, x + size, y + size, color);
    }

    private void DrawFilledSquare(float x, float y, float size, int color)
    {
      TVScreen2DImmediate.Draw_FilledBox(x - size, y - size, x + size, y + size, color);
    }

    private void DrawHollowCircle(float x, float y, float size, int points, int color)
    {
      TVScreen2DImmediate.Draw_Circle(x, y, size, points, color);
    }

    private void DrawFilledCircle(float x, float y, float size, int points, int color)
    {
      TVScreen2DImmediate.Draw_FilledCircle(x, y, size, points, color);
    }

    private void DrawRectGiant(ActorInfo a, float x, float y, float proty, int color)
    {
      TV_3DVECTOR boxmin = new TV_3DVECTOR();
      TV_3DVECTOR boxmax = new TV_3DVECTOR();
      a.GetBoundingBox(ref boxmin, ref boxmax, true);
      float scale = Engine.MeshDataSet.Scale_get(a.ID);
      boxmin *= scale;
      boxmax *= scale;

      List<TV_2DVECTOR?> ts = new List<TV_2DVECTOR?>();
      float bx = boxmin.x;
      float bz = boxmin.z;
      int i = 0;
      float ang = a.GetRotation().y;
      while (i < 4)
      {
        TV_2DVECTOR temp = new TV_2DVECTOR(x, y);
        temp -= new TV_2DVECTOR(bx * (float)Math.Cos(ang * Globals.PI / 180) + bz * (float)Math.Sin(ang * Globals.PI / 180),
                                bz * (float)Math.Cos(ang * Globals.PI / 180) - bx * (float)Math.Sin(ang * Globals.PI / 180));
        float tdist = Engine.TrueVision.TVMathLibrary.GetDistanceVec2D(new TV_2DVECTOR(), temp);
        float tangl = Engine.TrueVision.TVMathLibrary.Direction2Ang(temp.x, temp.y) - proty;
        if (tdist > radar_range)
        {
          tdist = radar_range;
          ts.Add(null);
        }
        float tx = radar_center.x - radar_radius * tdist / radar_range * (float)Math.Sin(tangl * Globals.PI / 180);
        float ty = radar_center.y + radar_radius * tdist / radar_range * (float)Math.Cos(tangl * Globals.PI / 180);
        if (ts.Count > 1 && ts[ts.Count - 1] != null && ts[ts.Count - 2] != null)
          ts[ts.Count - 1] = new TV_2DVECTOR(tx, ty);
        else
          ts.Add(new TV_2DVECTOR(tx, ty));

        switch (i)
        {
          case 3:
            bz -= radar_bigshiprenderunit;
            if (bz < boxmin.z)
            {
              bz = boxmin.z;
              i++;
              ts.Add(null);
            }
            break;
          case 2:
            bx -= radar_bigshiprenderunit;
            if (bx < boxmin.x)
            {
              bx = boxmin.x;
              i++;
              ts.Add(null);
            }
            break;
          case 1:
            bz += radar_bigshiprenderunit;
            if (bz > boxmax.z)
            {
              bz = boxmax.z;
              i++;
              ts.Add(null);
            }
            break;
          case 0:
            bx += radar_bigshiprenderunit;
            if (bx > boxmax.x)
            {
              bx = boxmax.x;
              i++;
              ts.Add(null);
            }
            break;
        }
      }

      for (int t = 0; t < ts.Count; t++)
      {
        int u = (t == ts.Count - 1) ? 0 : t + 1;
        if (ts[t] != null && ts[u] != null)
          DrawLine(((TV_2DVECTOR)ts[t]).x, ((TV_2DVECTOR)ts[t]).y, ((TV_2DVECTOR)ts[u]).x, ((TV_2DVECTOR)ts[u]).y, color);
      }
    }

    private void DrawTriangleGiant(ActorInfo a, float x, float y, float proty, int color)
    {
      TV_3DVECTOR boxmin = new TV_3DVECTOR();
      TV_3DVECTOR boxmax = new TV_3DVECTOR();
      a.GetBoundingBox(ref boxmin, ref boxmax, true);
      float scale = Engine.MeshDataSet.Scale_get(a.ID);
      boxmin *= scale;
      boxmax *= scale;

      List<TV_2DVECTOR?> ts = new List<TV_2DVECTOR?>();
      float bx = boxmin.x;
      float bz = boxmin.z;
      int i = 0;
      float ang = a.GetRotation().y;
      while (i < 3)
      {
        TV_2DVECTOR temp = new TV_2DVECTOR(x, y);
        temp -= new TV_2DVECTOR(bx * (float)Math.Cos(ang * Globals.PI / 180) + bz * (float)Math.Sin(ang * Globals.PI / 180),
                                bz * (float)Math.Cos(ang * Globals.PI / 180) - bx * (float)Math.Sin(ang * Globals.PI / 180));
        float tdist = Engine.TrueVision.TVMathLibrary.GetDistanceVec2D(new TV_2DVECTOR(), temp);
        float tangl = Engine.TrueVision.TVMathLibrary.Direction2Ang(temp.x, temp.y) - proty;
        if (tdist > radar_range)
        {
          tdist = radar_range;
          ts.Add(null);
        }
        float tx = radar_center.x - radar_radius * tdist / radar_range * (float)Math.Sin(tangl * Globals.PI / 180);
        float ty = radar_center.y + radar_radius * tdist / radar_range * (float)Math.Cos(tangl * Globals.PI / 180);
        if (ts.Count > 1 && ts[ts.Count - 1] != null && ts[ts.Count - 2] != null)
          ts[ts.Count - 1] = new TV_2DVECTOR(tx, ty);
        else
          ts.Add(new TV_2DVECTOR(tx, ty));

        switch (i)
        {
          case 2:
            bx -= radar_bigshiprenderunit;
            if (bx <= boxmin.x)
            {
              bx = boxmin.x;
              i++;
              ts.Add(null);
            }
            break;
          case 1:
            bz -= radar_bigshiprenderunit;
            bx += radar_bigshiprenderunit * (boxmax.x - boxmin.x) / (boxmax.z - boxmin.z) / 2;
            if (bz <= boxmin.z)
            {
              bz = boxmin.z;
              i++;
              ts.Add(null);
            }
            break;
          case 0:
            bz += radar_bigshiprenderunit;
            bx += radar_bigshiprenderunit * (boxmax.x - boxmin.x) / (boxmax.z - boxmin.z) / 2;
            if (bz >= boxmax.z)
            {
              bz = boxmax.z;
              i++;
              ts.Add(null);
            }
            break;

        }
      }

      for (int t = 0; t < ts.Count; t++)
      {
        int u = (t == ts.Count - 1) ? 0 : t + 1;
        if (ts[t] != null && ts[u] != null)
          DrawLine(((TV_2DVECTOR)ts[t]).x, ((TV_2DVECTOR)ts[t]).y, ((TV_2DVECTOR)ts[u]).x, ((TV_2DVECTOR)ts[u]).y, color);
      }
    }
  }
}
