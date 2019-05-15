using SWEndor.ActorTypes;

namespace SWEndor.Actors.Components
{
  public static class MoveDecorator
  {
    public static IMoveComponent Create(ActorTypeInfo atype)
    {
      if (atype.Mask.Has(ComponentMask.CAN_MOVE | ComponentMask.CAN_ROTATE))
        return MoveNormal.Instance;

      else if (atype.IsLaser)
        return MoveLaser.Instance;

      else if (atype.Mask.Has(ComponentMask.CAN_MOVE))
        return MoveForwardOnly.Instance;

      else if (atype.Mask.Has(ComponentMask.CAN_ROTATE))
        return RotateOnly.Instance;

      return NoMove.Instance;
    }
  }
}
