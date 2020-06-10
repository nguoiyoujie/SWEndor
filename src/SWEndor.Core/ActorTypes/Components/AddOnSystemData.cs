using SWEndor.Actors;
using SWEndor.Core;
using Primitives.FileFormat.INI;

namespace SWEndor.ActorTypes.Components
{
  internal struct AddOnSystemData
  {
    private static AddOnData[] NullAddon = new AddOnData[0];

    [INISubSectionList(SubsectionPrefix = "ADD")]
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
  }
}
