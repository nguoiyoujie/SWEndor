using SWEndor.ActorTypes;

namespace SWEndor.Actors.Components
{
  public static class MoveDecorator
  {
    public static IMoveComponent Create(ActorInfo actor, ActorTypeInfo atype, ActorCreationInfo acreate)
    {
      if (atype.NoMove && atype.NoRotate)
        return NoMove.Instance;

      else if (atype.IsLaser)
        return MoveLaser.Instance;

      else if (atype.NoRotate)
        return new MoveForwardOnly(acreate.InitialSpeed, atype.MaxSpeed, atype.MinSpeed, atype.MaxSpeedChangeRate);

      else if (atype.NoMove)
        return new RotateOnly(atype.MaxTurnRate, atype.MaxSecondOrderTurnRateFrac, atype.ZTilt, atype.ZNormFrac);

      return new MoveNormal(atype, acreate);
    }
  }
}
