namespace SWEndor.Input.Functions.Gameplay.Weapon
{
  public class PrevSecondary : InputFunction
  {
    private int _key = -1;
    public static string InternalName = "g_weap2mode-";
    public override int Key { get { return _key; } set { _key = value; } }
    public override string Name { get { return InternalName; } }
    public override InputOptions Options { get { return InputOptions.ONPRESS; } }

    public override void Process(Engine engine)
    {
      engine.PlayerInfo.PrevSecondaryWeapon();
      engine.SoundManager.SetSound("button_1");
    }
  }
}
