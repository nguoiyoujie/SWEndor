using MTV3D65;
using SWEndor.Game.Scenarios;

namespace SWEndor
{
  public class ShipSpawnEventArg
  {
    public readonly GSFunctions.ShipSpawnInfo Info;
    public readonly TV_3DVECTOR Position;
    public readonly TV_3DVECTOR TargetPosition;
    public readonly TV_3DVECTOR FacingPosition;

    public ShipSpawnEventArg(GSFunctions.ShipSpawnInfo info, TV_3DVECTOR position, TV_3DVECTOR targetposition, TV_3DVECTOR facingposition)
    {
      Info = info;
      Position = position;
      TargetPosition = targetposition;
      FacingPosition = facingposition;
    }
  }
}
