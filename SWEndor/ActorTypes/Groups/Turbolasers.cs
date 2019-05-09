using SWEndor.Actors.Data;

namespace SWEndor.ActorTypes.Groups
{
  public class Turbolasers : AddOn
  {
    internal Turbolasers(Factory owner, string name) : base(owner, name)
    {
      // Combat
      CombatData = CombatData.DefaultShip;
      ExplodeData = new ExplodeData(deathTrigger: DeathExplosionTrigger.ALWAYS, deathExplosionType: "Explosion", deathExplosionSize: 5);

      Attack_AngularDelta = 360f;
      Attack_HighAccuracyAngularDelta = 360f;

      TargetType |= TargetType.ADDON;
      RadarType = RadarType.HOLLOW_CIRCLE_S;

      RadarSize = 1;
    }
  }
}

