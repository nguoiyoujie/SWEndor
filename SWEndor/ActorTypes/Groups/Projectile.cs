using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using SWEndor.AI.Actions;

namespace SWEndor.ActorTypes.Groups
{
  public class Projectile : ActorTypeInfo
  {
    internal Projectile(Factory owner, string name) : base(owner, name)
    {
      // Combat
      CombatData = CombatData.Disabled;
      ExplodeData = new ExplodeData(deathTrigger: DeathExplosionTrigger.TIMENOTEXPIRED_ONLY, deathExplosionType: "Explosion");

      EnableDistanceCull = true;
      CullDistance = 6000;

      RadarType = RadarType.TRAILLINE;
      RadarSize = 1;

      Move_CloseEnough = 0;
      MaxSecondOrderTurnRateFrac = 0.5f;

      Mask = ComponentMask.LASER_PROJECTILE;
    }

    public float ImpactCloseEnoughDistance = 0;

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);
      ainfo.DyingMoveComponent = DyingKill.Instance;
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      base.ProcessState(ainfo);
      if (ainfo.ActorState == ActorState.NORMAL)
      {
        float impdist = ImpactCloseEnoughDistance;
        if (impdist > 0 && ainfo.CurrentAction != null && ainfo.CurrentAction is AttackActor)
        {
          ActorInfo target = ActorFactory.Get(((AttackActor)ainfo.CurrentAction).Target_ActorID);
          if (target != null)
          {
            if (target.TypeInfo is Projectile)
              impdist += ((Projectile)target.TypeInfo).ImpactCloseEnoughDistance;

            // Anticipate
            float dist = ActorDistanceInfo.GetDistance(ainfo, target, impdist + 1);

            if (dist < impdist)
            {
              target.TypeInfo.ProcessHit(target, ainfo, target.GetPosition(), new TV_3DVECTOR());
              ainfo.TypeInfo.ProcessHit(ainfo, target, target.GetPosition(), new TV_3DVECTOR());

              ainfo.OnHitEvent(target.ID);
              target.OnHitEvent(ainfo.ID);
            }
          }
        }
      }
    }
  }
}


