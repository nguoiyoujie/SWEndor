using SWEndor.Input.Functions.Gameplay;
using SWEndor.Input.Functions.Gameplay.Camera;
using SWEndor.Input.Functions.Gameplay.Special;
using SWEndor.Input.Functions.Gameplay.Speed;
using SWEndor.Input.Functions.Gameplay.UI;
using SWEndor.Input.Functions.Gameplay.Weapon;
using System;
using System.Collections.Generic;

namespace SWEndor.Input.Functions
{
  [Flags]
  public enum InputOptions
  {
    NONE = 0,
    ONPRESS = 0x01,
    WHILEPRESSED = 0x02
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
          && fn.Options.HasFlag(InputOptions.ONPRESS))
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
            && fn.Options.HasFlag(InputOptions.WHILEPRESSED)
            && fn.Key >= byte.MinValue 
            && fn.Key < byte.MaxValue 
            && keyPressedStates[fn.Key] != 0)
            fn.Process(engine);
          i++;
        }
      }

      public static InputFunction[] GetList()
      {
        List<InputFunction> ret = new List<InputFunction>();
        int i = 0;
        while (i < fns.Length)
        {
          InputFunction fn = fns[i];
          if (fn != null)
            ret.Add(fn);
          i++;
        }
        return ret.ToArray();
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
        fns = new InputFunction[100];

        int i = 0;

        fns[i++] = new PauseToMenu();
        fns[i++] = new ShowMap();

        // gameplay
        fns[i++] = new Up();
        fns[i++] = new Down();
        fns[i++] = new NextPrimary();
        fns[i++] = new PrevPrimary();
        fns[i++] = new NextSecondary();
        fns[i++] = new PrevSecondary();
        fns[i++] = new RemoveLockOn();
        fns[i++] = new SquadCommand();
        fns[i++] = new SquadCancelCommand();

        // gameplay: camera
        fns[i++] = new NextCameraMode();
        fns[i++] = new PrevCameraMode();
        fns[i++] = new MoveCameraForward();
        fns[i++] = new MoveCameraBackward();
        fns[i++] = new MoveCameraUpward();
        fns[i++] = new MoveCameraDownward();
        fns[i++] = new MoveCameraLeftward();
        fns[i++] = new MoveCameraRightward();

        // gameplay: UI toggles
        fns[i++] = new ToggleUIVisibility();
        fns[i++] = new ToggleSquadVisibility();
        fns[i++] = new ToggleStatusVisibility();
        fns[i++] = new ToggleRadarVisibility();
        fns[i++] = new ToggleScoreVisibility();

        // gameply: debug
        fns[i++] = new AddLife();
        fns[i++] = new AllEnemiesDead();
        fns[i++] = new AllEnemiesDying();
        fns[i++] = new TimeFast();
        fns[i++] = new TimeSlow();
        fns[i++] = new TimeReset();
        fns[i++] = new TimeJump();
        fns[i++] = new ToggleCameraStates();
        fns[i++] = new ToggleMovementLock();
        fns[i++] = new TogglePlayerAI();
      }
    }
  }
}
