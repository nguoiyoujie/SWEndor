using System.Collections.Generic;

namespace SWEndor
{
  public class MeshEntityTable
  {
    private readonly Dictionary<int, int> m_ids = new Dictionary<int, int>();

    public bool TryGet(int meshID, out int actorID)
    {
      return m_ids.TryGetValue(meshID, out actorID);
    }

    public void Put(int meshID, int actorID)
    {
      m_ids[meshID] = actorID;
    }

    public bool Remove(int meshID)
    {
      return m_ids.Remove(meshID);
    }
  }
}
