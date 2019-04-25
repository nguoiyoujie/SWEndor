using MTV3D65;
using SWEndor.Actors;
using System.Collections.Generic;

namespace SWEndor
{
  public class PlayerSpawner : SpawnerInfo
  {
    public PlayerSpawner(ActorInfo a) : base(a)
    {
      SpawnMoveDelay = 3;
      SpawnInterval = 5;
      SpawnsRemaining = 0;

      SpawnLocations = new TV_3DVECTOR[]{ new TV_3DVECTOR(0, 0, 0) };

      SpawnSpeed = -2;
      SpawnRotation = new TV_3DVECTOR(0, 0, 0);
      SpawnManualPositioningMult = new TV_3DVECTOR(0, 0, 0);
      SpawnSpeedPositioningMult = new TV_3DVECTOR(0, 0, -1);
    }
  }
}
