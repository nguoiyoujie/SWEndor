﻿using MTV3D65;
using SWEndor.Player;

namespace SWEndor.Input.Functions.Gameplay
{
  public class MoveCameraDownward : InputFunction
  {
    private int _key = (int)CONST_TV_KEY.TV_KEY_PAGEDOWN;
    public static string InternalName = "";
    public override int Key { get { return _key; } set { _key = value; } }
    public override string Name { get { return InternalName; } }
    public override InputOptions Options { get { return InputOptions.WHILEPRESSED; } }

    public override void Process(InputManager manager)
    {
      if (manager.Engine.PlayerCameraInfo.CameraMode == CameraMode.FREEMODE)
      {
        float rate = manager.Engine.InputManager.SHIFT ? 2500 : 500;
        TVCamera tvc = manager.Engine.PlayerCameraInfo.Camera;
        rate *= manager.Engine.Game.TimeControl.RenderInterval;
        tvc.MoveRelative(0, -rate, 0);
      }
    }
  }
}
