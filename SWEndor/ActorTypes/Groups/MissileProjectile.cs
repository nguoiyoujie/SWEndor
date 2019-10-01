using SWEndor.Actors.Data;
using SWEndor.Actors.Models;
using SWEndor.ActorTypes.Components;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Groups
{
  public class MissileProjectile : Projectile
  {
    internal MissileProjectile(Factory owner, string id, string name) : base(owner, id, name)
    {
      // Combat
      CombatData = CombatData.DefaultFighter;
      ArmorData = ArmorData.Default;
      Explodes = new ExplodeData[]
      {
        new ExplodeData("EXPL00", 1, 1, ExplodeTrigger.ON_DEATH | ExplodeTrigger.ONLY_WHEN_DYINGTIME_NOT_EXPIRED)
      };

      RenderData.CullDistance = 12000;

      // Projectile
      AIData.ImpactCloseEnoughDistance = 100;
      AIData.TargetType = TargetType.MUNITION;
      RenderData.RadarType = RadarType.TRAILLINE;

      Mask = ComponentMask.GUIDED_PROJECTILE;

      TrackerDummyWeapon = true;
    }
  }
}


