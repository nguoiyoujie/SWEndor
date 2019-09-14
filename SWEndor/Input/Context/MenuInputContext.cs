using MTV3D65;
using SWEndor.Actors;
using SWEndor.Terminal;
using System.Collections.Generic;

namespace SWEndor.Input.Context
{
  public class MenuInputContext : AInputContext
  {
    public MenuInputContext(InputManager manager) : base(manager) { }

    public override void HandleKeyBuffer(TV_KEYDATA keydata)
    {
      if (keydata.Pressed > 0)
      {
        if (Engine.Screen2D.CurrentPage?.OnKeyPress((CONST_TV_KEY)keydata.Key) ?? false)
          Engine.SoundManager.SetSound("button_1");

        // Terminal
        if (Engine.InputManager.CTRL && keydata.Key.Equals((int)CONST_TV_KEY.TV_KEY_T))
          TConsole.Visible = true;

        // Terminal
        if (keydata.Key.Equals((int)CONST_TV_KEY.TV_KEY_1))
        {
          Engine.SoundManager.QueueInterruptMusic("dynamic\\S-EMP-SM", 1657);
          Engine.SoundManager.SetSound("button_1");
        }

        if (keydata.Key.Equals((int)CONST_TV_KEY.TV_KEY_2))
        {
          Engine.SoundManager.QueueInterruptMusic("dynamic\\S-EMP-LG", 1657);
          Engine.SoundManager.SetSound("button_1");
        }
        
        if (keydata.Key.Equals((int)CONST_TV_KEY.TV_KEY_3))
        {
          Engine.SoundManager.QueueInterruptMusic("dynamic\\S-REB-SM", 1657);
          Engine.SoundManager.SetSound("button_1");
        }

        if (keydata.Key.Equals((int)CONST_TV_KEY.TV_KEY_4))
        {
          Engine.SoundManager.QueueInterruptMusic("dynamic\\S-REB-LG", 1657);
          Engine.SoundManager.SetSound("button_1");
        }

        if (keydata.Key.Equals((int)CONST_TV_KEY.TV_KEY_5))
        {
          Engine.SoundManager.QueueInterruptMusic("dynamic\\S-NEU-SM", 1657);
          Engine.SoundManager.SetSound("button_1");
        }

        if (keydata.Key.Equals((int)CONST_TV_KEY.TV_KEY_6))
        {
          Engine.SoundManager.QueueInterruptMusic("dynamic\\S-NEU-LG", 1657);
          Engine.SoundManager.SetSound("button_1");
        }

        if (keydata.Key.Equals((int)CONST_TV_KEY.TV_KEY_7))
        {
          string[] win = new string[] { "dynamic\\S-WIN-1", "dynamic\\S-WIN-2", "dynamic\\S-WIN-3", "dynamic\\S-WIN-4" };
          Engine.SoundManager.QueueInterruptMusic(win[Globals.Engine.Random.Next(0, win.Length)], 1657);
          Engine.SoundManager.SetSound("button_1");
        }
        
        if (keydata.Key.Equals((int)CONST_TV_KEY.TV_KEY_8))
        {
          Engine.SoundManager.QueueInterruptMusic("dynamic\\S-WIN-LG", 1657);
          Engine.SoundManager.SetSound("button_1");
        }

        if (keydata.Key.Equals((int)CONST_TV_KEY.TV_KEY_9))
        {
          List<int> list = new List<int>(Globals.Engine.GameScenarioManager.Scenario.MainEnemyFaction.GetWings());
          list.AddRange(Globals.Engine.GameScenarioManager.Scenario.MainAllyFaction.GetWings());

          foreach (int i in list)
          {
            ActorInfo a = Engine.ActorFactory.Get(i);
            a?.Delete();
          }
        }
      }
    }
  }
}
