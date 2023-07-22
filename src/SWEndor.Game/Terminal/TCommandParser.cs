using Primrose.Primitives.Extensions;
using System;

namespace SWEndor.Game.Terminal
{
  public static class TCommandParser
  {
    public static TCommandFeedback Execute(string input)
    {
      // Command syntax:
      // <command>
      // <command>:<param1>,<param2>,<param3>,...

      string cmd;
      string prms = "";

      int index = input.IndexOf(':');
      if (index > -1)
      {
        if (input.Length > index)
          prms = input.Substring(index + 1);

        cmd = input.Substring(0, index);
        return Execute(cmd, prms.Split(','));
      }
      else
      {
        cmd = input;
        return Execute(cmd, Array<string>.Empty);
      }
    }

    public static TCommandFeedback Execute(string cmd, string[] param)
    {
      TCommandBase command = TCommandFunctions.Get(cmd.ToLower());
      return command?.Execute(param) ?? TCommandFeedback.NULL;
    }
  }
}
