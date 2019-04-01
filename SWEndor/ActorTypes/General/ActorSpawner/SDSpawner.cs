using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using System.Collections.Generic;

namespace SWEndor
{
  public class SDSpawner : SpawnerInfo
  {
    public SDSpawner(ActorInfo a) : base(a)
    {
      SpawnTypes = new ActorTypeInfo[] { TIE_LN_ATI.Instance() };
      SpawnMoveDelay = 4;
      SpawnInterval = 5;
      SpawnsRemaining = 30;

      SpawnLocations = new List<TV_3DVECTOR>() { new TV_3DVECTOR(25, 0, -25)
                                               , new TV_3DVECTOR(25, 0, 25)
                                               , new TV_3DVECTOR(-25, 0, -25)
                                               , new TV_3DVECTOR(-25, 0, 25)
                                               };

      SpawnSpeed = -1;
      SpawnRotation = new TV_3DVECTOR();
      SpawnManualPositioningMult = new TV_3DVECTOR(0, -30, 0);
      SpawnSpeedPositioningMult = new TV_3DVECTOR();
    }
  }
}
