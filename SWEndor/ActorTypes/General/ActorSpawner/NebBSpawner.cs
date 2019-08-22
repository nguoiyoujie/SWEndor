using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;

namespace SWEndor
{
  public class NebBSpawner : SpawnerInfo
  {
    public NebBSpawner(ActorInfo a) : base(a)
    {
      SpawnTypes = new ActorTypeInfo[] { a.ActorTypeFactory.Get("Z-95")
                                       , a.ActorTypeFactory.Get("Y-Wing")
                                       };
      SpawnMoveDelay = 2;
      SpawnInterval = 10;
      SpawnsRemaining = 99;

      SpawnLocations = new TV_3DVECTOR[]{ new TV_3DVECTOR(-20, 0, 0) };

      SpawnSpeed = 25;
      SpawnRotation = new TV_3DVECTOR(0, 90, 0);
      SpawnManualPositioningMult = new TV_3DVECTOR(0, 0, 0);
      SpawnSpeedPositioningMult = new TV_3DVECTOR(0, 0, 1);
    }
  }
}
