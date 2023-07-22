﻿using SWEndor.Game.Models;

namespace SWEndor.Game.Actors
{
  public partial class ActorInfo
  {
    public bool CanCollide
    {
      get
      {
        return (IsPlayer && !Engine.PlayerInfo.PlayerAIEnabled)
            || TargetType.Has(TargetType.MUNITION)
            || (IsDying && TargetType.Has(TargetType.FIGHTER));
      }
    }

    public bool CanCollideWith(ActorInfo checkActor)
    {
      return checkActor != null
                 && TopParent != checkActor.TopParent
                 && checkActor.Mask.Has(ComponentMask.CAN_BECOLLIDED)
                 && !checkActor.IsAggregateMode;
    }

    public void DoCollide(ActorInfo target, ref CollisionResultData data)
    {
      target.TypeInfo.ProcessHit(Engine, target, this, data.Impact, data.Normal);
      TypeInfo.ProcessHit(Engine, this, target, data.Impact, -1 * data.Normal);
    }
  }
}
