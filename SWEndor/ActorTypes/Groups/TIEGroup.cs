using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class TIEGroup : ActorTypeInfo
  {
    internal TIEGroup(string name): base(name)
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;

      ZTilt = 2.5f;
      ZNormFrac = 0.01f;
      RadarSize = 2;

      CullDistance = 7200;

      IsTargetable = true;
      IsFighter = true;
      CanEvade = true;
      CanRetaliate = true;
      CanCheckCollisionAhead = true;

      HuntWeight = 5;
    }

    public override void ProcessNewState(ActorInfo ainfo)
    {
      base.ProcessNewState(ainfo);
      if (ainfo.ActorState == ActorState.DYING)
      {

        ainfo.ApplyZBalance = false;
        ainfo.OnTimedLife = true;

        if (ainfo.GetAllParents(1).Count > 0 || (!ainfo.GetStateB("No2ndKill") && Engine.Instance().Random.NextDouble() < 0.3f))
        {
          ainfo.TimedLife = 0.1f;
        }
        else
        {
          ainfo.TimedLife = 5f;
        }

        ainfo.IsCombatObject = false;

        ActorCreationInfo acinfo = new ActorCreationInfo(ElectroATI.Instance());
        acinfo.Position = ainfo.GetPosition();
        ActorInfo.Create(acinfo).AddParent(ainfo);
      }
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      base.ProcessState(ainfo);
      if (ainfo.ActorState == ActorState.DYING)
      {
        if (!ainfo.IsStateFDefined("RotateAngle"))
        {
          double d = Engine.Instance().Random.NextDouble();

          if (d > 0.5f)
          {
            ainfo.SetStateF("RotateAngle", Engine.Instance().Random.Next(180, 270));
          }
          else
          {
            ainfo.SetStateF("RotateAngle", Engine.Instance().Random.Next(-270, -180));
          }
        }
        float rotZ = ainfo.GetStateF("RotateAngle") * Game.Instance().TimeSinceRender;
        ainfo.Rotate(0, 0, rotZ);
        ainfo.XTurnAngle = 0;
        ainfo.YTurnAngle = 0;
      }

      if (ainfo.CreationState == CreationState.ACTIVE && (!GameScenarioManager.Instance().IsCutsceneMode || !ainfo.IsPlayer()))
      {
        TV_3DVECTOR engineloc = ainfo.GetRelativePositionXYZ(0, 0, -30);
        float dist = Engine.Instance().TVMathLibrary.GetDistanceVec3D(PlayerInfo.Instance().Position, engineloc);

        if (!ainfo.IsPlayer())
        {
          if (dist < 200)
          {
            if (PlayerInfo.Instance().enginetevol < 1 - dist / 200.0f)
              PlayerInfo.Instance().enginetevol = 1 - dist / 200.0f;
          }
        }
      }
    }

    public override void ProcessHit(ActorInfo ainfo, ActorInfo hitby, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      if (ainfo.ActorState == ActorState.DYING && !ainfo.GetStateB("No2ndKill"))
      {
        ainfo.ActorState = ActorState.DEAD;
      }

      base.ProcessHit(ainfo, hitby, impact, normal);
    }
  }
}

