using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Groups
{
  public class SurfaceGun : ActorTypeInfo
  {
    internal SurfaceGun(Factory owner, string name) : base(owner, name)
    {
      // Combat
      CombatData = CombatData.DefaultShip;
      Armor = ArmorInfo.Default;
      ExplodeData = new ExplodeData(deathTrigger: DeathExplosionTrigger.ALWAYS, deathExplosionSize: 5);

      CullDistance = 10000;

      RadarSize = 0;
      TargetType = TargetType.ADDON;
      RadarType = RadarType.NULL;

      Mask = ComponentMask.ACTOR;
    }
  }
}

