using SWEndor.Actors;
using SWEndor.Models;
using Primrose.Primitives.ValueTypes;
using SWEndor.Primitives.Extensions;
using Primrose.Expressions;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class MathFns
  {
    public static Val GetDistance(Context context, float3 pt1, float3 pt2)
    {
      return new Val(DistanceModel.GetDistance(context.Engine.TrueVision.TVMathLibrary, pt1.ToVec3(), pt2.ToVec3()));
    }

    public static Val GetActorDistance(Context context, int actorID1, int actorID2)
    {
      ActorInfo a1 = context.Engine.ActorFactory.Get(actorID1);
      ActorInfo a2 = context.Engine.ActorFactory.Get(actorID2);
      return new Val(DistanceModel.GetDistance(context.Engine, a1, a2));
    }

    public static Val GetActorDistance(Context context, int actorID1, int actorID2, float limit)
    {
      ActorInfo a1 = context.Engine.ActorFactory.Get(actorID1);
      ActorInfo a2 = context.Engine.ActorFactory.Get(actorID2);
      return new Val(DistanceModel.GetDistance(context.Engine, a1, a2, limit));
    }
  }
}
