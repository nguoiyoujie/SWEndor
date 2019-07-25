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
    DUMMY = 0,

    // SYSTEM
    SYSTEM_INIT_ERROR = 1,
    SYSTEM_RUN_ERROR = 2,

    SYSTEM_TITLE_ERROR = 1001,


    // PLAYER
    PLAYER_OUTOFBOUNDS = 11001,

  }

  public static class TextLocalization
  {
    static Dictionary<TextLocalKeys, string> keys = new Dictionary<TextLocalKeys, string>
    {
      { TextLocalKeys.DUMMY, "" },

      { TextLocalKeys.SYSTEM_INIT_ERROR, "Fatal Error occurred during initialization. Please see {0} in the /Log folder for the error message." },
      { TextLocalKeys.SYSTEM_RUN_ERROR, "Fatal Error occurred during initialization. Please see {0} in the /Log folder for the error message." },

      { TextLocalKeys.SYSTEM_TITLE_ERROR,"{0} - Error Encountered!" },


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
