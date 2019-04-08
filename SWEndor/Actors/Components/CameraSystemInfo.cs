using MTV3D65;
using System.Collections.Generic;

namespace SWEndor.Actors.Components
{
  public struct CameraSystemInfo
  {
    private readonly ActorInfo Actor;
    public TV_3DVECTOR DefaultCamLocation;
    public TV_3DVECTOR DefaultCamTarget;
    public TV_3DVECTOR[] CamLocations;
    public TV_3DVECTOR[] CamTargets;

    public float CamDeathCircleRadius;
    public float CamDeathCircleHeight;
    public float CamDeathCirclePeriod;

    public CameraSystemInfo(ActorInfo actor)
    {
      Actor = actor;

      DefaultCamLocation = new TV_3DVECTOR();
      DefaultCamTarget = new TV_3DVECTOR(0, 0, 2000);
      CamLocations = new TV_3DVECTOR[0];
      CamTargets = new TV_3DVECTOR[0];

      CamDeathCircleRadius = 350;
      CamDeathCircleHeight = 25;
      CamDeathCirclePeriod = 15;
    }

    public void Reset()
    {
      DefaultCamLocation = new TV_3DVECTOR();
      DefaultCamTarget = new TV_3DVECTOR(0, 0, 2000);
      CamLocations = new TV_3DVECTOR[0];
      CamTargets = new TV_3DVECTOR[0];

      CamDeathCircleRadius = 350;
      CamDeathCircleHeight = 25;
      CamDeathCirclePeriod = 15;
    }
  }
}
