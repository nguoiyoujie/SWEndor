using SWEndor.Game.Actors;
using SWEndor.Game.Models;

namespace SWEndor.Game.Projectiles
{
  public partial class ProjectileInfo
  {
    public bool CanCollide { get { return true; } }

    public bool CanCollideWith(ActorInfo checkActor)
    {
      return checkActor != null
                 && Owner?.TopParent != checkActor.TopParent
                 && checkActor.Mask.Has(ComponentMask.CAN_BECOLLIDED)
                 && !checkActor.IsAggregateMode;
    }

    public void DoCollide(ActorInfo target, ref CollisionResultData data)
    {
      target.TypeInfo.ProcessHit(Engine, target, this, data.Impact);
      TypeInfo.ProcessHit(Engine, this, target, data.Impact);
    }
  }
}
