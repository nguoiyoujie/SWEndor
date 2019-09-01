using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Groups
{
  public class Asteroid : ActorTypeInfo
  {
    internal Asteroid(Factory owner, string name) : base(owner, name)
    {
      // Combat
      CombatData = CombatData.Disabled;
      Armor = ArmorInfo.Immune;
      Explodes = new ExplodeInfo[] { new ExplodeInfo("ExpL00", 1, 1, ExplodeTrigger.ON_DEATH) };

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

