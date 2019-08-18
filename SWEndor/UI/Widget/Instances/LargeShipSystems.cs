using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.Actors.Traits;
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
        if (!p.Active
          || !(p.TypeInfo is ActorTypes.Groups.StarDestroyer
          || p.TypeInfo is ActorTypes.Groups.Warship
        ))
          return;

        DrawRadar(p);
      }
    }

    private void DrawRadar(ActorInfo p)
    {
      TVScreen2DImmediate.Action_Begin2D();

      DrawElement(Engine, p, p);
      foreach (ActorInfo c in p.Children)
        DrawElement(Engine, p, c);

      TVScreen2DImmediate.Action_End2D();
    }

    private void DrawElement(Engine engine, ActorInfo p, ActorInfo a)
    {
      TV_3DVECTOR ppos = p.GetGlobalPosition();
      TV_3DVECTOR apos = a.GetGlobalPosition();

      if (a.Active
        && a.TypeInfo.RadarSize > 0
        && (a.TypeInfo.AlwaysShowInRadar || ActorDistanceInfo.GetRoughDistance(new TV_3DVECTOR(ppos.x, 0, ppos.z), new TV_3DVECTOR(apos.x, 0, apos.z)) < radar_range * 2))
      {
        TV_2DVECTOR posvec = new TV_2DVECTOR(ppos.x, ppos.z) - new TV_2DVECTOR(apos.x, apos.z);
        float proty = p.Transform.Rotation.y;
        float dist = TrueVision.TVMathLibrary.GetDistanceVec2D(new TV_2DVECTOR(), posvec);
        float angl = TrueVision.TVMathLibrary.Direction2Ang(posvec.x, posvec.y) - proty;
        if (dist > radar_range)
          dist = radar_range;

        float x = radar_center.x - radar_radius * dist / radar_range * (float)Math.Sin(angl * Globals.PI / 180);
        float y = radar_center.y + radar_radius * dist / radar_range * (float)Math.Cos(angl * Globals.PI / 180);
        float scale = a.Transform.Scale;

        int scolor = a.Health.Color.GetIntColor();

        switch (a.TypeInfo.RadarType)
        {
          case RadarType.HOLLOW_CIRCLE_S:
            TVScreen2DImmediate.Draw_Circle(x, y, a.TypeInfo.RadarSize * 2, 6, scolor);
            break;
          case RadarType.HOLLOW_CIRCLE_M:
            TVScreen2DImmediate.Draw_Circle(x, y, a.TypeInfo.RadarSize * 2, 16, scolor);
            break;
          case RadarType.HOLLOW_CIRCLE_L:
            TVScreen2DImmediate.Draw_Circle(x, y, a.TypeInfo.RadarSize * 2, 40, scolor);
            break;
          case RadarType.RECTANGLE_GIANT:
            {
              BoundingBox box = a.MeshModel.GetBoundingBox(true); //engine.MeshDataSet.Mesh_getBoundingBox(actorID, true);
              radar_range = (box.Z.Max - box.Z.Min) * scale;

              TVScreen2DImmediate.Draw_Box(box.X.Min * scale * radar_radius / radar_range + radar_center.x
                                         , box.Z.Min * scale * radar_radius / radar_range + radar_center.y
                                         , box.X.Max * scale * radar_radius / radar_range + radar_center.x
                                         , box.Z.Max * scale * radar_radius / radar_range + radar_center.y
                                         , scolor);
              break;
            }
          case RadarType.TRIANGLE_GIANT:
            {
              BoundingBox box = a.MeshModel.GetBoundingBox(true); //engine.MeshDataSet.Mesh_getBoundingBox(actorID, true);
              radar_range = (box.Z.Max - box.Z.Min) * scale;

              TVScreen2DImmediate.Draw_Triangle(box.X.Min * scale * radar_radius / radar_range + radar_center.x
                                              , -box.Z.Min * scale * radar_radius / radar_range + radar_center.y
                                              , (box.X.Min + box.X.Max) / 2 * scale * radar_radius / radar_range + radar_center.x
                                              , -box.Z.Max * scale * radar_radius / radar_range + radar_center.y
                                              , box.X.Max * scale * radar_radius / radar_range + radar_center.x
                                              , -box.Z.Min * scale * radar_radius / radar_range + radar_center.y
                                              , scolor);

              break;
            }
        }
      }
    }
  }
}

