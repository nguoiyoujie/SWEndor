using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.AI.Actions;

namespace SWEndor.ActorTypes.Group
{
  public class Projectile : ActorTypeInfo
  {
    internal Projectile(Factory owner, string name) : base(owner, name)
    {
      // Combat
      IsCombatObject = false;
      IsSelectable = false;
      IsDamage = true;
      CollisionEnabled = false;
      EnableDistanceCull = true;
      CullDistance = 6000;

      RadarSize = 1;

      Move_CloseEnough = 0;
      MaxSecondOrderTurnRateFrac = 0.5f;
    }

    public float ImpactCloseEnoughDistance = 0;

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.ExplosionInfo.DeathExplosionTrigger = DeathExplosionTrigger.TIMENOTEXPIRED_ONLY;
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      base.ProcessState(ainfo);
      if (ainfo.ActorState == ActorState.NORMAL)
      {
        if (ImpactCloseEnoughDistance > 0 && ainfo.CurrentAction != null && ainfo.CurrentAction is AttackActor)
        {
          ActorInfo target = ActorFactory.Get(((AttackActor)ainfo.CurrentAction).Target_ActorID);
          if (target != null)
          {
            // Anticipate
            float dist = ActorDistanceInfo.GetDistance(ainfo.ID, target.ID, ImpactCloseEnoughDistance + 1);

            if (dist < ImpactCloseEnoughDistance)
            {
              target.TypeInfo.ProcessHit(target.ID, ainfo.ID, target.GetPosition(), new TV_3DVECTOR());
              ainfo.TypeInfo.ProcessHit(ainfo.ID, target.ID, target.GetPosition(), new TV_3DVECTOR());

              ainfo.OnHitEvent(target.ID);
              target.OnHitEvent(ainfo.ID);
            }
          }
        }
      }
    }

    public override void ProcessNewState(ActorInfo ainfo)
    {
      base.ProcessNewState(ainfo);
      if (ainfo.ActorState.IsDying())
      {
        /*
        if (ainfo.CombatInfo.TimedLife > 0 
          && ainfo.ExplosionInfo.DeathExplosionTrigger)
        {
          ActorCreationInfo acinfo = new ActorCreationInfo(ActorTypeFactory.Get(ainfo.ExplosionInfo.DeathExplosionType));
          acinfo.Position = ainfo.GetPosition();
          ActorInfo.Create(ActorFactory, acinfo);
        }
        */
        ainfo.ActorState = ActorState.DEAD;
      }
    }
  }
}


