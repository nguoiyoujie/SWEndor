using SWEndor.Actors;
using SWEndor.Actors.Data;

namespace SWEndor.ActorTypes.Groups
{
  public class SurfaceTower : ActorTypeInfo
  {
    internal SurfaceTower(Factory owner, string name) : base(owner, name)
    {
      // Combat
      CombatData = CombatData.DefaultShip;
      Armor = ActorInfo.ArmorModel.Default;
      ExplodeData = new ExplodeData(deathTrigger: DeathExplosionTrigger.ALWAYS, deathExplosionSize: 5);

      CullDistance = 10000;

      RadarSize = 1.5f;

      TargetType = TargetType.ADDON | TargetType.STRUCTURE;
      RadarType = RadarType.FILLED_SQUARE;

      Mask = ComponentMask.STATIC_ACTOR;
    }
  }
}

