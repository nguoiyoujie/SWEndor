using MTV3D65;
using SWEndor.ActorTypes;
using SWEndor.Primitives;

namespace SWEndor.Actors.Data
{
  public class MeshDataSet
  {
    internal MeshData[] list = new MeshData[Globals.ActorLimit];
    public MeshData this[ActorInfo actor] { get { return list[actor.dataID]; } }

    public void Init(ActorInfo actor, ActorTypeInfo type, ActorCreationInfo acinfo) { ((DoFunc<MeshData, ActorTypeInfo, ActorCreationInfo>)((ref MeshData d, ref ActorTypeInfo t, ref ActorCreationInfo ac) => { d.Init(t, ac); }))(ref list[actor.dataID], ref type, ref acinfo); }
    public void Reset(ActorInfo actor) { ((DoFunc<MeshData>)((ref MeshData d) => { d.Reset(); }))(ref list[actor.dataID]); }
    public void Do(ActorInfo actor, DoFunc<MeshData> func) { func.Invoke(ref list[actor.dataID]); }

    // Mesh { get; functions; }
    private static GetFunc<MeshData, TVMesh> m_get = (ref MeshData d) => { return d.Mesh; };
    public TVMesh Mesh_get(ActorInfo actor)
    {
      return m_get(ref list[actor.dataID]);
    }

    private static DoFunc<MeshData, int, ActorTypeInfo> m_gen = (ref MeshData d, ref int i, ref ActorTypeInfo t) => { d.Generate(i, t); };
    public void Mesh_generate(ActorInfo actor, ActorTypeInfo type)
    {
      int id = actor.ID;
      m_gen(ref list[actor.dataID], ref id, ref type);
    }

    private static GetFunc<MeshData, bool> m_vis = (ref MeshData d) => { return d.Mesh?.IsVisible() ?? false; };
    public bool Mesh_getIsVisible(ActorInfo actor)
    {
      return m_vis(ref list[actor.dataID]);
    }

    private static SetFunc<MeshData, int> m_sett = (ref MeshData d, int v) => { d.Mesh?.SetTexture(v); };
    public void Mesh_setTexture(ActorInfo actor, int iTexture)
    {
      m_sett(ref list[actor.dataID], iTexture);
    }

    private static DoFunc<MeshData, TV_3DVECTOR, TV_3DVECTOR, TV_3DVECTOR> m_bas = (ref MeshData d, ref TV_3DVECTOR f, ref TV_3DVECTOR u, ref TV_3DVECTOR r) => { d.Mesh?.GetBasisVectors(ref f, ref u, ref r); };
    public void Mesh_getBasisVectors(ActorInfo actor, ref TV_3DVECTOR front, ref TV_3DVECTOR up, ref TV_3DVECTOR right)
    {
      m_bas(ref list[actor.dataID], ref front, ref up, ref right);
    }

    private static GetFunc<MeshData, bool, BoundingBox> m_gbb = (ref MeshData d, ref bool local) =>
      {
        TV_3DVECTOR minV = new TV_3DVECTOR();
        TV_3DVECTOR maxV = new TV_3DVECTOR();
        d.Mesh?.GetBoundingBox(ref minV, ref maxV, local);

        return new BoundingBox(minV, maxV);
      };
    public BoundingBox Mesh_getBoundingBox(ActorInfo actor, bool uselocal)
    {
      return m_gbb(ref list[actor.dataID], ref uselocal);
    }

    private static GetFunc<MeshData, bool, BoundingSphere> m_gbs = (ref MeshData d, ref bool local) =>
      {
        TV_3DVECTOR p = new TV_3DVECTOR();
        float r = 0;
        d.Mesh?.GetBoundingSphere(ref p, ref r, local);

        return new BoundingSphere(p, r);
      };
    public BoundingSphere Mesh_getBoundingSphere(ActorInfo actor, bool uselocal)
    {
      return m_gbs(ref list[actor.dataID], ref uselocal);
    }

    private static GetFunc<MeshData, int> m_gvc = (ref MeshData d) => { return d.Mesh?.GetVertexCount() ?? 0; };
    public int Mesh_getVertexCount(ActorInfo actor)
    {
      return m_gvc(ref list[actor.dataID]);
    }

    private static GetFunc<MeshData, int, TV_3DVECTOR> m_gvt = (ref MeshData d, ref int vID) => { return d.GetVertex(vID); };
    public TV_3DVECTOR Mesh_getVertex(ActorInfo actor, int vertexID)
    {
      return m_gvt(ref list[actor.dataID], ref vertexID);
    }

    private static DoFunc<MeshData> m_des = (ref MeshData d) => { d.Mesh.Destroy(); };
    public void Mesh_destroy(ActorInfo actor)
    {
      m_des(ref list[actor.dataID]);
    }
  }
}
