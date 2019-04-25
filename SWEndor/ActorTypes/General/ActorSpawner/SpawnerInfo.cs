using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using System.Collections.Generic;

namespace SWEndor
{
  public class SpawnerInfo
  {
    public SpawnerInfo(ActorInfo a)
    {
      Owner = a;
    }

    public bool Enabled = false;

    public readonly ActorInfo Owner = null;
    public float SpawnMoveTime = Globals.Engine.Game.GameTime;
    public float NextSpawnTime = Globals.Engine.Game.GameTime + 1f;

    public ActorTypeInfo[] SpawnTypes = new ActorTypeInfo[0];
    public float SpawnMoveDelay = 4;
    public float SpawnInterval = 5;
    public float SpawnPlayerDelay = 10;
    public int SpawnsRemaining = 30;

    public TV_3DVECTOR[] SpawnLocations = new TV_3DVECTOR[0];
    public TV_3DVECTOR PlayerSpawnLocation = new TV_3DVECTOR();

    public float SpawnSpeed = -1; // -1 means follow spawner, -2 means maxSpeed of spawned
    public TV_3DVECTOR SpawnRotation = new TV_3DVECTOR();
    public TV_3DVECTOR SpawnManualPositioningMult = new TV_3DVECTOR();
    public TV_3DVECTOR SpawnSpeedPositioningMult = new TV_3DVECTOR();

  }
}
