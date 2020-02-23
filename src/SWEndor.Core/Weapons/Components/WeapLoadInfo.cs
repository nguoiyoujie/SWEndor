using Primitives.FileFormat.INI;

namespace SWEndor.Weapons
{
  internal struct WeapLoadInfo
  {
    private const string sNone = "";

    [INIValue(sNone, "Primary")]
    public int[] Primary;

    [INIValue(sNone, "Secondary")]
    public int[] Secondary;

    [INIValue(sNone, "AI")]
    public int[] AI;

    public static WeapLoadInfo Default = new WeapLoadInfo
    {
      Primary = new int[0],
      Secondary = new int[0],
      AI = new int[0]
    };
  }
}
