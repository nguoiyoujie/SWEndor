using SWEndor.Actors;
using SWEndor.Core;
using Primitives.FileFormat.INI;

namespace SWEndor.ActorTypes.Components
{
  internal struct AddOnSystemData
  {
    private static AddOnData[] NullAddon = new AddOnData[0];
    internal AddOnData[] AddOns;

    public static AddOnSystemData Default { get { return new AddOnSystemData(NullAddon); } }

    public AddOnSystemData(AddOnData[] addons)
    {
      AddOns = addons;
    }

    public void Create(Engine e, ActorInfo a)
    {
      foreach (AddOnData addon in AddOns)
        addon.Create(e, a);
    }

    public void LoadFromINI(INIFile f, string sectionname)
    {
      AddOnData.LoadFromINI(f, sectionname, "AddOns", out AddOns);
    }

    public void SaveToINI(INIFile f, string sectionname)
    {
      AddOnData.SaveToINI(f, sectionname, "AddOns", "ADD", AddOns);
    }
  }
}
