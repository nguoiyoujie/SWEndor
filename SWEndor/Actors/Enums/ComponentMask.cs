using System;

namespace SWEndor.Actors
{
  [Flags]
  public enum ComponentMask
  {
    NONE = 0,

    // Render
    CAN_RENDER = 0x1,
    HAS_ANIMATED_TEXTURE = 0x2,

    // Movement
    CAN_MOVE = 0x10,
    CAN_ROTATE = 0x20,

    // Combat Systems
    IS_DAMAGE = 0x100,
    CAN_BETARGETED = 0x200,
    CAN_COLLIDE = 0x400,
    CAN_BECOLLIDED = 0x800, // used to enable Mesh Collision

    // Minor Systems
    CAN_REGEN = 0x1000,
    CAN_EXPLODE = 0x2000,
    HAS_AI = 0x4000,

    // Special
    CAN_GLOW = 0x10000,

    // Component Masks for performing
    USESYSTEM_RENDER = CAN_RENDER,
    USESYSTEM_ANIMATEDTEXTURE = CAN_RENDER | HAS_ANIMATED_TEXTURE,
    USESYSTEM_FWD_MOVEMENT = CAN_MOVE,
    USESYSTEM_ROT_MOVEMENT = CAN_MOVE | CAN_ROTATE,
    USESYSTEM_COLLISION = CAN_COLLIDE,
    USESYSTEM_EXPLODE = CAN_EXPLODE,
    USESYSTEM_REGEN = CAN_REGEN,
    USESYSTEM_AI = HAS_AI,


    // Component Masks for creating objects
    STATIC_PROP = CAN_RENDER,

    EXPLOSION = CAN_RENDER | HAS_ANIMATED_TEXTURE,

    LASER_PROJECTILE = CAN_RENDER | CAN_MOVE | IS_DAMAGE | CAN_COLLIDE | CAN_EXPLODE | CAN_GLOW,
    GUIDED_PROJECTILE = CAN_RENDER | CAN_MOVE | CAN_ROTATE | IS_DAMAGE | CAN_BETARGETED | CAN_COLLIDE | HAS_AI | CAN_EXPLODE,

    DEBRIS = CAN_RENDER | CAN_MOVE | CAN_ROTATE | CAN_EXPLODE,

    STATIC_ACTOR = CAN_RENDER | CAN_BETARGETED | CAN_BECOLLIDED | CAN_REGEN | CAN_EXPLODE,
    MINDLESS_ACTOR = CAN_RENDER | CAN_MOVE | CAN_ROTATE | CAN_BETARGETED | CAN_COLLIDE | CAN_BECOLLIDED | CAN_REGEN | CAN_EXPLODE,
    ACTOR = CAN_RENDER | CAN_MOVE | CAN_ROTATE | CAN_BETARGETED | CAN_COLLIDE | CAN_BECOLLIDED | HAS_AI | CAN_REGEN | CAN_EXPLODE
  }

  public static class ComponentMaskExt
  {
    public static bool Has(this ComponentMask mask, ComponentMask subset) { return (mask & subset) == subset; }
  }
}
