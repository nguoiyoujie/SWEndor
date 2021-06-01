using MTV3D65;
using SWEndor.Game.Actors;
using SWEndor.Game.Actors.Models;
using SWEndor.Game.Core;
using SWEndor.Game.Explosions;
using SWEndor.Game.Models;
using Primrose.Primitives.Geometry;
using SWEndor.Game.Projectiles;
using System;
using System.Collections.Generic;

namespace SWEndor.Game.UI.Widgets
{
  public class RadarWidget : Widget
  {
    private readonly TV_2DVECTOR radar_center;
    private readonly float radar_radius;
    private readonly float radar_range;
    private readonly float radar_blinkfreq;
    private readonly float radar_bigshiprenderunit;

    public RadarWidget(Screen2D owner) : base(owner, "radar")
    {
      radar_center = new TV_2DVECTOR(Engine.ScreenWidth - Engine.ScreenHeight * 0.18f - 25, Engine.ScreenHeight * 0.82f - 60);
      radar_radius = Engine.ScreenHeight * 0.16f;
      radar_range = 4000;
      radar_blinkfreq = 2.5f;
      radar_bigshiprenderunit = 50;
    }

  public override bool Visible
    {
      get
      {
        ActorInfo p = PlayerInfo.Actor;
        return (!Owner.ShowPage
          && p != null
          && !p.IsDyingOrDead
          && Owner.ShowUI
          && Owner.ShowRadar
          && !Owner.OverrideTargetingRadar);
      }
    }

    public override void Draw()
    {
      ActorInfo p = PlayerInfo.Actor;
      if (p == null || !p.Active)
        return;

      COLOR c = pcolor;
      bool active = false;
      bool showcircle = true;
      if (!p.TypeInfo.SystemData.AllowSystemDamage || p.GetStatus(SystemPart.RADAR) == SystemState.ACTIVE)
        active = true;
      else if (p.GetStatus(SystemPart.RADAR) == SystemState.DISABLED)
        c = new COLOR(0.6f, 0.6f, 0.6f, 0.6f);
      else if (p.GetStatus(SystemPart.RADAR) == SystemState.DESTROYED)
      {
        c = new COLOR(1, 0, 0, 1);
        showcircle = Engine.Game.GameTime % 2 > 1;
      }
      TVScreen2DImmediate.Action_Begin2D();
      TVScreen2DImmediate.Draw_FilledCircle(radar_center.x, radar_center.y, radar_radius + 2, 300, new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
      if (showcircle)
      {
        TVScreen2DImmediate.Draw_Circle(radar_center.x, radar_center.y, radar_radius + 2, 300, c.Value);
        TVScreen2DImmediate.Draw_Circle(radar_center.x, radar_center.y, radar_radius - 2, 300, c.Value);
      }
      if (active)
      {
        Engine.ActorFactory.DoEach(DrawElement);
        Engine.ExplosionFactory.DoEach(DrawElement);
        Engine.ProjectileFactory.DoEach(DrawElement);
      }
      TVScreen2DImmediate.Action_End2D();
    }

    private static COLOR explColor = new COLOR(0.75f, 0.75f, 0, 0.75f);
    private static COLOR projColor = new COLOR(0.8f, 0.5f, 0, 0.6f);
    private static COLOR whitColor = new COLOR(1, 1, 1, 1);

    private COLOR GetColor(ActorInfo actor)
    {
      if (actor.TypeInfo.AIData.TargetType.Intersects(TargetType.MUNITION))
        return projColor;

      else if (actor.Faction != null)
        return actor.Faction.Color;

      else
        return whitColor;
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
            X = -Dist * LookUp.Sin(Angle, LookUp.Measure.DEGREES),
            Y = Dist * LookUp.Cos(Angle, LookUp.Measure.DEGREES)
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
            Angle = (float)Math.Atan2(X, Y) * Globals.Rad2Deg + 180,
            Dist = (float)Math.Sqrt(X * X + Y * Y)
          };
        }
      }
    }

    private void DrawElement(Engine engine, ActorInfo a)
    {
      ActorInfo p = PlayerInfo.Actor;
      if (a != null)
      {
        TV_3DVECTOR ppos = p.GetGlobalPosition();
        TV_3DVECTOR apos = a.GetGlobalPosition();

        if (a.Active
          && a.TypeInfo.RenderData.RadarSize > 0
          && (a.TypeInfo.RenderData.AlwaysShowInRadar || DistanceModel.GetRoughDistance(new TV_3DVECTOR(ppos.x, 0, ppos.z), new TV_3DVECTOR(apos.x, 0, apos.z)) < radar_range * 2))
        {
          int icolor = GetColor(a).Value;
          
          float proty = p.GetGlobalRotation().y;

          XYCoord posvec = new XYCoord { X = ppos.x - apos.x, Y = ppos.z - apos.z };
          PolarCoord polar = posvec.ToPolarCoord;
          polar.Angle -= proty;
          if (polar.Dist > radar_range)
            polar.Dist = radar_range;

          XYCoord xy = polar.ToXYCoord;
          float x = radar_center.x - radar_radius * xy.X / radar_range;
          float y = radar_center.y - radar_radius * xy.Y / radar_range;

          switch (a.TypeInfo.RenderData.RadarType)
          {
            case RadarType.TRAILLINE:
              TV_2DVECTOR prevtemp = new TV_2DVECTOR(ppos.x, ppos.z) - new TV_2DVECTOR(a.PrevPosition.x, a.PrevPosition.z);
              float prevdist = Engine.TrueVision.TVMathLibrary.GetDistanceVec2D(new TV_2DVECTOR(), prevtemp);
              float prevangl = Engine.TrueVision.TVMathLibrary.Direction2Ang(prevtemp.x, prevtemp.y) - proty;
              if (polar.Dist < radar_range && prevdist < radar_range)
              {
                float px = radar_center.x - radar_radius * prevdist / radar_range * LookUp.Sin(prevangl, LookUp.Measure.DEGREES);
                float py = radar_center.y + radar_radius * prevdist / radar_range * LookUp.Cos(prevangl, LookUp.Measure.DEGREES);

                DrawLine(x, y, px, py, icolor);
              }
              break;
            case RadarType.HOLLOW_SQUARE:
              DrawHollowSquare(x, y, a.TypeInfo.RenderData.RadarSize, icolor);
              break;
            case RadarType.FILLED_SQUARE:
              DrawFilledSquare(x, y, a.TypeInfo.RenderData.RadarSize, icolor);
              break;
            case RadarType.HOLLOW_CIRCLE_S:
              DrawHollowCircle(x, y, a.TypeInfo.RenderData.RadarSize, 4, icolor);
              break;
            case RadarType.HOLLOW_CIRCLE_M:
              DrawHollowCircle(x, y, a.TypeInfo.RenderData.RadarSize, 12, icolor);
              break;
            case RadarType.HOLLOW_CIRCLE_L:
              DrawHollowCircle(x, y, a.TypeInfo.RenderData.RadarSize, 36, icolor);
              break;
            case RadarType.FILLED_CIRCLE_S:
              DrawFilledCircle(x, y, a.TypeInfo.RenderData.RadarSize, 4, icolor);
              break;
            case RadarType.FILLED_CIRCLE_M:
              DrawFilledCircle(x, y, a.TypeInfo.RenderData.RadarSize, 12, icolor);
              break;
            case RadarType.FILLED_CIRCLE_L:
              DrawFilledCircle(x, y, a.TypeInfo.RenderData.RadarSize, 36, icolor);
              break;
            case RadarType.RECTANGLE_GIANT:
              {
                float gt = Engine.Game.GameTime * radar_blinkfreq;
                if (a.HP_Frac > 0.1f || (gt - (int)gt > 0.5f))
                {
                  DrawRectGiant(a.GetBoundingBox(true), a.Scale, a.GetGlobalRotation().y, posvec.X, posvec.Y, proty, icolor);
                }
              }
              break;
            case RadarType.TRIANGLE_GIANT:
              {
                float gt = Engine.Game.GameTime * radar_blinkfreq;
                if (a.HP_Frac > 0.1f || (gt - (int)gt > 0.5f))
                {
                  DrawTriangleGiant(a.GetBoundingBox(true), a.Scale, a.GetGlobalRotation().y, posvec.X, posvec.Y, proty, icolor);
                }
              }
              break;
          }
        }
      }
    }

    private void DrawElement(Engine engine, ExplosionInfo a)
    {
      ActorInfo p = PlayerInfo.Actor;
      if (a != null)
      {
        TV_3DVECTOR ppos = p.GetGlobalPosition();
        TV_3DVECTOR apos = a.GetGlobalPosition();

        if (a.Active
          && a.TypeInfo.RenderData.RadarSize > 0
          && (a.TypeInfo.RenderData.AlwaysShowInRadar || DistanceModel.GetRoughDistance(new TV_3DVECTOR(ppos.x, 0, ppos.z), new TV_3DVECTOR(apos.x, 0, apos.z)) < radar_range * 2))
        {
          int icolor = explColor.Value;

          float proty = p.GetGlobalRotation().y;

          XYCoord posvec = new XYCoord { X = ppos.x - apos.x, Y = ppos.z - apos.z };
          PolarCoord polar = posvec.ToPolarCoord;
          polar.Angle -= proty;
          if (polar.Dist > radar_range)
            polar.Dist = radar_range;

          XYCoord xy = polar.ToXYCoord;
          float x = radar_center.x - radar_radius * xy.X / radar_range;
          float y = radar_center.y - radar_radius * xy.Y / radar_range;

          switch (a.TypeInfo.RenderData.RadarType)
          {
            case RadarType.TRAILLINE:
              TV_2DVECTOR prevtemp = new TV_2DVECTOR(ppos.x, ppos.z) - new TV_2DVECTOR(a.PrevPosition.x, a.PrevPosition.z);
              float prevdist = Engine.TrueVision.TVMathLibrary.GetDistanceVec2D(new TV_2DVECTOR(), prevtemp);
              float prevangl = Engine.TrueVision.TVMathLibrary.Direction2Ang(prevtemp.x, prevtemp.y) - proty;
              if (polar.Dist < radar_range && prevdist < radar_range)
              {
                float px = radar_center.x - radar_radius * prevdist / radar_range * LookUp.Sin(prevangl, LookUp.Measure.DEGREES);
                float py = radar_center.y + radar_radius * prevdist / radar_range * LookUp.Cos(prevangl, LookUp.Measure.DEGREES);

                DrawLine(x, y, px, py, icolor);
              }
              break;
            case RadarType.HOLLOW_SQUARE:
              DrawHollowSquare(x, y, a.TypeInfo.RenderData.RadarSize, icolor);
              break;
            case RadarType.FILLED_SQUARE:
              DrawFilledSquare(x, y, a.TypeInfo.RenderData.RadarSize, icolor);
              break;
            case RadarType.HOLLOW_CIRCLE_S:
              DrawHollowCircle(x, y, a.TypeInfo.RenderData.RadarSize, 4, icolor);
              break;
            case RadarType.HOLLOW_CIRCLE_M:
              DrawHollowCircle(x, y, a.TypeInfo.RenderData.RadarSize, 12, icolor);
              break;
            case RadarType.HOLLOW_CIRCLE_L:
              DrawHollowCircle(x, y, a.TypeInfo.RenderData.RadarSize, 36, icolor);
              break;
            case RadarType.FILLED_CIRCLE_S:
              DrawFilledCircle(x, y, a.TypeInfo.RenderData.RadarSize, 4, icolor);
              break;
            case RadarType.FILLED_CIRCLE_M:
              DrawFilledCircle(x, y, a.TypeInfo.RenderData.RadarSize, 12, icolor);
              break;
            case RadarType.FILLED_CIRCLE_L:
              DrawFilledCircle(x, y, a.TypeInfo.RenderData.RadarSize, 36, icolor);
              break;
          }
        }
      }
    }

    private void DrawElement(Engine engine, ProjectileInfo a)
    {
      ActorInfo p = PlayerInfo.Actor;
      if (a != null)
      {
        TV_3DVECTOR ppos = p.GetGlobalPosition();
        TV_3DVECTOR apos = a.GetGlobalPosition();

        if (a.Active
          && a.TypeInfo.RenderData.RadarSize > 0
          && (a.TypeInfo.RenderData.AlwaysShowInRadar || DistanceModel.GetRoughDistance(new TV_3DVECTOR(ppos.x, 0, ppos.z), new TV_3DVECTOR(apos.x, 0, apos.z)) < radar_range * 2))
        {
          int icolor = projColor.Value;

          float proty = p.GetGlobalRotation().y;

          XYCoord posvec = new XYCoord { X = ppos.x - apos.x, Y = ppos.z - apos.z };
          PolarCoord polar = posvec.ToPolarCoord;
          polar.Angle -= proty;
          if (polar.Dist > radar_range)
            polar.Dist = radar_range;

          XYCoord xy = polar.ToXYCoord;
          float x = radar_center.x - radar_radius * xy.X / radar_range;
          float y = radar_center.y - radar_radius * xy.Y / radar_range;

          switch (a.TypeInfo.RenderData.RadarType)
          {
            case RadarType.TRAILLINE:
              TV_2DVECTOR prevtemp = new TV_2DVECTOR(ppos.x, ppos.z) - new TV_2DVECTOR(a.PrevPosition.x, a.PrevPosition.z);
              float prevdist = Engine.TrueVision.TVMathLibrary.GetDistanceVec2D(new TV_2DVECTOR(), prevtemp);
              float prevangl = Engine.TrueVision.TVMathLibrary.Direction2Ang(prevtemp.x, prevtemp.y) - proty;
              if (polar.Dist < radar_range && prevdist < radar_range)
              {
                float px = radar_center.x - radar_radius * prevdist / radar_range * LookUp.Sin(prevangl, LookUp.Measure.DEGREES);
                float py = radar_center.y + radar_radius * prevdist / radar_range * LookUp.Cos(prevangl, LookUp.Measure.DEGREES);

                DrawLine(x, y, px, py, icolor);
              }
              break;
            case RadarType.HOLLOW_SQUARE:
              DrawHollowSquare(x, y, a.TypeInfo.RenderData.RadarSize, icolor);
              break;
            case RadarType.FILLED_SQUARE:
              DrawFilledSquare(x, y, a.TypeInfo.RenderData.RadarSize, icolor);
              break;
            case RadarType.HOLLOW_CIRCLE_S:
              DrawHollowCircle(x, y, a.TypeInfo.RenderData.RadarSize, 4, icolor);
              break;
            case RadarType.HOLLOW_CIRCLE_M:
              DrawHollowCircle(x, y, a.TypeInfo.RenderData.RadarSize, 12, icolor);
              break;
            case RadarType.HOLLOW_CIRCLE_L:
              DrawHollowCircle(x, y, a.TypeInfo.RenderData.RadarSize, 36, icolor);
              break;
            case RadarType.FILLED_CIRCLE_S:
              DrawFilledCircle(x, y, a.TypeInfo.RenderData.RadarSize, 4, icolor);
              break;
            case RadarType.FILLED_CIRCLE_M:
              DrawFilledCircle(x, y, a.TypeInfo.RenderData.RadarSize, 12, icolor);
              break;
            case RadarType.FILLED_CIRCLE_L:
              DrawFilledCircle(x, y, a.TypeInfo.RenderData.RadarSize, 36, icolor);
              break;
            case RadarType.RECTANGLE_GIANT:
              DrawRectGiant(a.GetBoundingBox(true), a.Scale, a.GetGlobalRotation().y, posvec.X, posvec.Y, proty, icolor);
              break;
            case RadarType.TRIANGLE_GIANT:
              DrawTriangleGiant(a.GetBoundingBox(true), a.Scale, a.GetGlobalRotation().y, posvec.X, posvec.Y, proty, icolor);
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

    private void DrawRectGiant(Box box, float scale, float rot_y, float x, float y, float proty, int color)
    {
      List<TV_2DVECTOR?> ts = new List<TV_2DVECTOR?>();
      float bx = box.X.Min * scale;
      float bz = box.Z.Min * scale;
      int i = 0;
      float ang = rot_y;
      while (i < 4)
      {
        TV_2DVECTOR temp = new TV_2DVECTOR(x, y);
        temp -= new TV_2DVECTOR(bx * LookUp.Cos(ang, LookUp.Measure.DEGREES) + bz * LookUp.Sin(ang, LookUp.Measure.DEGREES),
                                bz * LookUp.Cos(ang, LookUp.Measure.DEGREES) - bx * LookUp.Sin(ang, LookUp.Measure.DEGREES));
        float tdist = Engine.TrueVision.TVMathLibrary.GetDistanceVec2D(new TV_2DVECTOR(), temp);
        float tangl = Engine.TrueVision.TVMathLibrary.Direction2Ang(temp.x, temp.y) - proty;
        if (tdist > radar_range)
        {
          tdist = radar_range;
          ts.Add(null);
        }
        float tx = radar_center.x - radar_radius * tdist / radar_range * LookUp.Sin(tangl, LookUp.Measure.DEGREES);
        float ty = radar_center.y + radar_radius * tdist / radar_range * LookUp.Cos(tangl, LookUp.Measure.DEGREES);
        if (ts.Count > 1 && ts[ts.Count - 1] != null && ts[ts.Count - 2] != null)
          ts[ts.Count - 1] = new TV_2DVECTOR(tx, ty);
        else
          ts.Add(new TV_2DVECTOR(tx, ty));

        switch (i)
        {
          case 3:
            bz -= radar_bigshiprenderunit;
            if (bz < box.Z.Min * scale)
            {
              bz = box.Z.Min * scale;
              i++;
              ts.Add(null);
            }
            break;
          case 2:
            bx -= radar_bigshiprenderunit;
            if (bx < box.X.Min * scale)
            {
              bx = box.X.Min * scale;
              i++;
              ts.Add(null);
            }
            break;
          case 1:
            bz += radar_bigshiprenderunit;
            if (bz > box.Z.Max * scale)
            {
              bz = box.Z.Max * scale;
              i++;
              ts.Add(null);
            }
            break;
          case 0:
            bx += radar_bigshiprenderunit;
            if (bx > box.X.Max * scale)
            {
              bx = box.X.Max * scale;
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

    private void DrawTriangleGiant(Box box, float scale, float rot_y, float x, float y, float proty, int color)
    {
      List<TV_2DVECTOR?> ts = new List<TV_2DVECTOR?>();
      float bx = box.X.Min * scale;
      float bz = box.Z.Min * scale;
      int i = 0;
      float ang = rot_y;
      while (i < 3)
      {
        TV_2DVECTOR temp = new TV_2DVECTOR(x, y);
        temp -= new TV_2DVECTOR(bx * LookUp.Cos(ang, LookUp.Measure.DEGREES) + bz * LookUp.Sin(ang, LookUp.Measure.DEGREES),
                                bz * LookUp.Cos(ang, LookUp.Measure.DEGREES) - bx * LookUp.Sin(ang, LookUp.Measure.DEGREES));
        float tdist = Engine.TrueVision.TVMathLibrary.GetDistanceVec2D(new TV_2DVECTOR(), temp);
        float tangl = Engine.TrueVision.TVMathLibrary.Direction2Ang(temp.x, temp.y) - proty;
        if (tdist > radar_range)
        {
          tdist = radar_range;
          ts.Add(null);
        }
        float tx = radar_center.x - radar_radius * tdist / radar_range * LookUp.Sin(tangl, LookUp.Measure.DEGREES);
        float ty = radar_center.y + radar_radius * tdist / radar_range * LookUp.Cos(tangl, LookUp.Measure.DEGREES);
        if (ts.Count > 1 && ts[ts.Count - 1] != null && ts[ts.Count - 2] != null)
          ts[ts.Count - 1] = new TV_2DVECTOR(tx, ty);
        else
          ts.Add(new TV_2DVECTOR(tx, ty));

        switch (i)
        {
          case 2:
            bx -= radar_bigshiprenderunit;
            if (bx <= box.X.Min * scale)
            {
              bx = box.X.Min * scale;
              i++;
              ts.Add(null);
            }
            break;
          case 1:
            bz -= radar_bigshiprenderunit;
            bx += radar_bigshiprenderunit * (box.X.Max * scale - box.X.Min * scale) / (box.Z.Max * scale - box.Z.Min * scale) / 2;
            if (bz <= box.Z.Min * scale)
            {
              bz = box.Z.Min * scale;
              i++;
              ts.Add(null);
            }
            break;
          case 0:
            bz += radar_bigshiprenderunit;
            bx += radar_bigshiprenderunit * (box.X.Max * scale - box.X.Min * scale) / (box.Z.Max * scale - box.Z.Min * scale) / 2;
            if (bz >= box.Z.Max * scale)
            {
              bz = box.Z.Max * scale;
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
