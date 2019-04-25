﻿using MTV3D65;
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

    public override void Process(ActorInfo owner)
    {
      if (owner.MovementInfo.MaxTurnRate == 0)
      {
        Complete = true;
        return;
      }

      if (CheckBounds(owner))
      {
        if (owner.CollisionInfo.ProspectiveCollisionLevel > 0 && owner.CollisionInfo.ProspectiveCollisionLevel < 5)
          Target_Position = owner.CollisionInfo.ProspectiveCollisionSafe;
        else
          Target_Position = Impact_Position + Normal * 10000;
        float dist = Globals.Engine.TrueVision.TVMathLibrary.GetDistanceVec3D(owner.GetPosition(), Impact_Position);
        float Target_Speed = owner.MovementInfo.MinSpeed; //dist / 25;

        float delta_angle = AdjustRotation(owner, Target_Position);
        float delta_speed = AdjustSpeed(owner, Target_Speed);

        Complete |= (delta_angle <= CloseEnoughAngle && delta_angle >= -CloseEnoughAngle); //&& delta_speed == 0);
      }

      if (CheckImminentCollision(owner, owner.MovementInfo.Speed * 2.5f))
      {
        float newavoid = GetAvoidanceAngle(owner.GetDirection(), Normal);
        float concavecheck = 60;
        if (!calcAvoidAngle || (AvoidanceAngle - newavoid > -concavecheck && AvoidanceAngle - newavoid < concavecheck))
        {
          AvoidanceAngle = newavoid;
          Impact_Position = owner.CollisionInfo.ProspectiveCollisionImpact;
          Normal = owner.CollisionInfo.ProspectiveCollisionNormal;
          calcAvoidAngle = true;
        }
        else
        { }
        owner.CollisionInfo.IsAvoidingCollision = true;
        Complete = false;
      }
      else
      {
        owner.CollisionInfo.IsAvoidingCollision = false;
        owner.Owner.Engine.ActionManager.QueueNext(owner.ID, new Wait(2.5f));
        Complete = true;
      }
    }

    private float GetAvoidanceAngle(TV_3DVECTOR travelling_vec, TV_3DVECTOR impact_normal)
    {
      //get an orthogonal direction to travelling_vec on the xz plane
      TV_3DVECTOR xzdir = new TV_3DVECTOR();
      Globals.Engine.TrueVision.TVMathLibrary.TVVec3Normalize(ref xzdir, new TV_3DVECTOR(travelling_vec.z, 0, -travelling_vec.x));

      TV_3DVECTOR avoidvec = new TV_3DVECTOR();
      Globals.Engine.TrueVision.TVMathLibrary.TVVec3Normalize(ref avoidvec, impact_normal - Globals.Engine.TrueVision.TVMathLibrary.VDotProduct(impact_normal, travelling_vec) * travelling_vec);
      float val = Globals.Engine.TrueVision.TVMathLibrary.VDotProduct(avoidvec, xzdir);
      return Globals.Engine.TrueVision.TVMathLibrary.ACos(val);
    }
  }
}
