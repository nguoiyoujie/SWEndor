using MTV3D65;
using SWEndor.Actors;
using SWEndor.AI.Actions;

namespace SWEndor.ActorTypes
{
  public class ProjectileGroup : ActorTypeInfo
  {
    internal ProjectileGroup(string name) : base(name)
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

    public override void ProcessState(ActorInfo ainfo)
    {
      base.ProcessState(ainfo);
      if (ainfo.ActorState == ActorState.NORMAL)
      {
        if (ImpactCloseEnoughDistance > 0 && ainfo.CurrentAction != null && ainfo.CurrentAction is AttackActor)
        {
          AttackActor action = (AttackActor)ainfo.CurrentAction;
          if (action.Target_Actor != null)
          {
            // Anticipate
            float dist = ActorDistanceInfo.GetDistance(ainfo, action.Target_Actor, ImpactCloseEnoughDistance + 1);

            if (dist < ImpactCloseEnoughDistance)
            {
              action.Target_Actor.TypeInfo.ProcessHit(action.Target_Actor, ainfo, action.Target_Actor.GetPosition(), new TV_3DVECTOR());
              ainfo.TypeInfo.ProcessHit(ainfo, action.Target_Actor, action.Target_Actor.GetPosition(), new TV_3DVECTOR());

              ainfo.OnHitEvent(action.Target_Actor);
              action.Target_Actor.OnHitEvent(ainfo);
            }
          }
        }
      }
    }
  }
}


