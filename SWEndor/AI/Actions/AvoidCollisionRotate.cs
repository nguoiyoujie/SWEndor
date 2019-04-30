using MTV3D65;
using SWEndor.Actors;

namespace SWEndor.AI.Actions
{
  public class AvoidCollisionRotate : ActionInfo
  {
    public AvoidCollisionRotate(TV_3DVECTOR impact_position, TV_3DVECTOR normal_vec, float close_enough_angle = 0.1f) : base("AvoidCollisionRotate")
    {
      Impact_Position = impact_position;
      Normal = normal_vec;
      CloseEnoughAngle = close_enough_angle;
      CanInterrupt = false;
    }

    // parameters
    public TV_3DVECTOR Impact_Position = new TV_3DVECTOR();
    public TV_3DVECTOR Target_Position = new TV_3DVECTOR();
    public TV_3DVECTOR Normal = new TV_3DVECTOR();
    private bool calcAvoidAngle = false;
    public float AvoidanceAngle = 90;
    public float CloseEnoughAngle = 0.1f;

    public override string ToString()
    {
      return string.Format("{0},{1},{2},{3},{4}"
                          , Name
                          , Utilities.ToString(Impact_Position)
                          , Utilities.ToString(Normal)
                          , CloseEnoughAngle
                          , Complete
                          );
    }

    public override void Process(Engine engine, int actorID)
    {
      ActorInfo actor = engine.ActorFactory.Get(actorID);
      if (actor.MovementInfo.MaxTurnRate == 0)
      {
        Complete = true;
        return;
      }

      if (CheckBounds(actor))
      {
        if (actor.CollisionInfo.ProspectiveCollisionLevel > 0 && actor.CollisionInfo.ProspectiveCollisionLevel < 5)
          Target_Position = actor.CollisionInfo.ProspectiveCollisionSafe;
        else
          Target_Position = Impact_Position + Normal * 10000;
        float dist = engine.TrueVision.TVMathLibrary.GetDistanceVec3D(actor.GetPosition(), Impact_Position);
        float Target_Speed = actor.MovementInfo.MinSpeed; //dist / 25;

        float delta_angle = AdjustRotation(actor, Target_Position);
        float delta_speed = AdjustSpeed(actor, Target_Speed);

        Complete |= (delta_angle <= CloseEnoughAngle && delta_angle >= -CloseEnoughAngle); //&& delta_speed == 0);
      }

      if (CheckImminentCollision(actor, actor.MovementInfo.Speed * 2.5f))
      {
        float newavoid = GetAvoidanceAngle(actor, actor.GetDirection(), Normal);
        float concavecheck = 60;
        if (!calcAvoidAngle || (AvoidanceAngle - newavoid > -concavecheck && AvoidanceAngle - newavoid < concavecheck))
        {
          AvoidanceAngle = newavoid;
          Impact_Position = actor.CollisionInfo.ProspectiveCollisionImpact;
          Normal = actor.CollisionInfo.ProspectiveCollisionNormal;
          calcAvoidAngle = true;
        }
        else
        { }
        actor.CollisionInfo.IsAvoidingCollision = true;
        Complete = false;
      }
      else
      {
        actor.CollisionInfo.IsAvoidingCollision = false;
        engine.ActionManager.QueueNext(actorID, new Wait(2.5f));
        Complete = true;
      }
    }

    private float GetAvoidanceAngle(ActorInfo owner, TV_3DVECTOR travelling_vec, TV_3DVECTOR impact_normal)
    {
      //get an orthogonal direction to travelling_vec on the xz plane
      TV_3DVECTOR xzdir = new TV_3DVECTOR();
      owner.GetEngine().TrueVision.TVMathLibrary.TVVec3Normalize(ref xzdir, new TV_3DVECTOR(travelling_vec.z, 0, -travelling_vec.x));

      TV_3DVECTOR avoidvec = new TV_3DVECTOR();
      owner.GetEngine().TrueVision.TVMathLibrary.TVVec3Normalize(ref avoidvec, impact_normal - owner.GetEngine().TrueVision.TVMathLibrary.VDotProduct(impact_normal, travelling_vec) * travelling_vec);
      float val = owner.GetEngine().TrueVision.TVMathLibrary.VDotProduct(avoidvec, xzdir);
      return owner.GetEngine().TrueVision.TVMathLibrary.ACos(val);
    }
  }
}
