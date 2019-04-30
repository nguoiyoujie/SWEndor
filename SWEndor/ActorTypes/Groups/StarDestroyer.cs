﻿using SWEndor.Actors;
using SWEndor.Actors.Components;

namespace SWEndor.ActorTypes.Groups
{
  public class StarDestroyer : ActorTypeInfo
  {
    internal StarDestroyer(Factory owner, string name) : base(owner, name)
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      CullDistance = 20000;

      ZTilt = 2.5f;
      ZNormFrac = 0.011f;
      RadarSize = 10;

      TargetType = TargetType.SHIP;
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);
      ainfo.ExplosionInfo.DeathExplosionTrigger = DeathExplosionTrigger.ALWAYS;
      ainfo.ExplosionInfo.DeathExplosionType = "ExplosionLg";
      ainfo.ExplosionInfo.DeathExplosionSize = 2f;
      ainfo.ExplosionInfo.ExplosionRate = 0.5f;
      ainfo.ExplosionInfo.ExplosionSize = 1;
      ainfo.ExplosionInfo.ExplosionType = "ExplosionSm";

      ainfo.DyingMoveComponent = new DyingSink(0.06f, 15f, 2.5f);
    }

    public override void ProcessNewState(ActorInfo ainfo)
    {
      base.ProcessNewState(ainfo);
      if (ainfo.ActorState.IsDying())
      {
        ainfo.CombatInfo.OnTimedLife = true;
        ainfo.CombatInfo.TimedLife = 25f;
        ainfo.CombatInfo.IsCombatObject = false;
      }
      else if (ainfo.ActorState.IsDead())
      {
        ActorCreationInfo acinfo = new ActorCreationInfo(ActorTypeFactory.Get("Explosion Wave"));
        acinfo.Position = ainfo.GetPosition();
        ActorInfo.Create(ActorFactory, acinfo).AddParent(ainfo.ID);
      }
    }
  }
}

