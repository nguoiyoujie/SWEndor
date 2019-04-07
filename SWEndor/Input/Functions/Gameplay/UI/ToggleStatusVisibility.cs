﻿using MTV3D65;
using SWEndor.Sound;

namespace SWEndor.Input.Functions.Gameplay.UI
{
  public class ToggleStatusVisibility : InputFunction
  {
    private int _key = (int)CONST_TV_KEY.TV_KEY_Y;
    public static string InternalName = "g_ui_status_toggle";
    public override int Key { get { return _key; } set { _key = value; } }
    public override string Name { get { return InternalName; } }
    public override InputOptions Options { get { return InputOptions.ONPRESS; } }

    public override void Process()
    {
      Screen2D.Instance().ShowStatus = !Screen2D.Instance().ShowStatus;
      SoundManager.Instance().SetSound("button_1");
    }
  }
}