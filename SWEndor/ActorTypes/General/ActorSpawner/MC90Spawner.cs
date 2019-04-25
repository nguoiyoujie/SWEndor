using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;

namespace SWEndor
{
  public class MC90Spawner : SpawnerInfo
  {
    public MC90Spawner(ActorInfo a) : base(a)
    {
      SpawnTypes = new ActorTypeInfo[] { a.TypeInfo.Owner.Get("X-Wing")
                                       , a.TypeInfo.Owner.Get("A-Wing")
                                       , a.TypeInfo.Owner.Get("Y-Wing")
                                       , a.TypeInfo.Owner.Get("B-Wing")
                                       };
      SpawnMoveDelay = 3;
      SpawnInterval = 10;
      SpawnsRemaining = 99;

      SpawnLocations = new TV_3DVECTOR[]{ new TV_3DVECTOR(-20, 0, 0) };

      SpawnSpeed = 75;
      SpawnRotation = new TV_3DVECTOR(0, 90, 0);
      SpawnManualPositioningMult = new TV_3DVECTOR(0, 0, 0);
      SpawnSpeedPositioningMult = new TV_3DVECTOR(0, 0, -1);
    }
  }
}
