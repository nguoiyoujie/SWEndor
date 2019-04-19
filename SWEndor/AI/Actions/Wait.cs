using SWEndor.Actors;

namespace SWEndor.AI.Actions
{
  public class Wait : ActionInfo
  {
    public Wait(float time = 5) : base("Wait")
    {
      WaitTime = time;
    }

    private float WaitTime = 0;
    private float ResumeTime = 0;

    public override string ToString()
    {
      return string.Format("{0},{1},{2}"
                          , Name
                          , ResumeTime - Game.Instance().GameTime
                          , Complete
                          );
    }

    public override void Process(ActorInfo owner)
    {
      if (ResumeTime == 0)
      {
        ResumeTime = Game.Instance().GameTime + WaitTime;
      }

      AdjustRotation(owner, owner.GetRelativePositionXYZ(0, 0, 1000), false);
      if (CheckImminentCollision(owner, owner.MovementInfo.Speed * 2.5f))
      {
        ActionManager.QueueFirst(owner.ID, new AvoidCollisionRotate(owner.CollisionInfo.ProspectiveCollisionImpact, owner.CollisionInfo.ProspectiveCollisionNormal));
      }
      Complete |= (ResumeTime < Game.Instance().GameTime);
    }
  }
}
