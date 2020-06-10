using MTV3D65;
using SWEndor.Actors;
using SWEndor.Core;
using SWEndor.Models;
using Primrose.Primitives.Factories;
using SWEndor.Weapons;
using SWEndor.Primitives.Extensions;

namespace SWEndor.AI.Actions
{
  internal class AvoidCollisionRotate : ActionInfo
  {

    private static ObjectPool<AvoidCollisionRotate> _pool;
    static AvoidCollisionRotate() { _pool = ObjectPool<AvoidCollisionRotate>.CreateStaticPool(() => { return new AvoidCollisionRotate(); }, (a) => { a.Reset(); }); }

    private AvoidCollisionRotate() : base("AvoidCollisionRotate") { }

    // parameters
    public TV_3DVECTOR Impact_Position = new TV_3DVECTOR();
    public TV_3DVECTOR Target_Position = new TV_3DVECTOR();
    public TV_3DVECTOR Normal = new TV_3DVECTOR();
    private bool calcAvoidAngle = false;
    public float AvoidanceAngle = 90;
    public float CloseEnoughAngle = 0.1f;

    public static AvoidCollisionRotate GetOrCreate(TV_3DVECTOR impact_position, TV_3DVECTOR normal_vec, float close_enough_angle = 0.1f)
    {
      AvoidCollisionRotate h = _pool.GetNew();

      h.Impact_Position = impact_position;
      h.Normal = normal_vec;
      h.CloseEnoughAngle = close_enough_angle;
      h.CanInterrupt = false;
      return h;
    }

    public override void Reset()
    {
      base.Reset();
      Impact_Position = new TV_3DVECTOR();
      Target_Position = new TV_3DVECTOR();
      Normal = new TV_3DVECTOR();
      calcAvoidAngle = false;
      AvoidanceAngle = 90;
      CloseEnoughAngle = 0.1f;
    }

    public override void Return()
    {
      base.Return();
      _pool.Return(this);

    }

    public override string ToString()
    {
      return string.Join(",", new string[]
        {
          Name
        , Impact_Position.Str()
        , Normal.Str()
        , CloseEnoughAngle.ToString()
        , Complete.ToString()
        });
    }

    public override void Process(Engine engine, ActorInfo actor)
    {
      if (actor.MoveData.MaxTurnRate == 0)
      {
        Complete = true;
        return;
      }

      Process(engine, actor, ref actor.CollisionData);
    }


    private void Process(Engine engine, ActorInfo actor, ref CollisionModel<ActorInfo> data)
    {
      if (CheckBounds(actor))
      {
        if (data.ProspectiveCollisionLevel > 0 && data.ProspectiveCollisionLevel < 5)
          actor.AI.Target.Set(data.ProspectiveCollisionSafe);
        else
          actor.AI.Target.Set(Impact_Position + Normal * 10000);
        float dist = engine.TrueVision.TVMathLibrary.GetDistanceVec3D(actor.GetGlobalPosition(), Impact_Position);

        actor.AI.SetTargetSpeed(actor.MoveData.MinSpeed);
        float delta_angle = actor.AI.AdjustRotation(engine, actor, 9999);
        float delta_speed = actor.AI.AdjustSpeed(actor, false);

        ActorInfo target = actor.ActorFactory.Get(data.ProspectiveCollision.ActorID);
        if (target != null && !actor.IsAlliedWith(target))
        {
          WeaponShotInfo w;
          actor.WeaponDefinitions.SelectWeapon(engine, actor, target, 0, dist, out w);
          if (!w.IsNull)
            w.Fire(engine, actor, target);
        }

        Complete |= (delta_angle <= CloseEnoughAngle && delta_angle >= -CloseEnoughAngle);
      }

      if (CheckImminentCollision(actor))
      {
        float newavoid = GetAvoidanceAngle(actor, actor.GetGlobalDirection(), Normal);
        float concavecheck = 60;
        if (!calcAvoidAngle || (AvoidanceAngle - newavoid > -concavecheck && AvoidanceAngle - newavoid < concavecheck))
        {
          AvoidanceAngle = newavoid;
          Impact_Position = data.ProspectiveCollision.Impact;
          Normal = data.ProspectiveCollision.Normal;
          calcAvoidAngle = true;
        }

        data.IsAvoidingCollision = true;
        Complete = false;
      }
      else
      {
        data.IsAvoidingCollision = false;
        Complete = true;
      }
    }

    private float GetAvoidanceAngle(ActorInfo owner, TV_3DVECTOR travelling_vec, TV_3DVECTOR impact_normal)
    {
      //get an orthogonal direction to travelling_vec on the xz plane
      TV_3DVECTOR xzdir = new TV_3DVECTOR();
      owner.Engine.TrueVision.TVMathLibrary.TVVec3Normalize(ref xzdir, new TV_3DVECTOR(travelling_vec.z, 0, -travelling_vec.x));

      TV_3DVECTOR avoidvec = new TV_3DVECTOR();
      owner.Engine.TrueVision.TVMathLibrary.TVVec3Normalize(ref avoidvec, impact_normal - owner.Engine.TrueVision.TVMathLibrary.VDotProduct(impact_normal, travelling_vec) * travelling_vec);
      float val = owner.Engine.TrueVision.TVMathLibrary.VDotProduct(avoidvec, xzdir);
      return owner.Engine.TrueVision.TVMathLibrary.ACos(val);
    }
  }
}
