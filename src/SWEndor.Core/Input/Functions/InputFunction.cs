﻿using SWEndor.Core;
using SWEndor.Input.Functions.Gameplay;
using SWEndor.Input.Functions.Gameplay.Camera;
using SWEndor.Input.Functions.Gameplay.Special;
using SWEndor.Input.Functions.Gameplay.Speed;
using SWEndor.Input.Functions.Gameplay.UI;
using SWEndor.Input.Functions.Gameplay.Weapon;
using SWEndor.Input.Functions.Utility.Screen;
using System;
using System.Collections.Generic;

namespace SWEndor.Input.Functions
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
    WHILEPRESSED = 0x02
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
      private static InputFunction[] fns = new InputFunction[0];

      public static void ProcessOnPress(Engine engine, int key)
      {
        InputFunction fn = Get(key);
        if (fn != null
          && fn.Enabled
          && fn.Options.Has(InputOptions.ONPRESS))
          fn.Process(engine);
      }

      public static void ProcessWhilePressed(Engine engine, byte[] keyPressedStates)
      {
        int i = 0;
        while (i < fns.Length)
        {
          InputFunction fn = fns[i];
          if (fn != null
            && fn.Enabled
            && fn.Options.Has(InputOptions.WHILEPRESSED)
            && fn.Key >= byte.MinValue 
            && fn.Key < byte.MaxValue 
            && keyPressedStates[fn.Key] != 0)
            fn.Process(engine);
          i++;
        }
      }

      public static InputFunction[] Functions
      {
        get
        {
          return fns;
        }
      }

      public static InputFunction Get(string name)
      {
        int i = 0;
        while (i < fns.Length)
        {
          InputFunction fn = fns[i];
          if (fn != null
            && fn.Name == name)
            return fn;
          i++;
        }
        return null;
      }

      public static InputFunction Get(int key)
      {
        int i = 0;
        while (i < fns.Length)
        {
          InputFunction fn = fns[i];
          if (fn != null
            && fn.Key == key)
            return fn;
          i++;
        }
        return null;
      }

      public static void GenerateDefault()
      {
        // hardcoded...
        List<InputFunction> f = new List<InputFunction>(100);

        f.Add(new PauseToMenu());
        f.Add(new ShowMap());

        // gameplay
        f.Add(new Up());
        f.Add(new Down());
        f.Add(new NextPrimary());
        f.Add(new PrevPrimary());
        f.Add(new NextSecondary());
        f.Add(new PrevSecondary());
        f.Add(new ToggleLockOn());
        f.Add(new SquadCommand());
        f.Add(new SquadCancelCommand());

        // gameplay: camera
        f.Add(new NextCameraMode());
        f.Add(new PrevCameraMode());
        f.Add(new ToggleFreeMode());
        f.Add(new MoveCameraForward());
        f.Add(new MoveCameraBackward());
        f.Add(new MoveCameraUpward());
        f.Add(new MoveCameraDownward());
        f.Add(new MoveCameraLeftward());
        f.Add(new MoveCameraRightward());

        // gameplay: UI toggles
        f.Add(new ToggleUIVisibility());
        f.Add(new ToggleSquadVisibility());
        f.Add(new ToggleStatusVisibility());
        f.Add(new ToggleRadarVisibility());
        f.Add(new ToggleScoreVisibility());

        // gameply: debug
        f.Add(new AddLife());
        f.Add(new AllEnemiesDead());
        f.Add(new AllEnemiesDying());
        f.Add(new TimeFast());
        f.Add(new TimeSlow());
        f.Add(new TimeReset());
        f.Add(new TimeJump());
        f.Add(new ToggleCameraStates());
        f.Add(new ToggleMovementLock());
        f.Add(new TogglePlayerAI());

        // utility: screenshot
        f.Add(new SaveScreenshot());

        fns = f.ToArray();
        
      }
    }
  }
}
