using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class XWing_RD_LU_WingATI : DebrisGroup
  {
    private static XWing_RD_LU_WingATI _instance;
    public static XWing_RD_LU_WingATI Instance()
    {
      if (_instance == null) { _instance = new XWing_RD_LU_WingATI(); }
      return _instance;
    }

    private XWing_RD_LU_WingATI() : base("XWing_RD_LU_WingATI")
    {
      // Combat
      IsCombatObject = false;
      IsSelectable = false;
      IsDamage = false;
      CollisionEnabled = false;
      OnTimedLife = true;
      TimedLife = 5f;

      MaxSpeed = 500;
      MinSpeed = 100;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"xwing\xwing_rightdown_leftup_wing.x");
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      base.ProcessState(ainfo);

      ainfo.ApplyZBalance = false;

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

  }
}

