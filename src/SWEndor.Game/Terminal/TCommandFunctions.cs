using Primrose.Primitives.Factories;

namespace SWEndor.Game.Terminal
{
  public static class TCommandFunctions
  {
    private static Registry<string, TCommandBase> _reg = new Registry<string, TCommandBase>()
    {
      {"?", new Commands.General.Help() },
      {"help", new Commands.General.Help() },
      {"version", new Commands.General.Version() },
      {"commands", new Commands.General.ListCommands() },
      {"actor.spawn", new Commands.Actor.Spawn() }
    };

    public static TCommandBase Get(string command)
    {
      return _reg.Get(command);
    }

    public static Primrose.Primitives.Collections.ThreadSafeEnumerable<string> List()
    {
      return _reg.EnumerateKeys();
    }
  }
}