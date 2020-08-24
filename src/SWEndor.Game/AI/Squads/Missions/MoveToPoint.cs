using MTV3D65;
using SWEndor.Game.Actors;
using SWEndor.Game.AI.Actions;
using SWEndor.Game.Core;
using SWEndor.Game.Models;

namespace SWEndor.Game.AI.Squads.Missions
{
  public class MoveToPoint : MissionInfo
  {
    private TV_3DVECTOR Target_Position;
    private float Speed;
    private float Close_enough_distance;

    public MoveToPoint(TV_3DVECTOR target_position, float speed, float close_enough_distance)
    {
      Target_Position = target_position;
      Speed = speed;
      Close_enough_distance = close_enough_distance;
    }

    public override ActionInfo GetNewAction(Engine engine, Squadron squad)
    {
      return Actions.Move.GetOrCreate(Target_Position, Speed, Close_enough_distance);
    }

    public override void Process(Engine engine, Squadron squad)
    {
      foreach (ActorInfo a in squad.Members)
      {
        float dist = DistanceModel.GetDistance(engine.TrueVision.TVMathLibrary, a.GetGlobalPosition(), Target_Position);
        if (dist > Close_enough_distance)
          return;
      }

      Complete = true;
    }
  }
}
