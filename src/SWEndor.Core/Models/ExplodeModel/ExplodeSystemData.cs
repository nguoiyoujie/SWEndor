using Primrose.FileFormat.INI;

namespace SWEndor.Models
{
  internal struct ExplodeSystemData
  {
    private static ExplodeData[] NullExpl = new ExplodeData[0];

    [INISubSectionList(SubsectionPrefix = "EXP")]
    internal ExplodeData[] Explodes;

    public static ExplodeSystemData Default { get { return new ExplodeSystemData(NullExpl); } }

    public ExplodeSystemData(ExplodeData[] expl)
    {
      Explodes = expl;
    }
  }
}
