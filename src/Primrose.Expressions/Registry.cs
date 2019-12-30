using Primrose.Primitives.Factories;

namespace Primrose.Expressions
{
  public partial class Script
  {
    /// <summary>
    /// Represents a registry of scripts
    /// </summary>
    public class Registry
    {
      /// <summary>The global script</summary>
      public static Script Global = new Script();
      private static Registry<Script> m_registry = new Registry<Script>();

      /// <summary>
      /// Adds a script to the registry
      /// </summary>
      /// <param name="id">The unique script identifier</param>
      /// <param name="script">The script to be added</param>
      public static void Add(string id, Script script)
      {
        m_registry.Put(id, script);
      }

      /// <summary>
      /// Clears all script data, including the global script
      /// </summary>
      public static void Clear()
      {
        m_registry.Clear();
        Global.Clear();
      }

      /// <summary>
      /// Retrieves a script from the registry
      /// </summary>
      /// <param name="id">The script identifier</param>
      /// <returns>The script associated with the id, or null if no script is found</returns>
      public static Script Get(string id)
      {
        return m_registry.Get(id);
      }

      /// <summary>
      /// Retrieves all scripts from the registry
      /// </summary>
      /// <returns>The array of all loaded scripts</returns>
      public static Script[] GetAll()
      {
        return m_registry.GetAll();
      }
    }
  }
}
