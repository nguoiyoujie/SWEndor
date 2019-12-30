using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Groups
{
  internal class MissileProjectile : ActorTypeInfo
  {
    internal MissileProjectile(Factory owner, string id, string name) : base(owner, id, name)
    {
      // Combat
      CombatData = CombatData.DefaultFighter;
      ArmorData = ArmorData.Default;
      ExplodeSystemData.Explodes = new ExplodeData[]
      {
        new ExplodeData("EXPL00", 1, 1, ExplodeTrigger.ON_DEATH | ExplodeTrigger.ONLY_WHEN_DYINGTIME_NOT_EXPIRED)
      };

      RenderData.CullDistance = 12000;
      RenderData.RadarType = RadarType.TRAILLINE;
      RenderData.RadarSize = 1;
      
      // Projectile
      CombatData.ImpactCloseEnoughDistance = 100;
      AIData.TargetType = TargetType.MUNITION;

      Mask = ComponentMask.GUIDED_PROJECTILE;

      AIData.Move_CloseEnough = 0;
      MoveLimitData.MaxSecondOrderTurnRateFrac = 0.5f;

      CombatData.DamageType = DamageType.MISSILE;

      WeapSystemData.TrackerDummyWeapon = true;
    }
  }
}


