using SWEndor.Game.Actors.Data;
using SWEndor.Game.Core;
using SWEndor.Game.Models;
using SWEndor.Game.Projectiles;

namespace SWEndor.Game.ProjectileTypes.Components
{
  internal struct MoveBehavior
  {
    internal delegate void MoveAction(Engine engine, ProjectileInfo actor, ref MoveData data, float time);
    internal MoveAction Move;

    public void Load(ProjectileTypeInfo atype)
    {
      if (atype.CombatData.IsTeleport)
        Move = MoveTeleport.Move;

      else if (atype.CombatData.IsLaser)
        Move = MoveLaser.Move;

      else if (atype.CombatData.IsMissile)
        Move = MoveMissile.Move;

      else if (atype.Mask.Has(ComponentMask.CAN_MOVE | ComponentMask.CAN_ROTATE))
        Move = MoveNormal.Move;

      else if (atype.Mask.Has(ComponentMask.CAN_MOVE))
        Move = MoveForwardOnly.Move;

      else if (atype.Mask.Has(ComponentMask.CAN_ROTATE))
        Move = RotateOnly.Move;

      else
        Move = NoMove.Move;
    }
  }
}
