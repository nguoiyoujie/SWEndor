using Primrose.Expressions;
using Primrose.Primitives.ValueTypes;
using SWEndor.Primitives.Extensions;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class PlayerCameraFns
  {
    public static Val SetPlayerLook(Context context)
    {
      context.Engine.PlayerCameraInfo.SetPlayerLook();
      return Val.NULL;
    }

    public static Val SetSceneLook(Context context)
    {
      context.Engine.PlayerCameraInfo.SetSceneLook();
      return Val.NULL;
    }

    public static Val SetDeathLook(Context context)
    {
      context.Engine.PlayerCameraInfo.SetDeathLook();
      return Val.NULL;
    }

    public static Val EnableFreeLook(Context context, bool enabled)
    {
      context.Engine.PlayerCameraInfo.IsFreeLook = enabled;
      return Val.NULL;
    }

    public static Val SetSceneLook_LookAtActor(Context context, int actorID)
    {
      context.Engine.PlayerCameraInfo.SceneLook.SetTarget_LookAtActor(actorID);
      return Val.NULL;
    }

    public static Val SetSceneLook_LookAtActor(Context context, int actorID, float3 displacementXYZ)
    {
      context.Engine.PlayerCameraInfo.SceneLook.SetTarget_LookAtActor(actorID, displacementXYZ.ToVec3());
      return Val.NULL;
    }

    public static Val SetSceneLook_LookAtActor(Context context, int actorID, float3 displacementXYZ, float3 displacementRelative)
    {
      context.Engine.PlayerCameraInfo.SceneLook.SetTarget_LookAtActor(actorID, displacementXYZ.ToVec3(), displacementRelative.ToVec3());
      return Val.NULL;
    }

    public static Val SetSceneLook_LookAtPoint(Context context, float3 point)
    {
      context.Engine.PlayerCameraInfo.SceneLook.SetTarget_LookAtPoint(point.ToVec3());
      return Val.NULL;
    }

    public static Val SetSceneLook_LookFromActor(Context context, int actorID)
    {
      context.Engine.PlayerCameraInfo.SceneLook.SetPosition_Actor(actorID);
      return Val.NULL;
    }

    public static Val SetSceneLook_LookFromActor(Context context, int actorID, float3 displacementXYZ)
    {
      context.Engine.PlayerCameraInfo.SceneLook.SetPosition_Actor(actorID, displacementXYZ.ToVec3());
      return Val.NULL;
    }

    public static Val SetSceneLook_LookFromActor(Context context, int actorID, float3 displacementXYZ, float3 displacementRelative)
    {
      context.Engine.PlayerCameraInfo.SceneLook.SetPosition_Actor(actorID, displacementXYZ.ToVec3(), displacementRelative.ToVec3());
      return Val.NULL;
    }

    public static Val SetSceneLook_LookFromPoint(Context context, float3 point)
    {
      context.Engine.PlayerCameraInfo.SceneLook.SetPosition_Point(point.ToVec3());
      return Val.NULL;
    }
  }
}
