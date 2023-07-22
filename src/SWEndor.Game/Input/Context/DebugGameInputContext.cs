using SWEndor.Game.Input.Functions;
using SWEndor.Game.Input.Functions.Gameplay;
using SWEndor.Game.Input.Functions.Gameplay.Special;
using SWEndor.Game.Input.Functions.Utility.Game;
using SWEndor.Game.Input.Functions.Utility.Screen;

namespace SWEndor.Game.Input.Context
{
  public class DebugGameInputContext : GameInputContext
  {
    public DebugGameInputContext(InputManager manager) : base(manager) { }

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
      TogglePlayerAI.InternalName,
      ToggleFreeMode.InternalName,

      SaveSnapshot.InternalName
    };

    public override void Set()
    {
      base.Set();
      foreach (string s in _dbg_functions)
      {
        int index = -1;
        while ((index = InputFunction.Registry.GetNext(s, ++index, out InputFunction fn)) != -1)
        {
          if (fn != null)
            fn.Enabled = true;
        }
      }
    }

    /*
    public override void HandleKeyBuffer(TV_KEYDATA keydata)
    {
      base.HandleKeyBuffer(keydata);
      if (keydata.Pressed > 0)
      {
        // Debug and Testing // 
        switch (keydata.Key)
        {
          case (int)CONST_TV_KEY.TV_KEY_7: // Toggle AI
            Globals.Engine.Game.EnableAI = !Globals.Engine.Game.EnableAI;
            break;

          case (int)CONST_TV_KEY.TV_KEY_8: // Toggle Collision
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
    }
    */
  }
}
