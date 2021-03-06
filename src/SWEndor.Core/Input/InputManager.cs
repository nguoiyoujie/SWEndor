﻿using MTV3D65;
using SWEndor.Core;
using SWEndor.Input.Context;

namespace SWEndor.Input
{
  public class InputManager
  {
    private TVInputEngine INPUT_ENGINE;
    private int numkeybuffer;
    private TV_KEYDATA[] KEY_BUFFER;
    private byte[] KEY_PRESSED;
    public bool SHIFT { get; private set; }
    public bool CTRL { get; private set; }
    public bool ALT { get; private set; }
    public int MOUSE_X;
    public int MOUSE_Y;
    private bool MOUSE_B1;
    private bool MOUSE_B2;
    private bool MOUSE_B3;
    private bool MOUSE_B4;
    private int MOUSE_SCROLL_NEW;
    private AInputContext _context;

    public AInputContext Context
    {
      get { return _context; }
      set
      {
        if (_context != value)
        {
          _context = value;
          _context.Set();
        }
      }
    }

    public readonly Engine Engine;

    public TerminalGameInputContext TerminalGameInputContext;
    public MenuInputContext MenuInputContext;
    public DebugGameInputContext DebugGameInputContext;
    public GameInputContext GameInputContext;


    internal InputManager(Engine engine)
    {
      Engine = engine;
      INPUT_ENGINE = new TVInputEngine();
      INPUT_ENGINE.Initialize(true, true);
      numkeybuffer = 0;
      KEY_BUFFER = new TV_KEYDATA[256];
      KEY_PRESSED = new byte[256];
      MOUSE_X = -99;
      MOUSE_Y = -99;
      MOUSE_B1 = false;
      MOUSE_B2 = false;
      MOUSE_B3 = false;
      MOUSE_B4 = false;
      MOUSE_SCROLL_NEW = 0;
      Context = null;

      TerminalGameInputContext = new TerminalGameInputContext(this);
      MenuInputContext = new MenuInputContext(this);
      DebugGameInputContext = new DebugGameInputContext(this);
      GameInputContext = new GameInputContext(this);
    }

    public void Dispose()
    {
      INPUT_ENGINE = null;
    }

    public void ClearInput()
    {
      INPUT_ENGINE.GetKeyBuffer(KEY_BUFFER, ref numkeybuffer);
      INPUT_ENGINE.GetKeyPressedArray(KEY_PRESSED);
    }

    public void ProcessInput()
    {
      if (Terminal.TConsole.Visible)
      { // Handling Terminal
          Context = TerminalGameInputContext;
      }
      else if (Engine.Screen2D.ShowPage && Engine.Screen2D.CurrentPage != null)
      { // Handling Menu
          Context = MenuInputContext;
      }
      else
      { // Handling Game
        Context = Engine.Settings.GameDebug ? DebugGameInputContext : GameInputContext;
      }

      INPUT_ENGINE.GetKeyBuffer(KEY_BUFFER, ref numkeybuffer);
      INPUT_ENGINE.GetKeyPressedArray(KEY_PRESSED);
      INPUT_ENGINE.GetMouseState(ref MOUSE_X, ref MOUSE_Y, ref MOUSE_B1, ref MOUSE_B2, ref MOUSE_B3, ref MOUSE_B4, ref MOUSE_SCROLL_NEW);
      INPUT_ENGINE.GetMousePosition(ref MOUSE_X, ref MOUSE_Y);

      SHIFT = (KEY_PRESSED[(int)CONST_TV_KEY.TV_KEY_LEFTSHIFT] != 0 || KEY_PRESSED[(int)CONST_TV_KEY.TV_KEY_RIGHTSHIFT] != 0);
      CTRL = (KEY_PRESSED[(int)CONST_TV_KEY.TV_KEY_LEFTCONTROL] != 0 || KEY_PRESSED[(int)CONST_TV_KEY.TV_KEY_RIGHTCONTROL] != 0);
      ALT = (KEY_PRESSED[(int)CONST_TV_KEY.TV_KEY_ALT_LEFT] != 0 || KEY_PRESSED[(int)CONST_TV_KEY.TV_KEY_ALT_RIGHT] != 0);

      for (int n = 0; n < numkeybuffer; n++)
        Context.HandleKeyBuffer(KEY_BUFFER[n]);

      Context.HandleKeyState(KEY_PRESSED);
      Context.HandleMouse(MOUSE_X, MOUSE_Y, MOUSE_B1, MOUSE_B2, MOUSE_B3, MOUSE_B4, MOUSE_SCROLL_NEW);
    }
  }
}

