﻿using MTV3D65;
using SWEndor.Player;
using SWEndor.Sound;

namespace SWEndor.Input.Functions.Gameplay.Weapon
{
  public class NextSecondary : InputFunction
  {
    private int _key = (int)CONST_TV_KEY.TV_KEY_X;
    public static string InternalName = "g_weap2mode+";
    public override int Key { get { return _key; } set { _key = value; } }
    public override string Name { get { return InternalName; } }
    public override InputOptions Options { get { return InputOptions.ONPRESS; } }

    public override void Process()
    {
      Globals.Engine.PlayerInfo.NextSecondaryWeapon();
      Globals.Engine.SoundManager.SetSound("button_1");
    }
  }
}
