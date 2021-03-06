﻿using SWEndor.Core;
using SWEndor.Sound;

namespace SWEndor.Input.Functions.Gameplay.Weapon
{
  public class PrevPrimary : InputFunction
  {
    private int _key = -1;
    public static string InternalName = "g_weap1mode-";
    public override int Key { get { return _key; } set { _key = value; } }
    public override string Name { get { return InternalName; } }
    public override InputOptions Options { get { return InputOptions.ONPRESS; } }

    public override void Process(Engine engine)
    {
      engine.PlayerInfo.PrevPrimaryWeapon();
      engine.SoundManager.SetSound(SoundGlobals.Button1);
    }
  }
}
