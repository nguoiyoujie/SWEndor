using MTV3D65;
using SWEndor.ActorTypes;
using SWEndor.Primitives;
using System;

namespace SWEndor.Actors.Data
{
  public class MeshDataSet
  {
    internal MeshData[] list = new MeshData[Globals.ActorLimit];
    public MeshData this[int id] { get { return list[id % Globals.ActorLimit]; } }

    public void Init(int id, ActorTypeInfo type, ActorCreationInfo acinfo) { ((DoFunc<MeshData, ActorTypeInfo, ActorCreationInfo>)((ref MeshData d, ref ActorTypeInfo t, ref ActorCreationInfo ac) => { d.Init(t, ac); }))(ref list[id % Globals.ActorLimit], ref type, ref acinfo); }
    public void Reset(int id) { ((DoFunc<MeshData>)((ref MeshData d) => { d.Reset(); }))(ref list[id % Globals.ActorLimit]); }
    public void Do(int id, DoFunc<MeshData> func) { func.Invoke(ref list[id % Globals.ActorLimit]); }

    // Mesh { get; functions; }
    public TVMesh Mesh_get(int id)
    {
      return ((GetFunc<MeshData, TVMesh>)((ref MeshData d) =>
      {
        return d.Mesh;
      }
      ))(ref list[id % Globals.ActorLimit]);
    }

    public void Mesh_generate(int id, ActorTypeInfo type)
    {
      ((DoFunc<MeshData, int, ActorTypeInfo>)((ref MeshData d, ref int i, ref ActorTypeInfo t) => 
      {
        d.Generate(i, t);
      }
      ))(ref list[id % Globals.ActorLimit], ref id, ref type);
    }

    public bool Mesh_getIsVisible(int id)
    {
      return ((GetFunc<MeshData, bool>)((ref MeshData d) => 
      {
        return d.Mesh?.IsVisible() ?? false;
      }
      ))(ref list[id % Globals.ActorLimit]);
    }

    public void Mesh_setTexture(int id, int iTexture)
    {
      ((SetFunc<MeshData, int>)((ref MeshData d, int v) =>
      {
        d.Mesh?.SetTexture(iTexture);
      }
      ))(ref list[id % Globals.ActorLimit], iTexture);
    }

    public BoundingBox Mesh_getBoundingBox(int id, bool uselocal)
    {
      return ((GetFunc<MeshData, bool, BoundingBox>)((ref MeshData d, ref bool local) =>
      {
        TV_3DVECTOR minV = new TV_3DVECTOR();
        TV_3DVECTOR maxV = new TV_3DVECTOR();
        d.Mesh?.GetBoundingBox(ref minV, ref maxV, local);

        return new BoundingBox(minV, maxV);
      }
      ))(ref list[id % Globals.ActorLimit], ref uselocal);
    }

    public BoundingSphere Mesh_getBoundingSphere(int id, bool uselocal)
    {
      return ((GetFunc<MeshData, bool, BoundingSphere>)((ref MeshData d, ref bool local) =>
      {
        TV_3DVECTOR p = new TV_3DVECTOR();
        float r = 0;
        d.Mesh?.GetBoundingSphere(ref p, ref r, local);

        return new BoundingSphere(p, r);
      }
      ))(ref list[id % Globals.ActorLimit], ref uselocal);
    }

    public int Mesh_getVertexCount(int id)
    {
      return ((GetFunc<MeshData, int>)((ref MeshData d) =>
      {
        return d.Mesh?.GetVertexCount() ?? 0;
      }
      ))(ref list[id % Globals.ActorLimit]);
    }

    public TV_3DVECTOR Mesh_getVertex(int id, int vertexID)
    {
      return ((GetFunc<MeshData, int, TV_3DVECTOR>)((ref MeshData d, ref int vID) => 
      {
        return d.GetVertex(vID);
      }
      ))(ref list[id % Globals.ActorLimit], ref vertexID);
    }

    public void Mesh_destroy(int id)
    {
      ((DoFunc<MeshData>)((ref MeshData d) => 
      {
        d.Mesh.Destroy();
      }
      ))(ref list[id % Globals.ActorLimit]);
    }
  }
}
