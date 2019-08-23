﻿using MTV3D65;
using SWEndor.ActorTypes;

namespace SWEndor.Actors.Data
{
  public struct MeshData
  {
    public TVMesh Mesh;
    public TVMesh FarMesh;

    public void Init(ActorTypeInfo atype, ActorCreationInfo acinfo)
    { }


    public void Generate(int id, ActorTypeInfo atype)
    {
      Mesh = atype.GenerateMesh();
      FarMesh = atype.GenerateFarMesh();

      Mesh.SetTag(id.ToString());
      //Mesh.ShowBoundingBox(true);

      FarMesh.SetTag(id.ToString());

      Mesh.ComputeBoundings();
      FarMesh.ComputeBoundings();

      Mesh.SetLightingMode(CONST_TV_LIGHTINGMODE.TV_LIGHTING_MANAGED, 8);
      FarMesh.SetLightingMode(CONST_TV_LIGHTINGMODE.TV_LIGHTING_MANAGED, 8);

      Mesh.Enable(true);
      FarMesh.Enable(true);
    }

    public void Reset()
    {
      Mesh?.Destroy();
      Mesh = null;

      FarMesh?.Destroy();
      FarMesh = null;
    }

    public int GetVertexCount() { return Mesh?.GetVertexCount() ?? 0; }

    public static DoFunc<MeshData, TV_3DVECTOR, TV_3DVECTOR, TV_3DVECTOR> GetBasisVectors = (ref MeshData d, ref TV_3DVECTOR f, ref TV_3DVECTOR u, ref TV_3DVECTOR r) =>
      {
        d.Mesh?.GetBasisVectors(ref f, ref u, ref r);
      };


  public TV_3DVECTOR GetVertex(int vertexID)
    {
      float x = 0;
      float y = 0;
      float z = 0;
      float dummy = 0;
      int dumint = 0;
      Mesh?.GetVertex(vertexID, ref x, ref y, ref z, ref dummy, ref dummy, ref dummy, ref dummy, ref dummy, ref dummy, ref dummy, ref dumint);
      return new TV_3DVECTOR(x, y, z);
    }
  }
}
