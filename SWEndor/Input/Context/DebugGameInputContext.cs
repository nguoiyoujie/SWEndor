using MTV3D65;
using SWEndor.Input.Functions;
using SWEndor.Input.Functions.Gameplay;
using SWEndor.Input.Functions.Gameplay.Camera;
using SWEndor.Input.Functions.Gameplay.Special;
using SWEndor.Input.Functions.Gameplay.Speed;
using SWEndor.Input.Functions.Gameplay.UI;
using SWEndor.Input.Functions.Gameplay.Weapon;

namespace SWEndor.Input.Context
{
  public class DebugGameInputContext : GameInputContext
  {
    public static string[] _dbg_functions = new string[]
    {
      AddLife.InternalName,
      AllEnemiesDead.InternalName,
      AllEnemiesDying.InternalName,
      TimeFast.InternalName,
      TimeSlow.InternalName,
      TimeReset.InternalName,
      TimeJump.InternalName,

      ToggleCameraStates.InternalName,
      ToggleMovementLock.InternalName,
      TogglePlayerAI.InternalName
    };

    public override void Set()
    {
      base.Set();
      foreach (string s in _dbg_functions)
      {
        InputFunction fn = InputFunction.Registry.Get(s);
        if (fn != null)
          fn.Enabled = true;
      }
    }


    public override void HandleKeyBuffer(TV_KEYDATA keydata)
    {
      base.HandleKeyBuffer(keydata);

      /*
      if (keydata.Pressed > 0)
      {
        // Debug and Testing // 
        switch (keydata.Key)
        {
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

          case (int)CONST_TV_KEY.TV_KEY_L: // Test save
            GameSaver.Save(@"save.txt");
            break;

          case (int)CONST_TV_KEY.TV_KEY_K: // Test LandInfo
            LandInfo.Instance().Enabled = !LandInfo.Instance().Enabled;
            break;

          case (int)CONST_TV_KEY.TV_KEY_J: // Test AtmosphereInfo
            AtmosphereInfo.Instance().Enabled = !AtmosphereInfo.Instance().Enabled;
            break;
        }
      }
      */
    }

    public override void HandleKeyState(byte[] keyPressedStates)
    {
      base.HandleKeyState(keyPressedStates);
    }

    public override void HandleMouse(int mouseX, int mouseY, bool button1, bool button2, bool button3, bool button4, int mouseScroll)
    {
      base.HandleMouse(mouseX, mouseY, button1, button2, button3, button4, mouseScroll);
    }
  }
}
