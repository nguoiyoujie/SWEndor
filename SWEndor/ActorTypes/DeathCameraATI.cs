using MTV3D65;
using SWEndor.Scenarios;
using System;

namespace SWEndor.Actors.Types
{
  public enum DeathCamMode { CIRCLE, FOLLOW };

  public class DeathCameraATI : ActorTypeInfo
  {
    private static DeathCameraATI _instance;
    public static DeathCameraATI Instance()
    {
      if (_instance == null) { _instance = new DeathCameraATI(); }
      return _instance;
    }

    private DeathCameraATI() : base("Death Camera")
    {
      // Combat
      IsCombatObject = false;
      IsSelectable = false;
      IsDamage = false;
      EnableDistanceCull = false;
      CollisionEnabled = false;

      SourceMesh = Engine.Instance().TVGlobals.GetMesh(Key);
      if (SourceMesh == null)
      {
        SourceMesh = Engine.Instance().TVScene.CreateMeshBuilder(Key);
        SourceMesh.Enable(false);
        SourceMesh.SetCollisionEnable(false);
      }
    }

    public override void ChaseCamera(ActorInfo ainfo)
    {
      TVCamera cam = PlayerCameraInfo.Instance().Camera;

      float circleperiod = ainfo.CamDeathCirclePeriod;
      float angularphase = (Game.Instance().GameTime % circleperiod) * (2 * Globals.PI / circleperiod);
      float radius = ainfo.CamDeathCircleRadius;
      float height = ainfo.CamDeathCircleHeight;

      TV_3DVECTOR pos = ainfo.GetPosition();
      switch (GameScenarioManager.Instance().Scenario.DeathCamMode)
      {
        case DeathCamMode.CIRCLE:
          cam.SetPosition(pos.x + radius * (float)Math.Cos(angularphase)
                        , pos.y + height
                        , pos.z + radius * (float)Math.Sin(angularphase));

          cam.SetLookAt(pos.x
                        , pos.y
                        , pos.z);
          break;
        case DeathCamMode.FOLLOW:
          TV_3DVECTOR pos2 = ainfo.GetRelativePositionFUR(0, ainfo.CamDeathCircleHeight, -ainfo.CamDeathCircleRadius, true);
          cam.SetPosition(pos2.x
                        , pos2.y
                        , pos2.z);

          cam.SetLookAt(pos.x
                        , pos.y
                        , pos.z);
          break;
      }
    }
  }
}


