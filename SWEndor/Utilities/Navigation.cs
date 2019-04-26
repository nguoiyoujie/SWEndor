using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.Scenarios;
using SWEndor.UI;
using SWEndor.UI.Menu;

namespace SWEndor
{
  public static class Navigation
  {
    public static Engine GetEngine(this ActorInfo a)
    {
      return a.FactoryOwner.Engine;
    }

    public static Engine GetEngine(this ActorTypeInfo at)
    {
      return at.FactoryOwner.Engine;
    }

    public static Engine GetEngine(this GameScenarioBase gsb)
    {
      return gsb.Manager.Engine;
    }

    public static Engine GetEngine(this Widget widget)
    {
      return widget.Owner.Engine;
    }

    public static Engine GetEngine(this Page page)
    {
      return page.Owner.Engine;
    }
  }
}
