using Primrose.Expressions;
using Primrose.Primitives.ValueTypes;
using SWEndor.Primitives.Extensions;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class PlayerCameraFns
  {
    public static Val SetPlayerLook(IContext context)
    {
      ((Context)context).Engine.PlayerCameraInfo.SetPlayerLook();
      return Val.NULL;
    }

    public static Val SetSceneLook(IContext context)
    {
      ((Context)context).Engine.PlayerCameraInfo.SetSceneLook();
      return Val.NULL;
    }

    public static Val SetDeathLook(IContext context)
    {
      ((Context)context).Engine.PlayerCameraInfo.SetDeathLook();
      return Val.NULL;
    }

    public static Val EnableFreeLook(IContext context, bool enabled)
    {
      ((Context)context).Engine.PlayerCameraInfo.IsFreeLook = enabled;
      return Val.NULL;
    }

    public static Val SetSceneLook_LookAtActor(IContext context, int actorID)
    {
      ((Context)context).Engine.PlayerCameraInfo.SceneLook.SetTarget_LookAtActor(actorID);
      return Val.NULL;
    }

    public static Val SetSceneLook_LookAtActor(IContext context, int actorID, float3 displacementXYZ)
    {
      ((Context)context).Engine.PlayerCameraInfo.SceneLook.SetTarget_LookAtActor(actorID, displacementXYZ.ToVec3());
      return Val.NULL;
    }

    public static Val SetSceneLook_LookAtActor(IContext context, int actorID, float3 displacementXYZ, float3 displacementRelative)
    {
      ((Context)context).Engine.PlayerCameraInfo.SceneLook.SetTarget_LookAtActor(actorID, displacementXYZ.ToVec3(), displacementRelative.ToVec3());
      return Val.NULL;
    }

    public static Val SetSceneLook_LookAtPoint(IContext context, float3 point)
    {
      ((Context)context).Engine.PlayerCameraInfo.SceneLook.SetTarget_LookAtPoint(point.ToVec3());
      return Val.NULL;
    }

    public static Val SetSceneLook_LookFromActor(IContext context, int actorID)
    {
      ((Context)context).Engine.PlayerCameraInfo.SceneLook.SetPosition_Actor(actorID);
      return Val.NULL;
    }

    public static Val SetSceneLook_LookFromActor(IContext context, int actorID, float3 displacementXYZ)
    {
      ((Context)context).Engine.PlayerCameraInfo.SceneLook.SetPosition_Actor(actorID, displacementXYZ.ToVec3());
      return Val.NULL;
    }

    public static Val SetSceneLook_LookFromActor(IContext context, int actorID, float3 displacementXYZ, float3 displacementRelative)
    {
      ((Context)context).Engine.PlayerCameraInfo.SceneLook.SetPosition_Actor(actorID, displacementXYZ.ToVec3(), displacementRelative.ToVec3());
      return Val.NULL;
    }

    public static Val SetSceneLook_LookFromPoint(IContext context, float3 point)
    {
      ((Context)context).Engine.PlayerCameraInfo.SceneLook.SetPosition_Point(point.ToVec3());
      return Val.NULL;
    }
  }
}
