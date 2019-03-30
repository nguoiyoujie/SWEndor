namespace SWEndor.Terminal
{
  public static class TCommandParser
  {
    public static TCommandFeedback Execute(string input)
    {
      // Command syntax:
      // <command>:<param1>`<param2>`<param3>`...

      string cmd = "";
      string prms = "";

      int index = input.IndexOf(':');
      if (index > -1)
      {
        if (input.Length > index)
          prms = input.Substring(index + 1);

        cmd = input.Substring(0, index);
      }
      else
      {
        cmd = input;
      }

      return Execute(cmd, prms.Split('`'));
    }

    public static TCommandFeedback Execute(string cmd, string[] param)
    {
      switch (cmd.ToLower())
      {
        case "actor.spawn":
          return new Commands.Actor.Spawn().Execute(param);
        default:
          return TCommandFeedback.NULL;
      }
    }
  }
}
