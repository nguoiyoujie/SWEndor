using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;

namespace SWEndor.ActorTypes.Groups
{
  public class Asteroid : ActorTypeInfo
  {
    internal Asteroid(Factory owner, string name) : base(owner, name)
    {
      // Combat
      CombatData = CombatData.Immune;
      ExplodeData = new ExplodeData(deathTrigger: DeathExplosionTrigger.ALWAYS, deathExplosionType: "Explosion");

      CullDistance = 4500;

      RadarType = RadarType.HOLLOW_CIRCLE_M;

      Mask = ComponentMask.MINDLESS_ACTOR;
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);
    }
  }
}

