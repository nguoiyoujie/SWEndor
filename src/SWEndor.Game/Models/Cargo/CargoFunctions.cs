using SWEndor.Game.Actors;
using SWEndor.Game.Core;

namespace SWEndor.Game.Models
{
  internal static class CargoFunctions
  {
    public enum CargoScanResult
    {
      NO_SCAN,
      NEW_SCAN,
      ALREADY_SCANNED
    }

    public static CargoScanResult ScanCargo(Engine engine, ActorInfo player, ActorInfo target)
    {
      if (player != null && !player.IsDyingOrDead)
      {
        if (target != null && !target.IsDyingOrDead)
        {
          if (player.IsSystemOperational(Actors.Models.SystemPart.SCANNER))
          {
            if (target.CargoScanned)
            {
              return CargoScanResult.ALREADY_SCANNED;
            }

            if (DistanceModel.GetDistance(engine, player, target) < player.CargoScanDistance + target.CargoVisibleDistance)
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
