using SWEndor.Game.Actors;
using SWEndor.Game.ActorTypes.Components;
using SWEndor.Game.Core;

namespace SWEndor.Game.Models
{
  internal struct CargoModel
  {
    public enum CargoScanResult
    {
      NO_SCAN,
      NEW_SCAN,
      ALREADY_SCANNED
    }

    internal bool Scanned; // Whether the cargo has been scanned by the player
    internal string Cargo;
    internal float ScanDistance;
    internal float VisibleDistance;

    public void Init(ref CargoData data)
    {
      VisibleDistance = data.CargoVisibleDistance;
      Scanned = false;
      ScanDistance = 0;
      Cargo = null;
    }

    public void Reset()
    {
      VisibleDistance = 200;
      Scanned = false;
      ScanDistance = 0;
      Cargo = null;
    }

    public static CargoScanResult ScanCargo(Engine engine, ActorInfo player, ActorInfo target)
    {
      if (player != null && !player.IsDyingOrDead)
      {
        if (target != null && !target.IsDyingOrDead)
        {
          if (player.IsSystemOperational(Actors.Models.SystemPart.SCANNER))
          {
            if (target.Cargo.Scanned)
            {
              return CargoScanResult.ALREADY_SCANNED;
            }

            if (DistanceModel.GetDistance(engine, player, target) < player.Cargo.ScanDistance + target.Cargo.VisibleDistance)
            {
              return CargoScanResult.NEW_SCAN;
            }
          }
        }
      }
      return CargoScanResult.NO_SCAN;
    }
  }
}
