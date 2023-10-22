using Primrose.FileFormat.INI;

namespace SWEndor.Game.ActorTypes.Components
{
  internal struct CargoData
  {
    [INIValue]
    public float CargoVisibleDistance;

    //[INIValue]
    //public float CargoScanDistance;

    public readonly static CargoData Default =
      new CargoData
      {
        CargoVisibleDistance = 200,
      };
  }
}
