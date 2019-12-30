using SWEndor.FileFormat.INI;

namespace SWEndor.Models
{
  internal struct ExplodeSystemData
  {
    private static ExplodeData[] NullExpl = new ExplodeData[0];
    internal ExplodeData[] Explodes;

    public static ExplodeSystemData Default { get { return new ExplodeSystemData(NullExpl); } }

    public ExplodeSystemData(ExplodeData[] expl)
    {
      Explodes = expl;
    }

    public void LoadFromINI(INIFile f, string sectionname)
    {
      ExplodeData.LoadFromINI(f, sectionname, "Explodes", out Explodes);
    }

    public void SaveToINI(INIFile f, string sectionname)
    {
      ExplodeData.SaveToINI(f, sectionname, "Explodes", "EXP", Explodes);
    }
  }
}
