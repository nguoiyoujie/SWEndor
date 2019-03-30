namespace SWEndor.Scenarios.Scripting
{
  public class ScriptFactory
  {
    private static ThreadSafeDictionary<string, Script> m_registry = new ThreadSafeDictionary<string, Script>();

    public static void AddScript(string id, Script script)
    {
      m_registry.Put(id, script);
    }

    public static void Unload()
    {
      m_registry.Clear();
    }

    public static Script GetScript(string id)
    {
      return m_registry.Get(id);
    }
  }
}
