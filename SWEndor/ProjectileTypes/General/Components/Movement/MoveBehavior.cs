using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.Core;
using SWEndor.Models;
using SWEndor.Projectiles;

namespace SWEndor.ProjectileTypes.Components
{
  public struct MoveBehavior
  {
    public delegate void MoveAction(Engine engine, ProjectileInfo actor, ref MoveData data, float time);
    public MoveAction Move;

    public void Load(ProjectileTypeInfo atype)
    {
      if (atype.Mask.Has(ComponentMask.CAN_MOVE | ComponentMask.CAN_ROTATE))
        Move = MoveNormal.Move;

      else if (atype.CombatData.IsLaser)
        Move = MoveLaser.Move;

      else if (atype.Mask.Has(ComponentMask.CAN_MOVE))
        Move = MoveForwardOnly.Move;

      else if (atype.Mask.Has(ComponentMask.CAN_ROTATE))
        Move = RotateOnly.Move;

      else
        Move = NoMove.Move;
    }
  }
}
