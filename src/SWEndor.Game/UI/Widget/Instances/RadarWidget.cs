using MTV3D65;
using SWEndor.Game.Actors;
using SWEndor.Game.Actors.Models;
using SWEndor.Game.Core;
using SWEndor.Game.Explosions;
using SWEndor.Game.Models;
using Primrose.Primitives.Geometry;
using SWEndor.Game.Projectiles;
using Primrose.Primitives.ValueTypes;
using SWEndor.Game.Primitives.Geometry;
using SWEndor.Game.Primitives.Extensions;

namespace SWEndor.Game.UI.Widgets
{
  public class RadarWidget : Widget
  {
    private readonly TV_2DVECTOR radar_center;
    private readonly float radar_radius;
    private readonly float radar_range;
    private readonly float radar_blinkfreq;
    //private readonly float radar_bigshiprenderunit;

    private TV_3DVECTOR radar_origin;
    private TV_3DVECTOR radar_direction;

    private static COLOR explColor = new COLOR(0.75f, 0.75f, 0, 0.75f);
    private static COLOR projColor = new COLOR(0.8f, 0.5f, 0, 0.6f);
    private static COLOR whitColor = new COLOR(1, 1, 1, 1);

    public RadarWidget(Screen2D owner) : base(owner, "radar")
    {
      radar_center = new TV_2DVECTOR(Engine.ScreenWidth - Engine.ScreenHeight * 0.18f - 25, Engine.ScreenHeight * 0.82f - 60);
      radar_radius = Engine.ScreenHeight * 0.16f;
      radar_range = 4000;
      radar_blinkfreq = 2.5f;
      //radar_bigshiprenderunit = 50;
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
      if (p.IsSystemOperational(SystemPart.RADAR))
        active = true;
      {
        SystemState status = p.GetStatus(SystemPart.RADAR);
        if (status == SystemState.DAMAGED)
        {
          c = new COLOR(1, 0, 0, 1);
          showcircle = Engine.Game.GameTime % 2 > 1;
        }
        else if (status == SystemState.DISABLED)
          c = new COLOR(0.6f, 0.6f, 0.6f, 0.6f);
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
        radar_origin = p.GetGlobalPosition();
        radar_direction = p.GetGlobalDirection();

        Engine.ActorFactory.DoEach(DrawElement);
        Engine.ExplosionFactory.DoEach(DrawElement);
        Engine.ProjectileFactory.DoEach(DrawElement);
      }
      TVScreen2DImmediate.Action_End2D();
    }

    private COLOR GetColor(ActorInfo actor)
    {
      if (actor.TargetType.Intersects(TargetType.MUNITION | TargetType.MINE))
        return projColor;

      else if (actor.Faction != null)
        return actor.Faction.Color;

      else
        return whitColor;
    }

    private void GetLocationOnRadar(Engine engine, TV_3DVECTOR ppos, TV_3DVECTOR pup, TV_3DVECTOR pright, TV_3DVECTOR apos, bool snapToRadar, out float x, out float y, out float dist)
    {
      // Standard Top-Down

      //XYCoord posvec = new XYCoord { X = ppos.x - apos.x, Y = ppos.z - apos.z };
      //PolarCoord polar = posvec.ToPolarCoord;
      //polar.Angle -= proty;
      //if (polar.Dist > radar_range)
      //  polar.Dist = radar_range;
      //
      //XYCoord xy = polar.ToXYCoord;
      //x = radar_center.x - radar_radius * xy.X / radar_range;
      //y = radar_center.y - radar_radius * xy.Y / radar_range;
      //dist = polar.Dist

      TV_3DVECTOR diff = apos - ppos;
      TV_3DVECTOR pright_n = pright;
      TV_3DVECTOR pup_n = pup;
      engine.TrueVision.TVMathLibrary.TVVec3Normalize(ref pright_n, pright);
      engine.TrueVision.TVMathLibrary.TVVec3Normalize(ref pup_n, pup_n);
      XYCoord pxy =  new XYCoord { X = engine.TrueVision.TVMathLibrary.TVVec3Dot(diff, pright_n), Y = engine.TrueVision.TVMathLibrary.TVVec3Dot(diff, pup_n) };
      PolarCoord polar = pxy.ToPolarCoord;
      if (snapToRadar && polar.Dist > radar_range)
        polar.Dist = radar_range;

      XYCoord xy = polar.ToXYCoord;
      x = radar_center.x - radar_radius * xy.X / radar_range;
      y = radar_center.y - radar_radius * xy.Y / radar_range;
      dist = engine.TrueVision.TVMathLibrary.GetDistanceVec2D(new TV_2DVECTOR(0, 0), new TV_2DVECTOR(x, y));
    }

    private void DrawElement(Engine engine, ActorInfo a)
    {
      ActorInfo p = PlayerInfo.Actor;
      if (a != null)
      {
        TV_3DVECTOR ppos = p.GetGlobalPosition();//radar_origin;
        TV_3DVECTOR apos = a.GetGlobalPosition();

        if (a.Active
          && a.TypeInfo.RenderData.RadarSize > 0
          && (a.TypeInfo.RenderData.AlwaysShowInRadar || DistanceModel.GetRoughDistance(new TV_3DVECTOR(ppos.x, 0, ppos.z), new TV_3DVECTOR(apos.x, 0, apos.z)) < radar_range * 2))
        {
          int icolor = GetColor(a).Value;

          TV_3DVECTOR prot = p.GetGlobalRotation();
          TV_3DVECTOR arot = a.GetGlobalRotation();
          float proty = prot.y;
          TV_3DVECTOR pright = new TV_3DVECTOR();
          TV_3DVECTOR pup = new TV_3DVECTOR();
          engine.TrueVision.TVMathLibrary.TVVec3Rotate(ref pright, new TV_3DVECTOR(0, 0, -1), prot.y, prot.x, prot.z);
          engine.TrueVision.TVMathLibrary.TVVec3Rotate(ref pup, new TV_3DVECTOR(-1, 0, 0), prot.y, prot.x, prot.z);
          GetLocationOnRadar(engine, ppos, pright, pup, apos, true, out float x, out float y, out float dist);

          switch (a.TypeInfo.RenderData.RadarType)
          {
            case RadarType.TRAILLINE:
              GetLocationOnRadar(engine, ppos, pright, pup, a.GetPrevGlobalPosition(), true, out float px, out float py, out float prevdist);
              if (dist < radar_range && prevdist < radar_range)
              {
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
                  Box box = a.GetBoundingBox(true);
                  _f4[0] = new float3(box.X.Min, 0, box.Z.Min);
                  _f4[1] = new float3(box.X.Max, 0, box.Z.Min);
                  _f4[2] = new float3(box.X.Max, 0, box.Z.Max);
                  _f4[3] = new float3(box.X.Min, 0, box.Z.Max);
                  DrawPolygon(_f4, a.Scale, ppos, apos, prot, arot, icolor);
                }
              }
              break;
            case RadarType.TRIANGLE_GIANT:
              {
                float gt = Engine.Game.GameTime * radar_blinkfreq;
                if (a.HP_Frac > 0.1f || (gt - (int)gt > 0.5f))
                {
                  Box box = a.GetBoundingBox(true);
                  float xm = (box.X.Min + box.X.Max) / 2;
                  _f4[0] = new float3(box.X.Min, 0, box.Z.Min);
                  _f4[1] = new float3(box.X.Max, 0, box.Z.Min);
                  _f4[2] = new float3(xm, 0, box.Z.Max);
                  _f4[3] = new float3(xm, 0, box.Z.Max);
                  DrawPolygon(_f4, a.Scale, ppos, apos, prot, arot, icolor);
                }
              }
              break;
            case RadarType.POLYGON:
              {
                float gt = Engine.Game.GameTime * radar_blinkfreq;
                if (a.HP_Frac > 0.1f || (gt - (int)gt > 0.5f))
                {
                  DrawPolygon(a.TypeInfo.RenderData.RadarPolygonPoints, a.Scale, ppos, apos, prot, arot, icolor);
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

          TV_3DVECTOR prot = p.GetGlobalRotation();
          TV_3DVECTOR pright = new TV_3DVECTOR();
          TV_3DVECTOR pup = new TV_3DVECTOR();
          engine.TrueVision.TVMathLibrary.TVVec3Rotate(ref pright, new TV_3DVECTOR(0, 0, -1), prot.y, prot.x, prot.z);
          engine.TrueVision.TVMathLibrary.TVVec3Rotate(ref pup, new TV_3DVECTOR(-1, 0, 0), prot.y, prot.x, prot.z);
          GetLocationOnRadar(engine, ppos, pright, pup, apos, true, out float x, out float y, out float dist);
          
          switch (a.TypeInfo.RenderData.RadarType)
          {
            case RadarType.TRAILLINE:
              GetLocationOnRadar(engine, ppos, pright, pup, a.GetPrevGlobalPosition(), true, out float px, out float py, out float prevdist);
              if (dist < radar_range && prevdist < radar_range)
              {
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

    private float3[] _f4 = new float3[4];
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


          TV_3DVECTOR prot = p.GetGlobalRotation();
          TV_3DVECTOR arot = a.GetGlobalRotation();
          TV_3DVECTOR pright = new TV_3DVECTOR();
          TV_3DVECTOR pup = new TV_3DVECTOR();
          engine.TrueVision.TVMathLibrary.TVVec3Rotate(ref pright, new TV_3DVECTOR(0, 0, -1), prot.y, prot.x, prot.z);
          engine.TrueVision.TVMathLibrary.TVVec3Rotate(ref pup, new TV_3DVECTOR(-1, 0, 0), prot.y, prot.x, prot.z);
          GetLocationOnRadar(engine, ppos, pright, pup, apos, true, out float x, out float y, out float dist);

          switch (a.TypeInfo.RenderData.RadarType)
          {
            case RadarType.TRAILLINE:
              GetLocationOnRadar(engine, ppos, pright, pup, a.GetPrevGlobalPosition(), true, out float px, out float py, out float prevdist);
              if (dist < radar_range && prevdist < radar_range)
              {
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
                Box box = a.GetBoundingBox(true);
                _f4[0] = new float3(box.X.Min, 0, box.Z.Min);
                _f4[1] = new float3(box.X.Max, 0, box.Z.Min);
                _f4[2] = new float3(box.X.Max, 0, box.Z.Max);
                _f4[3] = new float3(box.X.Min, 0, box.Z.Max);
                DrawPolygon(_f4, a.Scale, ppos, apos, prot, arot, icolor);
              }
              break;
            case RadarType.TRIANGLE_GIANT:
              {
                Box box = a.GetBoundingBox(true);
                float xm = (box.X.Min + box.X.Max) / 2;
                _f4[0] = new float3(box.X.Min, 0, box.Z.Min);
                _f4[1] = new float3(box.X.Max, 0, box.Z.Min);
                _f4[2] = new float3(xm, 0, box.Z.Max);
                _f4[3] = new float3(xm, 0, box.Z.Max);
                DrawPolygon(_f4, a.Scale, ppos, apos, prot, arot, icolor);
              }
              break;
            case RadarType.POLYGON:
              DrawPolygon(a.TypeInfo.RenderData.RadarPolygonPoints, a.Scale, ppos, apos, prot, arot, icolor);
              break;
          }
        }
      }
    }

    private void DrawLine(float x0, float y0, float x1, float y1, int color)
    {
      TVScreen2DImmediate.Draw_Line(x0, y0, x1, y1, color);
    }

    private void DrawLineWithinCircle(float x0, float y0, float x1, float y1, int color)
    {
      float2 start = new float2(x0, y0);
      float2 end = new float2(x1, y1);
      if (start == end) { return; }
      Circle circle = new Circle(radar_center.ToFloat2(), radar_radius);
      if (!circle.Contains(start) && !circle.Contains(end))
      {
        return;
      }
      else if (circle.Contains(start) && circle.Contains(end))
      {
        TVScreen2DImmediate.Draw_Line(x0, y0, x1, y1, color);
      }
      else
      {
        float2 pt1 = start;
        if (!circle.Contains(start))
        {
          circle.IntersectRayCircle(start, end, out pt1);
          start = pt1;
        }
        if (!circle.Contains(end))
        {
          circle.IntersectRayCircle(end, start, out pt1);
          end = pt1;
        }
        TVScreen2DImmediate.Draw_Line(start.x, start.y, end.x, end.y, color);
      }
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

    private TV_3DVECTOR GetPointOnRadar(float3 p, float3 scale, TV_3DVECTOR ppos, TV_3DVECTOR apos, TV_3DVECTOR prot, TV_3DVECTOR arot, bool snapToRadar, out float x, out float y, out float dist)
    {
      TV_3DVECTOR point = new TV_3DVECTOR();
      TV_3DVECTOR pright = new TV_3DVECTOR();
      TV_3DVECTOR pup = new TV_3DVECTOR();
      Engine.TrueVision.TVMathLibrary.TVVec3Rotate(ref pright, new TV_3DVECTOR(0, 0, -1), prot.y, prot.x, prot.z);
      Engine.TrueVision.TVMathLibrary.TVVec3Rotate(ref pup, new TV_3DVECTOR(-1, 0, 0), prot.y, prot.x, prot.z);
      Engine.TrueVision.TVMathLibrary.TVVec3Rotate(ref point, (p * scale).ToVec3(), arot.y, arot.x, arot.z);
      point += apos;
      GetLocationOnRadar(Engine, ppos, pright, pup, point, snapToRadar, out x, out y, out dist);
      return point;
    }

    private void DrawPolygon(float3[] points, float3 scale, TV_3DVECTOR ppos, TV_3DVECTOR apos, TV_3DVECTOR prot, TV_3DVECTOR arot, int color)
    {
      if (points == null || points.Length < 2) return;

      GetPointOnRadar(points[0], scale, ppos, apos, prot, arot, false, out float x0, out float y0, out _);
      float prevx = x0;
      float prevy = y0;

      for (int i = 1; i < points.Length; i++)
      {
        GetPointOnRadar(points[i], scale, ppos, apos, prot, arot, false, out float xn, out float yn, out _);
        DrawLineWithinCircle(prevx, prevy, xn, yn, color);
        if (i == points.Length - 1)
        {
          DrawLineWithinCircle(xn, yn, x0, y0, color);
        }
        prevx = xn;
        prevy = yn;
      }
    }
  }
}
