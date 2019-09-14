﻿using SWEndor.Actors;
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
      ArmorData = ArmorData.Immune;
      TimedLifeData = new TimedLifeData(true, 5);

      DyingMoveData.Spin(120, 270);

      MoveLimitData.MaxSpeed = 500;
      MoveLimitData.MinSpeed = 5;
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);
      ainfo.SetState_Dying();
    }
  }
}

