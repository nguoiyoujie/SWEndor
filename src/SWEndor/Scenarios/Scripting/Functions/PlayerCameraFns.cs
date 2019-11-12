using MTV3D65;
using Primrose.Expressions;
using Primrose.Primitives.ValueTypes;
using SWEndor.Primitives.Extensions;
using SWEndor.Scenarios.Scripting.Expressions;
using System;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class PlayerCameraFns
  {
    public static Val SetPlayerLook(Context context, Val[] ps)
    {
      context.Engine.PlayerCameraInfo.SetPlayerLook();
      return Val.NULL;
    }

    public static Val SetSceneLook(Context context, Val[] ps)
    {
      context.Engine.PlayerCameraInfo.SetSceneLook();
      return Val.NULL;
    }

    public static Val SetDeathLook(Context context, Val[] ps)
    {
      context.Engine.PlayerCameraInfo.SetDeathLook();
      return Val.NULL;
    }

    public static Val SetSceneLook_LookAtActor(Context context, Val[] ps)
    {
      switch (ps.Length)
      {
        case 0:
          throw new Exception("At least one parameter is required");

        case 1:
          context.Engine.PlayerCameraInfo.SceneLook.SetTarget_LookAtActor((int)ps[0]);
          break;

        case 2:
          context.Engine.PlayerCameraInfo.SceneLook.SetTarget_LookAtActor((int)ps[0], ((float3)ps[1]).ToVec3());
          break;

        default:
        case 3:
          context.Engine.PlayerCameraInfo.SceneLook.SetTarget_LookAtActor((int)ps[0], ((float3)ps[1]).ToVec3(), ((float3)ps[2]).ToVec3());
          break;
      }
      return Val.NULL;
    }

    public static Val SetSceneLook_LookAtPoint(Context context, Val[] ps)
    {
      context.Engine.PlayerCameraInfo.SceneLook.SetTarget_LookAtPoint(((float3)ps[0]).ToVec3());
      return Val.NULL;
    }

    public static Val SetSceneLook_LookFromActor(Context context, Val[] ps)
    {
      switch (ps.Length)
      {
        case 0:
          throw new Exception("At least one parameter is required");

        case 1:
          context.Engine.PlayerCameraInfo.SceneLook.SetPosition_Actor((int)ps[0]);
          break;

        case 2:
          context.Engine.PlayerCameraInfo.SceneLook.SetPosition_Actor((int)ps[0], ((float3)ps[1]).ToVec3());
          break;

        default:
        case 3:
          context.Engine.PlayerCameraInfo.SceneLook.SetPosition_Actor((int)ps[0], ((float3)ps[1]).ToVec3(), ((float3)ps[2]).ToVec3());
          break;
      }
      return Val.NULL;
    }

    public static Val SetSceneLook_LookFromPoint(Context context, Val[] ps)
    {
      context.Engine.PlayerCameraInfo.SceneLook.SetPosition_Point(((float3)ps[0]).ToVec3());
      return Val.NULL;
    }
  }
}
