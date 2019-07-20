using MTV3D65;
using SWEndor.Actors.Data;
using SWEndor.Actors.Traits;
using SWEndor.AI.Actions;

namespace SWEndor.Actors.Components
{
  public struct MoveNormal : IMoveComponent
  {
    public static readonly MoveNormal Instance = new MoveNormal();

    public void Move(ActorInfo actor, ref MoveData data)
    {
      float time = actor.Game.TimeSinceRender;

      // Hyperspace special: AI loop may not be in sync
      if (actor.CurrentAction is HyperspaceIn)
      {
        ((HyperspaceIn)actor.CurrentAction).ApplyMove(actor);
        actor.MoveRelative(data.Speed * time);
        return;
      }
      else if (actor.CurrentAction is HyperspaceOut)
      {
        ((HyperspaceOut)actor.CurrentAction).ApplyMove(actor);
        actor.MoveRelative(data.Speed * time);
        return;
      }

      // Control speed
      if (!data.FreeSpeed)
        data.Speed = data.Speed.Clamp(data.MinSpeed, data.MaxSpeed);

      // Control rotation
      if (data.ApplyZBalance)
      {
        TV_3DVECTOR vec = actor.Transform.Rotation;
        actor.Transform.Rotation = new TV_3DVECTOR(vec.x, vec.y, 0);
        actor.MoveRelative(data.Speed * time);
        data.ZRoll -= data.YTurnAngle * data.ZTilt * time;

        // Z rotation decay.
        data.Zdiv += time / 0.005f;
        while (data.Zdiv > 0 && !float.IsInfinity(data.Zdiv))
        {
          data.ZRoll *= 1 - data.ZNormFrac;
          data.Zdiv--;
        }

        float rotX2 = vec.x + data.XTurnAngle * time;
        rotX2 = rotX2.Clamp(-actor.TypeInfo.XLimit, actor.TypeInfo.XLimit);
        float rotY2 = vec.y + data.YTurnAngle * time;

        actor.Transform.Rotation = new TV_3DVECTOR(rotX2, rotY2, data.ZRoll);
      }
      else
      {
        TV_3DVECTOR vec = actor.Transform.Rotation;
        actor.MoveRelative(data.Speed * time);
        data.ZRoll = vec.z;
        data.ZRoll -= data.YTurnAngle * data.ZTilt * time;
        float rotX2 = vec.x + data.XTurnAngle * time;
        rotX2 = rotX2.Clamp(-actor.TypeInfo.XLimit, actor.TypeInfo.XLimit);
        float rotY2 = vec.y + data.YTurnAngle * time;
        actor.Transform.Rotation = new TV_3DVECTOR(rotX2, rotY2, data.ZRoll);
      }
    }
  }
}
