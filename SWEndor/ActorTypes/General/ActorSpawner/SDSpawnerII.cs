using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.ActorTypes.Instances;
using System.Collections.Generic;

namespace SWEndor
{
  public class SDSpawnerII : SpawnerInfo
  {
    public SDSpawnerII(ActorInfo a) : base(a)
    {
      SpawnTypes = new ActorTypeInfo[] { a.TypeInfo.Owner.Get("TIE Interceptor") };
      SpawnMoveDelay = 4;
      SpawnInterval = 5;
      SpawnsRemaining = 99;

      SpawnLocations = new TV_3DVECTOR[]{ new TV_3DVECTOR(25, 0, -25)
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
