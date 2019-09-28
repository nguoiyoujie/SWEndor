using MTV3D65;
using SWEndor.Core;
using SWEndor.UI.Menu.Pages;

namespace SWEndor.Input.Functions.Gameplay.UI
{
  public class ShowMap : InputFunction
  {
    private int _key = (int)CONST_TV_KEY.TV_KEY_SLASH;
    public static string InternalName = "showmap";
    public override int Key { get { return _key; } set { _key = value; } }
    public override string Name { get { return InternalName; } }
    public override InputOptions Options { get { return InputOptions.ONPRESS; } }

    public override void Process(Engine engine)
    {
      engine.Screen2D.CurrentPage = new ScenarioMap(engine.Screen2D, engine.GameScenarioManager.Scenario);
      Globals.Engine.Screen2D.ShowPage = true;
    }
  }
}
