using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.Actors.Models;
using SWEndor.ActorTypes.Components;
using SWEndor.AI.Actions;
using SWEndor.Core;
using SWEndor.Models;

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
      DyingMoveData.Kill();

      RenderData.CullDistance = 9000;

      RenderData.RadarType = RadarType.TRAILLINE;
      RenderData.RadarSize = 1;

      AIData.Move_CloseEnough = 0;
      MoveLimitData.MaxSecondOrderTurnRateFrac = 0.5f;

      Mask = ComponentMask.LASER_PROJECTILE;
      DamageType = DamageType.NORMAL;
  }

    public float ImpactCloseEnoughDistance = 0;

    public override void ProcessState(Engine engine, ActorInfo ainfo)
    {
      base.ProcessState(engine, ainfo);
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
              target.TypeInfo.ProcessHit(engine, target, ainfo, target.GetGlobalPosition(), new TV_3DVECTOR());
              ainfo.TypeInfo.ProcessHit(engine, ainfo, target, target.GetGlobalPosition(), new TV_3DVECTOR());

              ainfo.OnHitEvent(target);
              target.OnHitEvent(ainfo);
            }
          }
        }
      }
    }
  }
}


