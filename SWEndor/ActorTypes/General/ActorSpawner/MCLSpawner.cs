using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;

namespace SWEndor
{
  public class MCLSpawner : SpawnerInfo
  {
    public MCLSpawner(ActorInfo a) : base(a)
    {
      SpawnTypes = new ActorTypeInfo[] { a.ActorTypeFactory.Get("Z-95")
                                       , a.ActorTypeFactory.Get("Y-Wing")
                                       };
      SpawnMoveDelay = 2.5f;
      SpawnInterval = 15;
      SpawnsRemaining = 99;

      SpawnLocations = new TV_3DVECTOR[]{ new TV_3DVECTOR(-20, 0, 0) };

      SpawnSpeed = 125;
      SpawnRotation = new TV_3DVECTOR(0, 90, 0);
      SpawnManualPositioningMult = new TV_3DVECTOR(0, 0, 0);
      SpawnSpeedPositioningMult = new TV_3DVECTOR(0, 0, 1);
    }
  }
}
