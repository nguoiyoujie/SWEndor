using MTV3D65;
using SWEndor.Actors;
using SWEndor.Scenarios;

namespace SWEndor.Input.Context
{
  public class DebugGameInputContext : GameInputContext
  {
    public override void HandleKeyBuffer(TV_KEYDATA keydata)
    {
      base.HandleKeyBuffer(keydata);

      if (keydata.Pressed > 0)
      {
        // Debug and Testing
        switch (keydata.Key)
        {
          case (int)CONST_TV_KEY.TV_KEY_4: // GIve life to player
            PlayerInfo.Instance().Lives++;
            break;

          case (int)CONST_TV_KEY.TV_KEY_5: // Destroy all enemies
            if (GameScenarioManager.Instance().Scenario != null)
              foreach (ActorInfo ainfo in GameScenarioManager.Instance().Scenario.MainEnemyFaction.GetAll())
                ainfo.ActorState = ActorState.DEAD;
            break;

          case (int)CONST_TV_KEY.TV_KEY_7: // Toggle AI
            Game.Instance().EnableAI = !Game.Instance().EnableAI;
            break;

          case (int)CONST_TV_KEY.TV_KEY_8: // Toggle Collisioj
            Game.Instance().EnableCollision = !Game.Instance().EnableCollision;
            break;

          case (int)CONST_TV_KEY.TV_KEY_9: // Toggle Perf
            Game.Instance().EnablePerf = !Game.Instance().EnablePerf;
            break;

          case (int)CONST_TV_KEY.TV_KEY_0: // Toggle Sound
            Game.Instance().EnableSound = !Game.Instance().EnableSound;
            break;

          case (int)CONST_TV_KEY.TV_KEY_N: // Test movement control lock
            PlayerInfo.Instance().IsMovementControlsEnabled = !PlayerInfo.Instance().IsMovementControlsEnabled;
            break;

          case (int)CONST_TV_KEY.TV_KEY_M: // Test time skip
            Game.Instance().AddTime = 5;
            break;

          case (int)CONST_TV_KEY.TV_KEY_L: // Test save
            GameSaver.Save(@"save.txt");
            break;

          case (int)CONST_TV_KEY.TV_KEY_P: // Test Player AI
            PlayerInfo.Instance().PlayerAIEnabled = !PlayerInfo.Instance().PlayerAIEnabled;
            break;

          case (int)CONST_TV_KEY.TV_KEY_K: // Test LandInfo
            LandInfo.Instance().Enabled = !LandInfo.Instance().Enabled;
            break;

          case (int)CONST_TV_KEY.TV_KEY_J: // Test AtmosphereInfo
            AtmosphereInfo.Instance().Enabled = !AtmosphereInfo.Instance().Enabled;
            break;

          case (int)CONST_TV_KEY.TV_KEY_H: // Force switch of camera mode
            if (PlayerCameraInfo.Instance().CameraMode == CameraMode.FREEMODE) // last camera mode
              PlayerCameraInfo.Instance().CameraMode = 0;
            else
              PlayerCameraInfo.Instance().CameraMode++;
            break;
        }
      }
    }

    public override void HandleKeyState(byte[] keyPressedStates)
    {
      base.HandleKeyState(keyPressedStates);

      // Game speed
      if (keyPressedStates[(int)CONST_TV_KEY.TV_KEY_MINUS] != 0)
      {
        Game.Instance().TimeControl.SpeedModifier *= 0.9f;
        Screen2D.Instance().MessageSecondaryText(string.Format("DEV: TIMEMULT = {0:0.00}", Game.Instance().TimeControl.SpeedModifier)
                                                        , 1.5f
                                                        , new TV_COLOR(0.5f, 0.5f, 1, 1)
                                                        , 99);
        Utilities.Clamp(ref Game.Instance().TimeControl.SpeedModifier, 0.01f, 100);
      }
      if (keyPressedStates[(int)CONST_TV_KEY.TV_KEY_EQUALS] != 0)
      {
        Game.Instance().TimeControl.SpeedModifier /= 0.9f;
        Screen2D.Instance().MessageSecondaryText(string.Format("DEV: TIMEMULT = {0:0.00}", Game.Instance().TimeControl.SpeedModifier)
                                                , 1.5f
                                                , new TV_COLOR(0.5f, 0.5f, 1, 1)
                                                , 99);
        Utilities.Clamp(ref Game.Instance().TimeControl.SpeedModifier, 0.01f, 100);
      }
      if (keyPressedStates[(int)CONST_TV_KEY.TV_KEY_BACKSPACE] != 0)
      {
        Game.Instance().TimeControl.SpeedModifier = 1;
        Screen2D.Instance().MessageSecondaryText(string.Format("DEV: TIMEMULT = {0:0.00}", Game.Instance().TimeControl.SpeedModifier)
                                                , 1.5f
                                                , new TV_COLOR(0.5f, 0.5f, 1, 1)
                                                , 99);
      }
    }

    public override void HandleMouse(int mouseX, int mouseY, bool button1, bool button2, bool button3, bool button4, int mouseScroll)
    {
      base.HandleMouse(mouseX, mouseY, button1, button2, button3, button4, mouseScroll);
    }
  }
}
