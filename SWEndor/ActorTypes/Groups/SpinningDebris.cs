using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Groups
{
  public class SpinningDebris : Debris
  {
    internal SpinningDebris(Factory owner, string name) : base(owner, name)
    {
      CombatData = CombatData.Disabled;
      Armor = ArmorInfo.Immune;
      TimedLifeData = new TimedLifeData(true, 5);

      MaxSpeed = 500;
      MinSpeed = 5;
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);
      ainfo.DyingMoveComponent = new DyingSpin(120, 270);
      ainfo.SetState_Dying();
    }
  }
}

