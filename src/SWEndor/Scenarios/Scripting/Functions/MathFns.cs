using SWEndor.Actors;
using SWEndor.Models;
using Primrose.Primitives.ValueTypes;
using SWEndor.Primitives.Extensions;
using Primrose.Expressions;
using Primrose.Primitives.Extensions;

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

    public static Val Int(Context context, float val)
    {
      return new Val((int)val);
    }

    public static Val Max(Context context, float val1, float val2)
    {
      return new Val(val1.Max(val2));
    }

    public static Val Min(Context context, float val1, float val2)
    {
      return new Val(val1.Min(val2));
    }

    public static Val FormatAsTime(Context context, float val)
    {
      return new Val("{0:00}:{1:00}".F((int)(val) / 60, (int)(val) % 60));
    }
  }
}
