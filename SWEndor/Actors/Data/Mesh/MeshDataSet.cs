using MTV3D65;
using SWEndor.ActorTypes;
using SWEndor.Primitives;
using System;

namespace SWEndor.Actors.Data
{
  public class MeshDataSet
  {
    internal MeshData[] list = new MeshData[Globals.ActorLimit];
    public MeshData this[ActorInfo actor] { get { return list[actor.dataID]; } }

    public void Init(ActorInfo actor, ActorTypeInfo type, ActorCreationInfo acinfo) { ((DoFunc<MeshData, ActorTypeInfo, ActorCreationInfo>)((ref MeshData d, ref ActorTypeInfo t, ref ActorCreationInfo ac) => { d.Init(t, ac); }))(ref list[actor.dataID], ref type, ref acinfo); }
    public void Reset(ActorInfo actor) { ((DoFunc<MeshData>)((ref MeshData d) => { d.Reset(); }))(ref list[actor.dataID]); }
    public void Do(ActorInfo actor, DoFunc<MeshData> func) { func.Invoke(ref list[actor.dataID]); }

    // Scale { get; set; }



    public void Scale_set(ActorInfo actor, float value)
    {
      ((SetFunc<MeshData, float>)((ref MeshData d, float v) => 
      {
        d.Scale = v;
      }
      ))(ref list[actor.dataID], value);
    }

    public float Scale_get(ActorInfo actor)
    {
      return ((GetFunc<MeshData, float>)((ref MeshData d) => 
      {
        return d.Scale;
      }
      ))(ref list[actor.dataID]);
    }

    // Mesh { get; functions; }
    public TVMesh Mesh_get(ActorInfo actor)
    {
      return ((GetFunc<MeshData, TVMesh>)((ref MeshData d) =>
      {
        return d.Mesh;
      }
      ))(ref list[actor.dataID]);
    }

    public void Mesh_generate(ActorInfo actor, ActorTypeInfo type)
    {
      int id = actor.ID;
      ((DoFunc<MeshData, int, ActorTypeInfo>)((ref MeshData d, ref int i, ref ActorTypeInfo t) => 
      {
        d.Generate(i, t);
      }
      ))(ref list[actor.dataID], ref id, ref type);
    }

    public bool Mesh_getIsVisible(ActorInfo actor)
    {
      return ((GetFunc<MeshData, bool>)((ref MeshData d) => 
      {
        return d.Mesh?.IsVisible() ?? false;
      }
      ))(ref list[actor.dataID]);
    }

    public void Mesh_setTexture(ActorInfo actor, int iTexture)
    {
      ((SetFunc<MeshData, int>)((ref MeshData d, int v) =>
      {
        d.Mesh?.SetTexture(iTexture);
      }
      ))(ref list[actor.dataID], iTexture);
    }

    public void Mesh_getBasisVectors(ActorInfo actor, ref TV_3DVECTOR front, ref TV_3DVECTOR up, ref TV_3DVECTOR right)
    {
      ((DoFunc<MeshData, TV_3DVECTOR, TV_3DVECTOR, TV_3DVECTOR>)((ref MeshData d, ref TV_3DVECTOR f, ref TV_3DVECTOR u, ref TV_3DVECTOR r) =>
      {
        d.Mesh?.GetBasisVectors(ref f, ref u, ref r);
      }
      ))(ref list[actor.dataID], ref front, ref up, ref right);
    }

    public BoundingBox Mesh_getBoundingBox(ActorInfo actor, bool uselocal)
    {
      return ((GetFunc<MeshData, bool, BoundingBox>)((ref MeshData d, ref bool local) =>
      {
        TV_3DVECTOR minV = new TV_3DVECTOR();
        TV_3DVECTOR maxV = new TV_3DVECTOR();
        d.Mesh?.GetBoundingBox(ref minV, ref maxV, local);

        return new BoundingBox(minV, maxV);
      }
      ))(ref list[actor.dataID], ref uselocal);
    }

    public BoundingSphere Mesh_getBoundingSphere(ActorInfo actor, bool uselocal)
    {
      return ((GetFunc<MeshData, bool, BoundingSphere>)((ref MeshData d, ref bool local) =>
      {
        TV_3DVECTOR p = new TV_3DVECTOR();
        float r = 0;
        d.Mesh?.GetBoundingSphere(ref p, ref r, local);

        return new BoundingSphere(p, r);
      }
      ))(ref list[actor.dataID], ref uselocal);
    }

    public int Mesh_getVertexCount(ActorInfo actor)
    {
      return ((GetFunc<MeshData, int>)((ref MeshData d) =>
      {
        return d.Mesh?.GetVertexCount() ?? 0;
      }
      ))(ref list[actor.dataID]);
    }

    public TV_3DVECTOR Mesh_getVertex(ActorInfo actor, int vertexID)
    {
      return ((GetFunc<MeshData, int, TV_3DVECTOR>)((ref MeshData d, ref int vID) => 
      {
        return d.GetVertex(vID);
      }
      ))(ref list[actor.dataID], ref vertexID);
    }

    public void Mesh_destroy(ActorInfo actor)
    {
      ((DoFunc<MeshData>)((ref MeshData d) => 
      {
        d.Mesh.Destroy();
      }
      ))(ref list[actor.dataID]);
    }
  }
}
