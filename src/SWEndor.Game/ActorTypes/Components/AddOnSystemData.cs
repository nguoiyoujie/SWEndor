using SWEndor.Game.Actors;
using SWEndor.Game.Core;
using Primrose.FileFormat.INI;

namespace SWEndor.Game.ActorTypes.Components
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
