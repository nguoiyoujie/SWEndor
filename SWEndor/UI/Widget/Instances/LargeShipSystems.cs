using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using System;

namespace SWEndor.UI.Widgets
{
  public class LargeShipSystems : Widget
  {
    private TV_2DVECTOR radar_center;
    private float radar_radius;
    private float radar_range;

    private TV_COLOR pcolor { get { return (PlayerInfo.Actor?.Faction == null) ? new TV_COLOR(1, 1, 1, 1) : PlayerInfo.Actor.Faction.Color; } }

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
        return (!Owner.ShowPage
            && PlayerInfo.Actor != null
            && PlayerInfo.Actor.ActorState != ActorState.DEAD
            && PlayerInfo.Actor.ActorState != ActorState.DYING

            && Owner.ShowUI);
      }
    }

    public override void Draw()
    {
      ActorInfo p = PlayerInfo.Actor;
      if (p == null
        || p.CreationState != CreationState.ACTIVE
        || !(p.TypeInfo is ActorTypes.Groups.StarDestroyer
          || p.TypeInfo is ActorTypes.Groups.Warship
        ))
        return;

      DrawRadar();
    }

    private void DrawRadar()
    {
      TVScreen2DImmediate.Action_Begin2D();

      DrawElement(Engine, PlayerInfo.ActorID);
      foreach (int id in PlayerInfo.Actor.Children)
        DrawElement(Engine, id);

      TVScreen2DImmediate.Action_End2D();
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
          TV_2DVECTOR posvec = new TV_2DVECTOR(ppos.x, ppos.z) - new TV_2DVECTOR(apos.x, apos.z);
          float proty = p.GetRotation().y;
          float dist = TrueVision.TVMathLibrary.GetDistanceVec2D(new TV_2DVECTOR(), posvec);
          float angl = TrueVision.TVMathLibrary.Direction2Ang(posvec.x, posvec.y) - proty;
          if (dist > radar_range)
            dist = radar_range;

          float x = radar_center.x - radar_radius * dist / radar_range * (float)Math.Sin(angl * Globals.PI / 180);
          float y = radar_center.y + radar_radius * dist / radar_range * (float)Math.Cos(angl * Globals.PI / 180);
          float scale = Engine.MeshDataSet.Scale_get(a.ID);
          int scolor = Engine.SysDataSet.StrengthColor_get(actorID).GetIntColor();

          if (a.TypeInfo.TargetType.HasFlag(TargetType.ADDON))
          {
            TVScreen2DImmediate.Draw_Circle(x, y, a.TypeInfo.RadarSize * 2, 6, scolor);
          }
          else if (a.TypeInfo.RadarType == RadarType.RECTANGLE_GIANT)
          {
            TV_3DVECTOR boxmin = new TV_3DVECTOR();
            TV_3DVECTOR boxmax = new TV_3DVECTOR();
            a.GetBoundingBox(ref boxmin, ref boxmax, true);
            boxmin *= scale;
            boxmax *= scale;
            radar_range = boxmax.z - boxmin.z;

            TVScreen2DImmediate.Draw_Box(boxmin.x * radar_radius / radar_range + radar_center.x
                                       , boxmin.z * radar_radius / radar_range + radar_center.y
                                       , boxmax.x * radar_radius / radar_range + radar_center.x
                                       , boxmax.z * radar_radius / radar_range + radar_center.y
                                       , scolor);
          }
          else if (a.TypeInfo.RadarType == RadarType.TRIANGLE_GIANT)
          {
            TV_3DVECTOR boxmin = new TV_3DVECTOR();
            TV_3DVECTOR boxmax = new TV_3DVECTOR();
            a.GetBoundingBox(ref boxmin, ref boxmax, true);
            boxmin *= scale;
            boxmax *= scale;
            radar_range = boxmax.z - boxmin.z;

            TVScreen2DImmediate.Draw_Triangle(boxmin.x * radar_radius / radar_range + radar_center.x
                                            , -boxmin.z * radar_radius / radar_range + radar_center.y
                                            , (boxmin.x + boxmax.x) * radar_radius / radar_range / 2 + radar_center.x
                                            , -boxmax.z * radar_radius / radar_range + radar_center.y
                                            , boxmax.x * radar_radius / radar_range + radar_center.x
                                            , -boxmin.z * radar_radius / radar_range + radar_center.y
                                            , scolor);

          }
        }
      }
    }
  }
}
