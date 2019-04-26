﻿using MTV3D65;

namespace SWEndor.Input.Functions.Gameplay.UI
{
  public class ToggleRadarVisibility : InputFunction
  {
    private int _key = (int)CONST_TV_KEY.TV_KEY_R;
    public static string InternalName = "g_ui_radar_toggle";
    public override int Key { get { return _key; } set { _key = value; } }
    public override string Name { get { return InternalName; } }
    public override InputOptions Options { get { return InputOptions.ONPRESS; } }

    public override void Process(InputManager manager)
    {
      Globals.Engine.Screen2D.ShowRadar = !Globals.Engine.Screen2D.ShowRadar;
      Globals.Engine.SoundManager.SetSound("button_1");
    }
  }
}
