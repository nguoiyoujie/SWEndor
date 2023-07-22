using SWEndor.Game.Core;
using SWEndor.Game.Input.Functions.Gameplay;
using SWEndor.Game.Input.Functions.Gameplay.Camera;
using SWEndor.Game.Input.Functions.Gameplay.Special;
using SWEndor.Game.Input.Functions.Gameplay.Speed;
using SWEndor.Game.Input.Functions.Gameplay.UI;
using SWEndor.Game.Input.Functions.Gameplay.Weapon;
using SWEndor.Game.Input.Functions.Utility;
using SWEndor.Game.Input.Functions.Utility.Game;
using SWEndor.Game.Input.Functions.Utility.Screen;
using System;
using System.Collections.Generic;

namespace SWEndor.Game.Input.Functions
{
  /// <summary>
  /// Input option types
  /// </summary>
  [Flags]
  public enum InputOptions
  {
    /// <summary>This option does nothing</summary>
    NONE = 0,

    /// <summary>Triggers once on a key press</summary>
    ONPRESS = 0x01,

    /// <summary>Triggers every tick while the key remains in the pressed state</summary>
    WHILEPRESSED = 0x02,

    /// <summary>Is valid only when CTRL state is also active</summary>
    CTRL = 0x04,

    /// <summary>Is valid only when SHIFT state is also active</summary>
    SHIFT = 0x08,

    /// <summary>Is valid only when ALT state is also active</summary>
    ALT = 0x10,
  }

  /// <summary>
  /// Provides extension methods for InputOptions enum
  /// </summary>
  public static class InputOptionsExt
  {
    /// <summary>Returns whether an input option contains a subset</summary>
    /// <param name="mask">The mask to compare</param>
    /// <param name="subset">The subset to compare</param>
    /// <returns>Returns true if the mask contains all bits of the subset, false if otherwise</returns>
    public static bool Has(this InputOptions mask, InputOptions subset) { return (mask & subset) == subset; }
  }

  public abstract class InputFunction
  {
    public abstract string Name { get; }
    public abstract int Key { get; set; }
    public abstract InputOptions Options { get; }
    public bool Enabled { get; set; }
    public virtual void Process(Engine engine) { }

    public static class Registry
    {
      private readonly static List<InputFunction> fns = new List<InputFunction>();

      public static void ProcessOnPress(Engine engine, int key)
      {
        int index = -1;
        while ((index = GetNext(key, ++index, out InputFunction fn)) != -1)
        {
          if (fn != null
            && fn.Enabled
            && fn.Options.Has(InputOptions.ONPRESS)
            && (engine.InputManager.CTRL || !fn.Options.Has(InputOptions.CTRL))
            && (engine.InputManager.SHIFT || !fn.Options.Has(InputOptions.SHIFT))
            && (engine.InputManager.ALT || !fn.Options.Has(InputOptions.ALT))
            )
            fn.Process(engine);
        }
      }

      public static void ProcessWhilePressed(Engine engine, byte[] keyPressedStates)
      {
        int i = 0;
        while (i < fns.Count)
        {
          InputFunction fn = fns[i];
          if (fn != null
            && fn.Enabled
            && fn.Options.Has(InputOptions.WHILEPRESSED)
            && (engine.InputManager.CTRL || !fn.Options.Has(InputOptions.CTRL))
            && (engine.InputManager.SHIFT || !fn.Options.Has(InputOptions.SHIFT))
            && (engine.InputManager.ALT || !fn.Options.Has(InputOptions.ALT))
            && fn.Key >= byte.MinValue 
            && fn.Key < byte.MaxValue 
            && keyPressedStates[fn.Key] != 0
            )
            fn.Process(engine);
          i++;
        }
      }

      public static List<InputFunction> Functions
      {
        get
        {
          return fns;
        }
      }

      public static int GetNext(string name, int index, out InputFunction fn)
      {
        while (index < fns.Count)
        {
          fn = fns[index];
          if (fn != null
            && fn.Name == name)
            return index;
          index++;
        }
        fn = null;
        return -1;
      }

      public static int GetNext(int key, int index, out InputFunction fn)
      {
        while (index < fns.Count)
        {
          fn = fns[index];
          if (fn != null
            && fn.Key == key)
            return index;
          index++;
        }
        fn = null;
        return -1;
      }

      public static void GenerateDefault()
      {
        // hardcoded...
        fns.AddRange(new List<InputFunction>(100)
        {
          new PauseToMenu(),
          new ShowMap(),

          // gameplay
          new Up(),
          new Down(),
          new NextPrimary(),
          new PrevPrimary(),
          new NextSecondary(),
          new PrevSecondary(),
          new ToggleLockOn(),
          new SquadCommand(),
          new SquadCancelCommand(),

          // gameplay: camera
          new NextCameraMode(),
          new PrevCameraMode(),
          new ToggleFreeMode(),
          new MoveCameraForward(),
          new MoveCameraBackward(),
          new MoveCameraUpward(),
          new MoveCameraDownward(),
          new MoveCameraLeftward(),
          new MoveCameraRightward(),

          // gameplay: UI toggles
          new ToggleUIVisibility(),
          new ToggleSquadVisibility(),
          new ToggleStatusVisibility(),
          new ToggleRadarVisibility(),
          new ToggleScoreVisibility(),

          // gameply: debug
          new AddLife(),
          new AllEnemiesDead(),
          new AllEnemiesDying(),
          new TimeFast(),
          new TimeSlow(),
          new TimeReset(),
          new TimeJump(),
          new ToggleCameraStates(),
          new ToggleMovementLock(),
          new TogglePlayerAI(),

          // utility: screenshot
          new SaveScreenshot(),
          new SaveSnapshot(),

          // terminal
          new OpenTerminal(),
        });
      }
    }
  }
}
