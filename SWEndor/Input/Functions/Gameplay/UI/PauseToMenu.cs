using MTV3D65;
using SWEndor.UI.Menu.Pages;

namespace SWEndor.Input.Functions.Gameplay.UI
{
  public class PauseToMenu : InputFunction
  {
    private int _key = (int)CONST_TV_KEY.TV_KEY_ESCAPE;
    public static string InternalName = "";
    public override int Key { get { return _key; } set { _key = value; } }
    public override string Name { get { return InternalName; } }
    public override InputOptions Options { get { return InputOptions.ONPRESS; } }

    public override void Process()
    {
      if (!Screen2D.Instance().ShowPage)
      {
        Screen2D.Instance().CurrentPage = new PauseMenu(); // configurable?
        Screen2D.Instance().ShowPage = true;
        Game.Instance().IsPaused = true;
      }
    }
  }
}
