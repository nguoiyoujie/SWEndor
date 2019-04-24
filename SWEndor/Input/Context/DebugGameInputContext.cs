using MTV3D65;
using SWEndor.Input.Functions;
using SWEndor.Input.Functions.Gameplay.Special;

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
            Globals.Engine.Game.EnableAI = !Globals.Engine.Game.EnableAI;
            break;

          case (int)CONST_TV_KEY.TV_KEY_8: // Toggle Collisioj
            Globals.Engine.Game.EnableCollision = !Globals.Engine.Game.EnableCollision;
            break;

          case (int)CONST_TV_KEY.TV_KEY_9: // Toggle Perf
            Globals.Engine.Game.EnablePerf = !Globals.Engine.Game.EnablePerf;
            break;

          case (int)CONST_TV_KEY.TV_KEY_0: // Toggle Sound
            Globals.Engine.Game.EnableSound = !Globals.Engine.Game.EnableSound;
            break;

          case (int)CONST_TV_KEY.TV_KEY_L: // Test save
            GameSaver.Save(@"save.txt");
            break;

          case (int)CONST_TV_KEY.TV_KEY_K: // Test LandInfo
            Globals.Engine.LandInfo.Enabled = !Globals.Engine.LandInfo.Enabled;
            break;

          case (int)CONST_TV_KEY.TV_KEY_J: // Test AtmosphereInfo
            Globals.Engine.AtmosphereInfo.Enabled = !Globals.Engine.AtmosphereInfo.Enabled;
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
