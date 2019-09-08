using MTV3D65;
using SWEndor.UI.Menu.Pages;

namespace SWEndor.Input.Functions.Gameplay.UI
{
  public class PauseToMenu : InputFunction
  {
    private int _key = (int)CONST_TV_KEY.TV_KEY_ESCAPE;
    public static string InternalName = "pause";
    public override int Key { get { return _key; } set { _key = value; } }
    public override string Name { get { return InternalName; } }
    public override InputOptions Options { get { return InputOptions.ONPRESS; } }

    public override void Process(Engine engine)
    {
      if (!engine.Screen2D.ShowPage)
      {
        engine.Screen2D.CurrentPage = new PauseMenu(engine.Screen2D); // configurable?
        engine.Screen2D.ShowPage = true;
        engine.Game.IsPaused = true;
      }
    }
  }
}
