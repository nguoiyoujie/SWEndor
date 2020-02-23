using Primitives.FileFormat.INI;

namespace SWEndor.Models
{
  internal struct ExplodeSystemData
  {
    private const string sExplode = "Explode";
    private static ExplodeData[] NullExpl = new ExplodeData[0];

    [INISubSectionList(sExplode, "EXP", "Explodes")]
    internal ExplodeData[] Explodes;

    public static ExplodeSystemData Default { get { return new ExplodeSystemData(NullExpl); } }

    public ExplodeSystemData(ExplodeData[] expl)
    {
      Explodes = expl;
    }
  }
}
