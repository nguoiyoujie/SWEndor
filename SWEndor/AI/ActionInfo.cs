using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Primitives;
using System;

namespace SWEndor.AI.Actions
{
  public class ActionInfo
  {
    public ActionInfo(string name)
    {
      Name = name;
    }

    // parameters
    public string Name { get; private set; }
    public bool Complete = false;
    public bool CanInterrupt = true;
    public ActionInfo NextAction = null;

    // collision avoidance
    private float m_collisioncheck_time = 0;


    public override string ToString()
    {
      return "{0},{1}".F(Name
                       , Complete
                       );
    }

    public virtual void Process(Engine engine, ActorInfo owner)
    {
      Complete = true;
    }

    protected bool CheckBounds(ActorInfo owner)
    {
      float boundmult = 0.99f;
      if (!(owner.TypeInfo is ActorTypes.Groups.Projectile) 
        && owner.IsOutOfBounds(owner.GetEngine().GameScenarioManager.MinAIBounds * boundmult, owner.GetEngine().GameScenarioManager.MaxAIBounds * boundmult) 
        && owner.AIData.EnteredCombatZone)
      {
        float x = owner.GetEngine().Random.Next((int)(owner.GetEngine().GameScenarioManager.MinAIBounds.x * 0.65f), (int)(owner.GetEngine().GameScenarioManager.MaxAIBounds.x * 0.65f));
        float y = owner.GetEngine().Random.Next(-200, 200);
        float z = owner.GetEngine().Random.Next((int)(owner.GetEngine().GameScenarioManager.MinAIBounds.z * 0.65f), (int)(owner.GetEngine().GameScenarioManager.MaxAIBounds.z * 0.65f));

        if (owner.CurrentAction is Move)
          owner.CurrentAction.Complete = true;
        owner.QueueFirst(new ForcedMove(new TV_3DVECTOR(x, y, z), owner.MoveData.MaxSpeed, -1, 360 / (owner.MoveData.MaxTurnRate + 72)));
        return false;
      }
      else
      {
        if (!owner.IsOutOfBounds(owner.GetEngine().GameScenarioManager.MinAIBounds * boundmult, owner.GetEngine().GameScenarioManager.MaxAIBounds * boundmult))
        {
          owner.AIData.EnteredCombatZone = true;
        }
      }
      return true;
    }

    protected bool CheckImminentCollision(ActorInfo owner)
    {
      if (!owner.TypeInfo.AIData.CanCheckCollisionAhead)
        return false;

      if (!owner.Engine.MaskDataSet[owner].Has(ComponentMask.CAN_BECOLLIDED))
        return false;

      float dist = owner.MoveData.Speed * Globals.ImminentCollisionFactor * (1 + (Math.Abs(owner.MoveData.XTurnAngle) + Math.Abs(owner.MoveData.YTurnAngle)) / 30f) ; // turning plays a role

      if (dist <= 0)
        return false;

      return CollisionSystem.ActivateImminentCollisionCheck(owner.Engine, owner, ref m_collisioncheck_time, dist);
    }

    public void Dispose()
    {
      NextAction = null;
      Complete = true;
    }
  }
}
