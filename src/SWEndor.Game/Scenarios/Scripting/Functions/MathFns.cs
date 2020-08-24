using SWEndor.Game.Actors;
using SWEndor.Game.Models;
using Primrose.Primitives.ValueTypes;
using SWEndor.Game.Primitives.Extensions;
using Primrose.Expressions;
using Primrose.Primitives.Extensions;
using SWEndor.Game.Core;

namespace SWEndor.Game.Scenarios.Scripting.Functions
{
  public static class MathFns
  {
    public static Val GetDistance(IContext context, float3 pt1, float3 pt2)
    {
      return new Val(DistanceModel.GetDistance(((Context)context).Engine.TrueVision.TVMathLibrary, pt1.ToVec3(), pt2.ToVec3()));
    }

    public static Val GetActorDistance(IContext context, int actorID1, int actorID2)
    {
      Engine e = ((Context)context).Engine;
      ActorInfo a1 = e.ActorFactory.Get(actorID1);
      ActorInfo a2 = e.ActorFactory.Get(actorID2);
      return new Val(DistanceModel.GetDistance(e, a1, a2));
    }

    public static Val GetActorDistance(IContext context, int actorID1, int actorID2, float limit)
    {
      Engine e = ((Context)context).Engine;
      ActorInfo a1 = e.ActorFactory.Get(actorID1);
      ActorInfo a2 = e.ActorFactory.Get(actorID2);
      return new Val(DistanceModel.GetDistance(e, a1, a2, limit));
    }

    public static Val Int(IContext context, float val)
    {
      return new Val((int)val);
    }

    public static Val Max(IContext context, float val1, float val2)
    {
      return new Val(val1.Max(val2));
    }

    public static Val Min(IContext context, float val1, float val2)
    {
      return new Val(val1.Min(val2));
    }

    public static Val FormatAsTime(IContext context, float val)
    {
      return new Val("{0:00}:{1:00}".F((int)(val) / 60, (int)(val) % 60));
    }
  }
}
