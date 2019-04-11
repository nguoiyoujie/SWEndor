using SWEndor.Primitives;

namespace SWEndor.Scenarios.Scripting
{
  public partial class Script
  {
    public class Registry
    {
      private static ThreadSafeDictionary<string, Script> m_registry = new ThreadSafeDictionary<string, Script>();

      public static void Add(string id, Script script)
      {
        m_registry.Put(id, script);
      }

      public static void Clear()
      {
        m_registry.Clear();
      }

      public static Script Get(string id)
      {
        return m_registry.Get(id);
      }
    }
  }
}
