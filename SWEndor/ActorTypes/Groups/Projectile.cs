using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;
using SWEndor.AI.Actions;

namespace SWEndor.ActorTypes.Groups
{
  public class Projectile : ActorTypeInfo
  {
    internal Projectile(Factory owner, string name) : base(owner, name)
    {
      // Combat
      CombatData = CombatData.Disabled;
      ArmorData = ArmorData.Immune;
      Explodes = new ExplodeInfo[]
      {
        new ExplodeInfo("ExpS00", 1, 1, ExplodeTrigger.ON_DEATH | ExplodeTrigger.ONLY_WHEN_DYINGTIME_NOT_EXPIRED)
      };

      RenderData.CullDistance = 9000;

      RenderData.RadarType = RadarType.TRAILLINE;
      RenderData.RadarSize = 1;

      AIData.Move_CloseEnough = 0;
      MoveLimitData.MaxSecondOrderTurnRateFrac = 0.5f;

      Mask = ComponentMask.LASER_PROJECTILE;
      DamageType = DamageType.NORMAL;
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
      if (!ainfo.IsDyingOrDead)
      {
        float impdist = ImpactCloseEnoughDistance;
        if (impdist > 0 && ainfo.CurrentAction != null && ainfo.CurrentAction is ProjectileAttackActor)
        {
          ActorInfo target = ((ProjectileAttackActor)ainfo.CurrentAction).Target_Actor;
          if (target != null)
          {
            if (target.TypeInfo is Projectile)
              impdist += ((Projectile)target.TypeInfo).ImpactCloseEnoughDistance;

            // Anticipate
            float dist = ActorDistanceInfo.GetDistance(ainfo, target, impdist + 1);

            if (dist < impdist)
            {
              target.TypeInfo.ProcessHit(target, ainfo, target.GetGlobalPosition(), new TV_3DVECTOR());
              ainfo.TypeInfo.ProcessHit(ainfo, target, target.GetGlobalPosition(), new TV_3DVECTOR());

              ainfo.OnHitEvent(target);
              target.OnHitEvent(ainfo);
            }
          }
        }
      }
    }
  }
}


