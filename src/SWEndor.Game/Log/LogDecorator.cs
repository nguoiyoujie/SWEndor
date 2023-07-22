using Primrose.Primitives.Factories;

namespace SWEndor
{
  internal static class LogDecorator
  {
    private static readonly Registry<LogType, string> Decorator = new Registry<LogType, string>();

    static LogDecorator()
    {
      Decorator.Add(LogType.SYS_INIT, "{0} initalizing, version {1}.");
      Decorator.Add(LogType.SYS_CLOSE, "{0} exiting.");
      Decorator.Add(LogType.INFO, "{0}");
      Decorator.Add(LogType.ASSET_LOADING, "Loading {0} {1}");
      Decorator.Add(LogType.ASSET_LOADED, "Load OK {0} {1}");
      Decorator.Add(LogType.SHADER_LOADED, "Shader '{0}' was created.");
      Decorator.Add(LogType.SHADER_LOAD_ERROR, "Shader '{0}' error: {1}");
      Decorator.Add(LogType.MISSING_SHADER_VALUE_SOURCE, "Missing function for DynamicShaderDataSource '{0}'!");

      Decorator.Add(LogType.GETDISTANCE_CACHECLEAR, "GetDistance cache clear: {0} / {1}, taking {2} ms");

#if DEBUG
      Decorator.Add(LogType.ACTOR_CREATED, "{0} was created.");
      Decorator.Add(LogType.ACTOR_CREATIONSTATECHANGED, "{0} creation state changed to {1}.");
      Decorator.Add(LogType.ACTOR_ACTORSTATECHANGED, "{0} actor state changed to {1}.");

      Decorator.Add(LogType.ACTOR_DAMAGED, "{0} was damaged by {1} for {2}. HP is now {3}.");
      Decorator.Add(LogType.ACTOR_HEALED, "{0} was healed by {1} for {2}. HP is now {3}.");
      Decorator.Add(LogType.ACTOR_KILLED, "{0} was killed.");
      Decorator.Add(LogType.ACTOR_KILLED_BY, "{0} was killed by {1}.");
      Decorator.Add(LogType.ACTOR_DISPOSED, "{0} was disposed.");
#endif
    }

    public static string GetFormat(LogType logtype)
    {
      return Decorator[logtype];
    }
  }
}
