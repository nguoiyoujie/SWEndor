using MTV3D65;
using Primrose.Primitives.ValueTypes;
using SWEndor.Core;
using SWEndor.Models;

namespace SWEndor.Actors.Models
{
  /// <summary>
  /// Storage of AI states per actor, transferrable across actions
  /// </summary>
  internal struct AIModel
  {
    private enum TargetMode { POINT, ACTOR, ACTOR_SMARTPREDICTION }

    internal bool CanEvade;
    internal bool CanRetaliate;
    internal int HuntWeight;

    internal float AIMinSpeed;
    internal float AIMaxSpeed;


    // CombatZone // TO-DO: Change this to int when multiple Combat Zones is possible
    public int CombatZone;

    // Targeting
    public TargetInfo Target;
    private float targetSpeed;

    // Distance Control
    public float FollowDistance { get; private set; }
    public void SetFollowDistance(ActorInfo owner, float value) { FollowDistance = value < 0 ? owner.TypeInfo.AIData.Move_CloseEnough : value; }

    public void Reset()
    {
      CanEvade = true;
      CanRetaliate = true;
      HuntWeight = 1;
      CombatZone = -1;
      AIMinSpeed = -1;
      AIMaxSpeed = -1;
    }

    public void Init(ref ActorTypes.Components.AIData data, ref ActorTypes.Components.MoveLimitData mdata)
    {
      CanEvade = data.CanEvade;
      CanRetaliate = data.CanRetaliate;
      HuntWeight = data.HuntWeight;
      CombatZone = -1;
      AIMinSpeed = data.AIMinSpeed == -1 ? mdata.MinSpeed : data.AIMinSpeed;
      AIMaxSpeed = data.AIMaxSpeed == -1 ? mdata.MaxSpeed : data.AIMaxSpeed;
    }

    public void SetTargetSpeed(float speed) { targetSpeed = speed; }

    internal float AdjustRotation(Engine engine, ActorInfo owner, float responsiveness = 10)
    {
      Target.Update(engine, owner);
      return AIUpdate.AdjustRotation(engine, owner, Target.Position, responsiveness);
    }

    internal float AdjustSpeed(ActorInfo owner, bool useAILimit)
    {
      return AIUpdate.AdjustSpeed(owner.Engine, owner, useAILimit, ref targetSpeed);
    }
  }
}
