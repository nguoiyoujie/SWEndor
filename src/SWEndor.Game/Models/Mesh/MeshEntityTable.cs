using Primrose.Primitives.Factories;
using System.Collections.Generic;

namespace SWEndor
{
  public struct MeshInfo
  {
    public int ActorID;
    public int RenderOrder;
  }

  public class MeshEntityTable : Registry<int, MeshInfo>
  {
    //private readonly Dictionary<int, MeshInfo> m_ids = new Dictionary<int, MeshInfo>();
    private readonly Registry<int, int> m_renderOrderCount = new Registry<int, int>();
    private readonly HashSet<int> m_Visible = new HashSet<int>();

    public bool TryGet(int meshID, out MeshInfo info)
    {
      if (Contains(meshID)) 
      {
        info = this[meshID];
        return true;
      }
      info = default;
      return false;
      //return m_ids.TryGetValue(meshID, out info);
    }

    public override void Put(int key, MeshInfo item)
    {
      base.Put(key, item);
      m_renderOrderCount[item.RenderOrder]++;
    }

    public override bool Remove(int key)
    {
      if (Contains(key))
      {
        MeshInfo info = this[key];
        m_renderOrderCount[info.RenderOrder]--;
      }
      return base.Remove(key);
    }

    public void MarkVisible(int meshID, bool visible)
    {
      if (visible)
      {
        if (!m_Visible.Contains(meshID))
          m_Visible.Add(meshID);
      }
      else
      {
        m_Visible.Remove(meshID);
      }
    }

    public bool IsVisible(int meshID)
    {
      return m_Visible.Contains(meshID);
    }

    public int GetMaxRenderOrder()
    {
      int ret = 0;
      foreach (int v in m_renderOrderCount.EnumerateKeys())
      {
        if (m_renderOrderCount[v] > 0 && v > ret)
        {
          ret = v;
        }
      }
      return ret;
    }

    //public void Put(int meshID, MeshInfo info)
    //{
    //  m_ids[meshID] = info;
    //}
    //
    //public bool Remove(int meshID)
    //{
    //  return m_ids.Remove(meshID);
    //}
  }
}
