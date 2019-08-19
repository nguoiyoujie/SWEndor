using MTV3D65;
using SWEndor.Actors.Data;

namespace SWEndor.Actors.Components
{
  public static class MeshSystem
  {
    internal static void SetScale(Engine engine, ActorInfo actor, float scale)
    {
      engine.MeshDataSet.Scale_set(actor, scale);
    }

    internal static void EnlargeScale(Engine engine, ActorInfo actor, float scale)
    {
      engine.MeshDataSet.Scale_set(actor, engine.MeshDataSet.Scale_get(actor) + scale);
    }

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
      if (data.Mesh != null && data.FarMesh != null)
      {
        TV_3DVECTOR pos = actor.GetPosition();
        TV_3DVECTOR rot = actor.GetRotation();

        data.Mesh.SetPosition(pos.x, pos.y, pos.z);
        data.Mesh.SetRotation(rot.x, rot.y, rot.z);

        data.FarMesh.SetPosition(pos.x, pos.y, pos.z);
        data.FarMesh.SetRotation(rot.x, rot.y, rot.z);

        data.Mesh.SetCollisionEnable(engine.MaskDataSet[actor].Has(ComponentMask.CAN_BECOLLIDED) && !actor.IsAggregateMode);
        //data.FarMesh.SetCollisionEnable(false);
      }
    }
  }
}
