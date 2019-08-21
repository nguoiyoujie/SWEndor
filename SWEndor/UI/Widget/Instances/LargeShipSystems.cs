using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes;
using SWEndor.Primitives;
using System;

namespace SWEndor.UI.Widgets
{
  public class LargeShipSystems : Widget
  {
    private TV_2DVECTOR radar_center;
    private float radar_radius;
    private float radar_range;

    public LargeShipSystems(Screen2D owner) : base(owner, "largeshipsystems")
    {
      radar_center = new TV_2DVECTOR(-Engine.ScreenWidth * 0.42f, Engine.ScreenHeight * 0.3f) + new TV_2DVECTOR(Engine.ScreenWidth / 2, Engine.ScreenHeight / 2);
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
        || !(p.TypeInfo is ActorTypes.Groups.StarDestroyer
          || p.TypeInfo is ActorTypes.Groups.Warship
        ))
        return;

      DrawRadar();
    }

    private void DrawRadar()
    {
      TVScreen2DImmediate.Action_Begin2D();

      DrawElement(Engine, PlayerInfo.Actor);
      foreach (ActorInfo a in PlayerInfo.Actor.Children)
        DrawElement(Engine, a);

      TVScreen2DImmediate.Action_End2D();
    }

    private void DrawElement(Engine engine, ActorInfo a)
    {
      ActorInfo p = PlayerInfo.Actor;
      if (a != null)
      {
        TV_3DVECTOR ppos = p.GetGlobalPosition();
        TV_3DVECTOR apos = a.GetGlobalPosition();

        if (a.Active
          && a.TypeInfo.RadarSize > 0
          && (a.TypeInfo.AlwaysShowInRadar || ActorDistanceInfo.GetRoughDistance(new TV_3DVECTOR(ppos.x, 0, ppos.z), new TV_3DVECTOR(apos.x, 0, apos.z)) < radar_range * 2))
        {
          TV_2DVECTOR posvec = new TV_2DVECTOR(ppos.x, ppos.z) - new TV_2DVECTOR(apos.x, apos.z);
          float proty = p.GetGlobalRotation().y;
          float dist = TrueVision.TVMathLibrary.GetDistanceVec2D(new TV_2DVECTOR(), posvec);
          float angl = TrueVision.TVMathLibrary.Direction2Ang(posvec.x, posvec.y) - proty;
          if (dist > radar_range)
            dist = radar_range;

          float x = radar_center.x - radar_radius * dist / radar_range * (float)Math.Sin(angl * Globals.PI / 180);
          float y = radar_center.y + radar_radius * dist / radar_range * (float)Math.Cos(angl * Globals.PI / 180);
          float scale = a.Scale;
          int scolor = a.HP_Color.GetIntColor();

          if (a.TypeInfo.TargetType.HasFlag(TargetType.ADDON))
          {
            TVScreen2DImmediate.Draw_Circle(x, y, a.TypeInfo.RadarSize * 2, 6, scolor);
          }
          else if (a.TypeInfo.RadarType == RadarType.RECTANGLE_GIANT)
          {
            BoundingBox box = engine.MeshDataSet.Mesh_getBoundingBox(a, true);
            radar_range = (box.Z.Max - box.Z.Min) * scale;

            TVScreen2DImmediate.Draw_Box(box.X.Min * scale * radar_radius / radar_range + radar_center.x
                                       , box.Z.Min * scale * radar_radius / radar_range + radar_center.y
                                       , box.X.Max * scale * radar_radius / radar_range + radar_center.x
                                       , box.Z.Max * scale * radar_radius / radar_range + radar_center.y
                                       , scolor);
          }
          else if (a.TypeInfo.RadarType == RadarType.TRIANGLE_GIANT)
          {
            BoundingBox box = engine.MeshDataSet.Mesh_getBoundingBox(a, true);
            radar_range = (box.Z.Max - box.Z.Min) * scale;

            TVScreen2DImmediate.Draw_Triangle(box.X.Min * scale * radar_radius / radar_range + radar_center.x
                                            , -box.Z.Min * scale * radar_radius / radar_range + radar_center.y
                                            , (box.X.Min + box.X.Max) / 2 * scale * radar_radius / radar_range + radar_center.x
                                            , -box.Z.Max * scale * radar_radius / radar_range + radar_center.y
                                            , box.X.Max * scale * radar_radius / radar_range + radar_center.x
                                            , -box.Z.Min * scale * radar_radius / radar_range + radar_center.y
                                            , scolor);

          }
        }
      }
    }
  }
}
