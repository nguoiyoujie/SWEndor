using MTV3D65;
using SWEndor.Input.Functions;
using SWEndor.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace SWEndor
{
  public enum TextLocalKeys
  {
    PLAYER_OUTOFBOUNDS,

  }

  public static class TextLocalization
  {
    static Dictionary<TextLocalKeys, string> keys = new Dictionary<TextLocalKeys, string>
    {
      { TextLocalKeys.PLAYER_OUTOFBOUNDS, "You are going out of bounds! Return to the battle!" }





    };

    public static string Get(TextLocalKeys key)
    {
      if (keys.ContainsKey(key))
        return "";

      return keys[key];
    }
  }
}
