using MTV3D65;
using SWEndor.Game.Actors;
using SWEndor.Game.Core;
using SWEndor.Game.Models;
using Primrose.Primitives.Geometry;
using Primrose.Primitives.ValueTypes;

namespace SWEndor.Game.UI.Widgets
{
  public class LargeShipSystemsWidget : Widget
  {
    private readonly TV_2DVECTOR radar_center;
    private readonly float radar_radius;
    private readonly float radar_range;

    public LargeShipSystemsWidget(Screen2D owner) : base(owner, "largeshipsystems")
    {
      radar_center = owner.ScreenCenter + new TV_2DVECTOR(-Engine.ScreenWidth * 0.42f, Engine.ScreenHeight * 0.3f);
      radar_radius = Engine.ScreenHeight * 0.25f;
      radar_range = 1500;
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
      if (p == null
        || !p.Active
        || !p.TargetType.Has(TargetType.SHIP)
        )
        return;

      DrawRadar();
    }

    private void DrawRadar()
    {
      TVScreen2DImmediate.Action_Begin2D();

      float zoom = DrawElement(Engine, PlayerInfo.Actor, 1);
      foreach (ActorInfo a in PlayerInfo.Actor.Children)
      {
        DrawElement(Engine, a, zoom);
      }

      TVScreen2DImmediate.Action_End2D();
    }

    private float DrawElement(Engine engine, ActorInfo a, float zoom)
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
          TV_2DVECTOR posvec = new TV_2DVECTOR(ppos.x, ppos.z) - new TV_2DVECTOR(apos.x, apos.z);
          float proty = p.GetGlobalRotation().y;
          float dist = engine.TrueVision.TVMathLibrary.GetDistanceVec2D(new TV_2DVECTOR(), posvec);
          float angl = engine.TrueVision.TVMathLibrary.Direction2Ang(posvec.x, posvec.y) - proty;
          //if (dist > radar_range)
          //  dist = radar_range;

          float x = radar_center.x - dist * LookUp.Sin(angl, LookUp.Measure.DEGREES) * zoom;
          float y = radar_center.y + dist * LookUp.Cos(angl, LookUp.Measure.DEGREES) * zoom;
          float3 scale = a.Scale;
          int icolor = a.HP_Color.Value;

          switch (a.TypeInfo.RenderData.RadarType)
          {
            case RadarType.HOLLOW_CIRCLE_S:
              TVScreen2DImmediate.Draw_Circle(x, y, a.TypeInfo.RenderData.RadarSize * 2, 4, icolor);
              break;
            case RadarType.HOLLOW_CIRCLE_M:
              TVScreen2DImmediate.Draw_Circle(x, y, a.TypeInfo.RenderData.RadarSize * 2, 12, icolor);
              break;
            case RadarType.HOLLOW_CIRCLE_L:
              TVScreen2DImmediate.Draw_Circle(x, y, a.TypeInfo.RenderData.RadarSize * 2, 36, icolor);
              break;
            case RadarType.RECTANGLE_GIANT:
              {
                Box box = a.GetBoundingBox(true);
                zoom *= radar_radius / ((box.Z.Max - box.Z.Min) * scale.z);

                TVScreen2DImmediate.Draw_Box(box.X.Min * scale.x * zoom + radar_center.x
                                           , box.Z.Min * scale.z * zoom + radar_center.y
                                           , box.X.Max * scale.x * zoom + radar_center.x
                                           , box.Z.Max * scale.z * zoom + radar_center.y
                                           , icolor);
                break;
              }
            case RadarType.TRIANGLE_GIANT:
              {
                Box box = a.GetBoundingBox(true);
                zoom *= radar_radius / ((box.Z.Max - box.Z.Min) * scale.z);

                TVScreen2DImmediate.Draw_Triangle(box.X.Min * scale.x * zoom + radar_center.x
                                                , -box.Z.Min * scale.z * zoom + radar_center.y
                                                , (box.X.Min + box.X.Max) / 2 * scale.x * zoom + radar_center.x
                                                , -box.Z.Max * scale.z * zoom + radar_center.y
                                                , box.X.Max * scale.x * zoom + radar_center.x
                                                , -box.Z.Min * scale.z * zoom + radar_center.y
                                                , icolor);

                break;
              }
            case RadarType.POLYGON:
              {
                Box box = a.GetBoundingBox(true);
                zoom *= radar_radius / ((box.Z.Max - box.Z.Min) * scale.z);
                DrawPolygon(a.TypeInfo.RenderData.RadarPolygonPoints, scale * zoom, a.GetGlobalRotation().y, x, y, proty, icolor);
                break;
              }
          }
        }
      }
      return zoom;
    }

    private void DrawPolygon(float3[] points, float3 scale, float rot_y, float x, float y, float proty, int color)
    {
      if (points == null || points.Length < 2) return;

      float bx = points[0].x * scale.x;
      float bz = points[0].z * scale.z;
      float ang = rot_y - proty;
      float cos = LookUp.Cos(ang, LookUp.Measure.DEGREES);
      float sin = -LookUp.Sin(ang, LookUp.Measure.DEGREES);
      TV_2DVECTOR pt0 = new TV_2DVECTOR(x, y) - new TV_2DVECTOR(bx * cos + bz * sin,
                                                                bz * cos - bx * sin);

      TV_2DVECTOR prevpt = pt0;

      for (int i = 1; i < points.Length; i++)
      {
        bx = points[i].x * scale.x;
        bz = points[i].z * scale.z;
        TV_2DVECTOR pt = new TV_2DVECTOR(x, y) - new TV_2DVECTOR(bx * cos + bz * sin,
                                                                 bz * cos - bx * sin);

        if (i == points.Length - 1)
        {
          DrawLine(pt.x, pt.y, pt0.x, pt0.y, color);
        }
        DrawLine(prevpt.x, prevpt.y, pt.x, pt.y, color);
        prevpt = pt;
      }
    }

    private void DrawLine(float x0, float y0, float x1, float y1, int color)
    {
      TVScreen2DImmediate.Draw_Line(x0, y0, x1, y1, color);
    }
  }
}
