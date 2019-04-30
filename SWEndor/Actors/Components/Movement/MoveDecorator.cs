using SWEndor.ActorTypes;

namespace SWEndor.Actors.Components
{
  public static class MoveDecorator
  {
    public static IMoveComponent Create(ActorInfo actor, ActorTypeInfo atype, ActorCreationInfo acreate)
    {
      if (atype.NoMove)
        return NoMove.Instance;

      else if (atype.MaxTurnRate == 0 && atype.MaxSpeed == Globals.LaserSpeed)
        return MoveLaser.Instance;

      else if (atype.MaxTurnRate == 0)
        return new MoveForwardOnly(acreate.InitialSpeed, atype.MaxSpeed, atype.MinSpeed, atype.MaxSpeedChangeRate);

      else if (atype.MaxSpeed == 0)
        return new RotateOnly(atype.MaxTurnRate, atype.MaxSecondOrderTurnRateFrac, atype.ZTilt, atype.ZNormFrac);

      return new MoveNormal(atype, acreate);
    }
  }
}
