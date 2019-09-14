﻿using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes;

namespace SWEndor.ActorTypes.Components
{
  public struct MoveBehavior
  {
    public delegate void MoveAction(ActorInfo actor, ref MoveData data);
    public MoveAction Move;

    public void Load(ActorTypeInfo atype)
    {
      if (atype.Mask.Has(ComponentMask.CAN_MOVE | ComponentMask.CAN_ROTATE))
        Move = MoveNormal.Move;

      else if (atype.IsLaser)
        Move = MoveLaser.Move;

      else if (atype.Mask.Has(ComponentMask.CAN_MOVE))
        Move = MoveForwardOnly.Move;

      else if (atype.Mask.Has(ComponentMask.CAN_ROTATE))
        Move = RotateOnly.Move;

      else
        Move = NoMove.Move;
    }
  }
}
