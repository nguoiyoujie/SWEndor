using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class WarshipGroup : ActorTypeInfo
  {
    internal WarshipGroup(string name): base(name)
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CullDistance = 40000;

      ZTilt = 7.5f;
      ZNormFrac = 0.005f;
      RadarSize = 10;

      IsTargetable = true;
      IsShip = true;
    }

    public override void ProcessNewState(ActorInfo ainfo)
    {
      base.ProcessNewState(ainfo);
      if (ainfo.ActorState == ActorState.DYING)
      {
        ainfo.OnTimedLife = true;
        ainfo.TimedLife = 20f;
        ainfo.IsCombatObject = false;
      }
      else if (ainfo.ActorState == ActorState.DEAD)
      {
        ActorCreationInfo acinfo = new ActorCreationInfo(ExplosionWaveATI.Instance());
        acinfo.Position = ainfo.GetPosition();
        ActorInfo.Create(acinfo).AddParent(ainfo);
      }
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      base.ProcessState(ainfo);
      if (ainfo.ActorState == ActorState.DYING)
      {
        ainfo.XTurnAngle += 0.12f * Game.Instance().TimeSinceRender;
        ainfo.MoveAbsolute(2.5f * Game.Instance().TimeSinceRender, -15f * Game.Instance().TimeSinceRender, 0);
      }
    }
  }
}

