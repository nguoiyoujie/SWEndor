using MTV3D65;
using SWEndor.Actors.Data;

namespace SWEndor.Actors.Components
{
  public static class MeshSystem
  {
    internal static void SetScale(Engine engine, int actorID, float scale)
    {
      engine.MeshDataSet.Scale_set(actorID, scale);
    }

    internal static void EnlargeScale(Engine engine, int actorID, float scale)
    {
      engine.MeshDataSet.Scale_set(actorID, engine.MeshDataSet.Scale_get(actorID) + scale);
    }

    internal static void RenderMesh(Engine engine, int actorID)
    {
      RenderMesh(engine, actorID, ref engine.MeshDataSet.list[engine.ActorFactory.GetIndex(actorID)]);
    }

    private static void RenderMesh(Engine engine, int actorID, ref MeshData data)
    {
      if (data.Initialized && data.Mesh.IsVisible())
        data.Mesh.Render();
    }

    internal static void RenderFarMesh(Engine engine, int actorID)
    {
      RenderFarMesh(engine, actorID, ref engine.MeshDataSet.list[engine.ActorFactory.GetIndex(actorID)]);
    }

    private static void RenderFarMesh(Engine engine, int actorID, ref MeshData data)
    {
      if (data.FarMesh != null && data.Mesh.IsVisible())
        data.Mesh.Render();
    }

    internal static void Update(Engine engine, int actorID)
    {
      Update(engine, actorID, ref engine.MeshDataSet.list[engine.ActorFactory.GetIndex(actorID)]);
    }

    private static void Update(Engine engine, int actorID, ref MeshData data)
    {
      if (data.Mesh != null && data.FarMesh != null)
      {
        ActorInfo actor = engine.ActorFactory.Get(actorID);
        TV_3DVECTOR pos = actor.GetPosition();
        TV_3DVECTOR rot = actor.GetRotation();

        data.Mesh.SetPosition(pos.x, pos.y, pos.z);
        data.Mesh.SetRotation(rot.x, rot.y, rot.z);
        data.FarMesh.SetPosition(pos.x, pos.y, pos.z);
        data.FarMesh.SetRotation(rot.x, rot.y, rot.z);

        data.Mesh.SetCollisionEnable(engine.MaskDataSet[actorID].Has(ComponentMask.CAN_BECOLLIDED) && !ActorInfo.IsAggregateMode(engine, actorID));
        data.FarMesh.SetCollisionEnable(false);
      }
    }
  }
}
