using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;
using SWEndor.Core;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Groups
{
  public class SpinningDebris : Debris
  {
    internal SpinningDebris(Factory owner, string name) : base(owner, name)
    {
      CombatData = CombatData.Disabled;
      ArmorData = ArmorData.Immune;
      Explodes = new ExplodeData[]
      {
        new ExplodeData("ExpS00", 0.5f, 1, ExplodeTrigger.ON_DYING | ExplodeTrigger.CREATE_ON_MESHVERTICES),
      };

      TimedLifeData = new TimedLifeData(true, 5);
      DyingMoveData.Spin(120, 270);

      MoveLimitData.MaxSpeed = 500;
      MoveLimitData.MinSpeed = 5;
    }

    public override void Initialize(Engine engine, ActorInfo ainfo)
    {
      base.Initialize(engine, ainfo);
      ainfo.SetState_Dying();
    }
  }
}

