using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class Transport_Box4ATI : DebrisGroup
  {
    private static Transport_Box4ATI _instance;
    public static Transport_Box4ATI Instance()
    {
      if (_instance == null) { _instance = new Transport_Box4ATI(); }
      return _instance;
    }

    private Transport_Box4ATI() : base("Transport Box 4")
    {
      // Combat
      IsCombatObject = false;
      IsSelectable = false;
      IsDamage = false;
      CollisionEnabled = false;
      OnTimedLife = true;
      TimedLife = 12f;

      MaxSpeed = 500;
      MinSpeed = 100;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"transport\transport_box4.x");
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      base.ProcessState(ainfo);

      ainfo.ApplyZBalance = false;

      if (!ainfo.IsStateFDefined("RotateAngle"))
      {
        ainfo.SetStateF("RotateAngle", Engine.Instance().Random.Next(-450, 450));
      }
      float rotZ = ainfo.GetStateF("RotateAngle") * Game.Instance().TimeSinceRender;
      ainfo.Rotate(0, 0, rotZ);
      ainfo.XTurnAngle = 0;
      ainfo.YTurnAngle = 0;
    }
  }
}

