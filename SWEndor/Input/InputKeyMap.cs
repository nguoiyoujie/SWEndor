using MTV3D65;
using System.Collections.Generic;

namespace SWEndor.Input
{
  public static class InputKeyMap
  {
    private static ThreadSafeDictionary<string, int> FunctionKeyMap { get; set; }

    public static void GenerateDefaultFnKeys()
    {
      FunctionKeyMap = new ThreadSafeDictionary<string, int>();

      // Game defaults
      FunctionKeyMap.Put("g_speed+", (int)CONST_TV_KEY.TV_KEY_Q);
      FunctionKeyMap.Put("g_speed-", (int)CONST_TV_KEY.TV_KEY_A);
      FunctionKeyMap.Put("g_weap1mode+", (int)CONST_TV_KEY.TV_KEY_Z);
      FunctionKeyMap.Put("g_weap1mode-", -1);
      FunctionKeyMap.Put("g_weap2mode+", (int)CONST_TV_KEY.TV_KEY_X);
      FunctionKeyMap.Put("g_weap2mode-", -1);
      FunctionKeyMap.Put("g_ui_toggle", (int)CONST_TV_KEY.TV_KEY_U);
      FunctionKeyMap.Put("g_ui_status_toggle", (int)CONST_TV_KEY.TV_KEY_Y);
      FunctionKeyMap.Put("g_ui_score_toggle", (int)CONST_TV_KEY.TV_KEY_T);
      FunctionKeyMap.Put("g_ui_radar_toggle", (int)CONST_TV_KEY.TV_KEY_R);
      FunctionKeyMap.Put("g_cammode+", (int)CONST_TV_KEY.TV_KEY_E);
      FunctionKeyMap.Put("g_cammode-", -1);
    }

    public static string[] GetFnKeys()
    {
      return FunctionKeyMap.GetKeys();
    }

    public static int GetFnKey(string key)
    {
      return FunctionKeyMap.Get(key);
    }

    public static bool IsKeyInUse(int value)
    {
      return new List<int>(FunctionKeyMap.GetValues()).Contains(value);
    }

    public static bool SetFnKey(string key, int value)
    {
      if (IsKeyInUse(value))
        return false;
      FunctionKeyMap.Put(key, value);
      return true;
    }
  }
}
