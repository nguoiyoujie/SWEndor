using MTV3D65;
using SWEndor.Actors.Data;

namespace SWEndor.Actors.Components
{
  public static class MeshSystem
  {
    internal static void RenderMesh(Engine engine, ActorInfo actor)
    {
      RenderMesh(ref engine.MeshDataSet.list[actor.dataID]);
    }

    private static void RenderMesh(ref MeshData data)
    {
      if (data.Mesh?.IsVisible() ?? false)
        data.Mesh.Render();
    }

    internal static void RenderFarMesh(Engine engine, ActorInfo actor)
    {
      RenderFarMesh(ref engine.MeshDataSet.list[actor.dataID]);
    }

    private static void RenderFarMesh(ref MeshData data)
    {
      if (data.FarMesh != null && data.Mesh.IsVisible())
        data.Mesh.Render();
    }

    internal static void Update(Engine engine, ActorInfo actor)
    {
      Update(engine, actor, ref engine.MeshDataSet.list[actor.dataID]);
    }

    private static void Update(Engine engine, ActorInfo actor, ref MeshData data)
    {
      if (data.Mesh == null && data.FarMesh == null)
        return;

      TV_3DMATRIX mat = actor.GetMatrix();

      data.Mesh?.SetMatrix(mat);
      data.FarMesh?.SetMatrix(mat);

      data.Mesh?.SetCollisionEnable(engine.MaskDataSet[actor].Has(ComponentMask.CAN_BECOLLIDED) && !actor.IsAggregateMode);
    }
  }
}
