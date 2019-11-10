﻿using MTV3D65;
using SWEndor.Core;
using SWEndor.Player;

namespace SWEndor.Input.Functions.Gameplay
{
  public class MoveCameraUpward : InputFunction
  {
    private int _key = (int)CONST_TV_KEY.TV_KEY_PAGEUP;
    public static string InternalName = "c_moveup";
    public override int Key { get { return _key; } set { _key = value; } }
    public override string Name { get { return InternalName; } }
    public override InputOptions Options { get { return InputOptions.WHILEPRESSED; } }

    public override void Process(Engine engine)
    {
      if (engine.PlayerCameraInfo.CameraMode == CameraMode.FREEMODE)
      {
        float rate = engine.InputManager.SHIFT ? 2500 : 500;
        TVCamera tvc = engine.PlayerCameraInfo.Camera;
        rate *= engine.Game.TimeControl.UpdateInterval;
        tvc.MoveRelative(0, rate, 0);
      }
    }
  }
}
