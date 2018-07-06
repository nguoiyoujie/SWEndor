using System.Collections.Generic;
using MTV3D65;

namespace SWEndor
{
  public class InputManager
  {
    private static InputManager _instance;
    public static InputManager Instance()
    {
      if (_instance == null) { _instance = new InputManager(); }
      return _instance;
    }

    private InputManager()
    {
      INPUT_ENGINE = new TVInputEngine();
      INPUT_ENGINE.Initialize(true);
      numkeybuffer = 0;
      KEY_BUFFER = new TV_KEYDATA[256];
      KEY_PRESSED = new byte[256];
      MOUSE_X = 0;
      MOUSE_Y = 0;
      prevMOUSE_X = 0;
      prevMOUSE_Y = 0;
      MOUSE_B1 = false;
      MOUSE_B2 = false;
      MOUSE_B3 = false;
      MOUSE_B4 = false;
      MOUSE_SCROLL_NEW = 0;
    }

    public void Dispose()
    {
      INPUT_ENGINE = null;
    }

    public TVInputEngine INPUT_ENGINE { get; private set; }
    int numkeybuffer;
    private TV_KEYDATA[] KEY_BUFFER;
    private byte[] KEY_PRESSED;
    public int MOUSE_X;
    public int MOUSE_Y;
    private int prevMOUSE_X;
    private int prevMOUSE_Y;
    private bool MOUSE_B1;
    private bool MOUSE_B2;
    private bool MOUSE_B3;
    private bool MOUSE_B4;
    private int MOUSE_SCROLL_NEW;



    public static ThreadSafeDictionary<string, int> FunctionKeyMap { get; private set; }

    public static void GenerateKeyMap()
    {
      FunctionKeyMap = new ThreadSafeDictionary<string, int>();

      // Control functions - DO NOT CHANGE
      //FunctionKeyMap.AddorUpdateItem("esc", (int)CONST_TV_KEY.TV_KEY_ESCAPE);
      //FunctionKeyMap.AddorUpdateItem("ret", (int)CONST_TV_KEY.TV_KEY_RETURN);
      //FunctionKeyMap.AddorUpdateItem("del", (int)CONST_TV_KEY.TV_KEY_DELETE);
      //FunctionKeyMap.AddorUpdateItem("bsp", (int)CONST_TV_KEY.TV_KEY_BACKSPACE);

      // Game defaults
      FunctionKeyMap.AddorUpdateItem("g_speed+", (int)CONST_TV_KEY.TV_KEY_Q);
      FunctionKeyMap.AddorUpdateItem("g_speed-", (int)CONST_TV_KEY.TV_KEY_A);
      FunctionKeyMap.AddorUpdateItem("g_weap1mode+", (int)CONST_TV_KEY.TV_KEY_Z);
      FunctionKeyMap.AddorUpdateItem("g_weap1mode-", -1);
      FunctionKeyMap.AddorUpdateItem("g_weap2mode+", (int)CONST_TV_KEY.TV_KEY_X);
      FunctionKeyMap.AddorUpdateItem("g_weap2mode-", -1);
      FunctionKeyMap.AddorUpdateItem("g_ui_toggle", (int)CONST_TV_KEY.TV_KEY_U);
      FunctionKeyMap.AddorUpdateItem("g_ui_status_toggle", (int)CONST_TV_KEY.TV_KEY_Y);
      FunctionKeyMap.AddorUpdateItem("g_ui_score_toggle", (int)CONST_TV_KEY.TV_KEY_T);
      FunctionKeyMap.AddorUpdateItem("g_ui_radar_toggle", (int)CONST_TV_KEY.TV_KEY_R);
      FunctionKeyMap.AddorUpdateItem("g_cammode+", (int)CONST_TV_KEY.TV_KEY_E);
      FunctionKeyMap.AddorUpdateItem("g_cammode-", -1);
    }

    public void ProcessInput()
    {
      INPUT_ENGINE.GetKeyBuffer(KEY_BUFFER, ref numkeybuffer);

      for (int n = 0; n < numkeybuffer; n++)
      {
        if (KEY_BUFFER[n].Pressed > 0)
        {
          if (Screen2D.Instance().ShowPage && Screen2D.Instance().CurrentPage != null)
          {
            if (Screen2D.Instance().CurrentPage.OnKeyPress((CONST_TV_KEY)KEY_BUFFER[n].Key))
              SoundManager.Instance().SetSound("Button_1");
          }
          else
          {
            if (KEY_BUFFER[n].Key == (int)CONST_TV_KEY.TV_KEY_ESCAPE)
            {
              if (!Screen2D.Instance().ShowPage)
              {
                Screen2D.Instance().CurrentPage = new UIPage_PauseMenu();
                Screen2D.Instance().ShowPage = true;
                Game.Instance().IsPaused = true;
              }
            }

            if (KEY_BUFFER[n].Key == (int)CONST_TV_KEY.TV_KEY_RETURN)
            {
            }
            else if (KEY_BUFFER[n].Key == FunctionKeyMap.GetItem("g_cammode+"))
            {
              switch (PlayerInfo.Instance().CameraMode)
              {
                case CameraMode.FIRSTPERSON:
                  PlayerInfo.Instance().CameraMode = CameraMode.THIRDPERSON;
                  break;
                case CameraMode.THIRDPERSON:
                  PlayerInfo.Instance().CameraMode = CameraMode.THIRDREAR;
                  break;
                case CameraMode.THIRDREAR:
                  PlayerInfo.Instance().CameraMode = CameraMode.FIRSTPERSON;
                  break;
              }
            }
            else if (KEY_BUFFER[n].Key == FunctionKeyMap.GetItem("g_cammode-"))
            {
              switch (PlayerInfo.Instance().CameraMode)
              {
                case CameraMode.FIRSTPERSON:
                  PlayerInfo.Instance().CameraMode = CameraMode.THIRDREAR;
                  break;
                case CameraMode.THIRDPERSON:
                  PlayerInfo.Instance().CameraMode = CameraMode.FIRSTPERSON;
                  break;
                case CameraMode.THIRDREAR:
                  PlayerInfo.Instance().CameraMode = CameraMode.THIRDPERSON;
                  break;
              }
            }
            else if (KEY_BUFFER[n].Key == FunctionKeyMap.GetItem("g_weap1mode+"))
            {
              PlayerInfo.Instance().NextPrimaryWeapon();
              SoundManager.Instance().SetSound("Button_1");
            }
            else if (KEY_BUFFER[n].Key == FunctionKeyMap.GetItem("g_weap1mode-"))
            {
              PlayerInfo.Instance().PrevPrimaryWeapon();
              SoundManager.Instance().SetSound("Button_1");
            }
            else if (KEY_BUFFER[n].Key == FunctionKeyMap.GetItem("g_weap2mode+"))
            {
              PlayerInfo.Instance().NextSecondaryWeapon();
              SoundManager.Instance().SetSound("Button_1");
            }
            else if (KEY_BUFFER[n].Key == FunctionKeyMap.GetItem("g_weap2mode-"))
            {
              PlayerInfo.Instance().PrevSecondaryWeapon();
              SoundManager.Instance().SetSound("Button_1");
            }
            else if (KEY_BUFFER[n].Key == FunctionKeyMap.GetItem("g_ui_toggle"))
            {
              PlayerInfo.Instance().ShowUI = !PlayerInfo.Instance().ShowUI;
              SoundManager.Instance().SetSound("Button_1");
            }
            else if (KEY_BUFFER[n].Key == FunctionKeyMap.GetItem("g_ui_status_toggle"))
            {
              PlayerInfo.Instance().ShowStatus = !PlayerInfo.Instance().ShowStatus;
              SoundManager.Instance().SetSound("Button_1");
            }
            else if (KEY_BUFFER[n].Key == FunctionKeyMap.GetItem("g_ui_score_toggle"))
            {
              PlayerInfo.Instance().ShowScore = !PlayerInfo.Instance().ShowScore;
              SoundManager.Instance().SetSound("Button_1");
            }
            else if (KEY_BUFFER[n].Key == FunctionKeyMap.GetItem("g_ui_radar_toggle"))
            {
              PlayerInfo.Instance().ShowRadar = !PlayerInfo.Instance().ShowRadar;
              SoundManager.Instance().SetSound("Button_1");
            }

            // Debug
            else if (KEY_BUFFER[n].Key == (int)CONST_TV_KEY.TV_KEY_5)
            {
              foreach (ActorInfo ainfo in GameScenarioManager.Instance().EnemyFighters.Values)
              {
                ainfo.ActorState = ActorState.DEAD;
              }
              foreach (ActorInfo ainfo in GameScenarioManager.Instance().EnemyShips.Values)
              {
                ainfo.ActorState = ActorState.DEAD;
              }
            }
            else if (KEY_BUFFER[n].Key == (int)CONST_TV_KEY.TV_KEY_N)
            {
              PlayerInfo.Instance().IsMovementControlsEnabled = !PlayerInfo.Instance().IsMovementControlsEnabled;
            }
            else if (KEY_BUFFER[n].Key == (int)CONST_TV_KEY.TV_KEY_M)
            {
              Game.Instance().AddTime = 10;
            }
            else if (KEY_BUFFER[n].Key == (int)CONST_TV_KEY.TV_KEY_L)
            {
              GameSaver.Save(@"save.txt");
            }
            //else if (KEY_BUFFER[n].Key == (int)CONST_TV_KEY.TV_KEY_P)
            //{
            //  Game.Instance().IsPaused = !Game.Instance().IsPaused;
            //}
          }
        }
      }

      if (!Screen2D.Instance().ShowPage || Screen2D.Instance().CurrentPage == null)
      {

        INPUT_ENGINE.GetKeyPressedArray(KEY_PRESSED);

        // speed
        if ((byte)KEY_PRESSED.GetValue(FunctionKeyMap.GetItem("g_speed+")) != 0)
        {
          PlayerInfo.Instance().ChangeSpeed(1);
        }
        else if ((byte)KEY_PRESSED.GetValue(FunctionKeyMap.GetItem("g_speed-")) != 0)
        {
          PlayerInfo.Instance().ChangeSpeed(-1);
        }

        INPUT_ENGINE.GetMouseState(ref MOUSE_X, ref MOUSE_Y, ref MOUSE_B1, ref MOUSE_B2, ref MOUSE_B3, ref MOUSE_B4, ref MOUSE_SCROLL_NEW);
        if (MOUSE_B1)
        {
          PlayerInfo.Instance().FireWeapon(false);
        }
        if (MOUSE_B2)
        {
          PlayerInfo.Instance().FireWeapon(true);
        }

        INPUT_ENGINE.GetMousePosition(ref MOUSE_X, ref MOUSE_Y);
        PlayerInfo.Instance().RotateCam(MOUSE_X / Engine.Instance().ScreenWidth * 2 - 1, MOUSE_Y / Engine.Instance().ScreenHeight * 2 - 1);

        // not needed, but may be useful if there is an intention to change how mouse steering works
        prevMOUSE_X = MOUSE_X;
        prevMOUSE_Y = MOUSE_Y;
      }
    }
  }
}

