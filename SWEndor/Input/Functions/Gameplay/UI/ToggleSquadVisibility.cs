using MTV3D65;

namespace SWEndor.Input.Functions.Gameplay.UI
{
  public class ToggleSquadVisibility : InputFunction
  {
    private int _key = (int)CONST_TV_KEY.TV_KEY_W;
    public static string InternalName = "g_squad_toggle";
    public override int Key { get { return _key; } set { _key = value; } }
    public override string Name { get { return InternalName; } }
    public override InputOptions Options { get { return InputOptions.ONPRESS; } }

    public override void Process(Engine engine)
    {
      Globals.Engine.Screen2D.ShowSquad = !Globals.Engine.Screen2D.ShowSquad;
      Globals.Engine.SoundManager.SetSound("button_1");
    }
  }
}
