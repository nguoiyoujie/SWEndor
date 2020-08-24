using Primrose.FileFormat.INI;

namespace SWEndor.Game.Weapons
{
  internal struct WeapLoadInfo
  {
    [INIValue]
    public int[] Primary;

    [INIValue]
    public int[] Secondary;

    [INIValue]
    public int[] AI;

    public static WeapLoadInfo Default = new WeapLoadInfo
    {
      Primary = new int[0],
      Secondary = new int[0],
      AI = new int[0]
    };
  }
}
