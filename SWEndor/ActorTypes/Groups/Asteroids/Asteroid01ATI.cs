using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class Asteroid01ATI : AsteroidGroup
  {
    private static Asteroid01ATI _instance;
    public static Asteroid01ATI Instance()
    {
      if (_instance == null) { _instance = new Asteroid01ATI(); }
      return _instance;
    }

    private Asteroid01ATI() : base("Asteroid 01")
    {
      CullDistance = 10000;
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"asteroids\asteroid01.x");
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

      if (!ainfo.IsStateFDefined("RotateAngleRate"))
      {
        double d = Engine.Instance().Random.NextDouble() * 2.5;
        ainfo.SetStateF("RotateAngleRate", (float)d);
      }
      float rotZ = ainfo.GetStateF("RotateAngle") * Game.Instance().TimeSinceRender * ainfo.GetStateF("RotateAngleRate");
      ainfo.Rotate(0, 0, rotZ);
      ainfo.XTurnAngle = 0;
      ainfo.YTurnAngle = 0;
    }
  }
}

