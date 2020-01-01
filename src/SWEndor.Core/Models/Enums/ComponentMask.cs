using System;

namespace SWEndor.Models
{
  [Flags]
  public enum ComponentMask
  {
    /// <summary>This value represents nothing</summary>
    NONE = 0,

    // Render

    /// <summary>Represents a renderable object</summary>
    CAN_RENDER = 0x1,

    // Transform

    /// <summary>Represents a transformable object that can move</summary>
    CAN_MOVE = 0x10,

    /// <summary>Represents a transformable object that can rotate</summary>
    CAN_ROTATE = 0x20,

    // Combat

    /// <summary>Represents an object that can be targeted by the player's target system</summary>
    CAN_BETARGETED = 0x200,

    /// <summary>Represents an object that can be collided with</summary>
    CAN_BECOLLIDED = 0x800,

    /// <summary>Represents an object that can support 'AI'</summary>
    HAS_AI = 0x4000,



    // Short-hand objects

    /// <summary>Represents a static prop object</summary>
    STATIC_PROP = CAN_RENDER,

    /// <summary>Represents a static explosion object</summary>
    EXPLOSION = CAN_RENDER,

    /// <summary>Represents a laser projectile</summary>
    LASER_PROJECTILE = CAN_RENDER | CAN_MOVE,

    /// <summary>Represents a guided, homing projectile</summary>
    GUIDED_PROJECTILE = CAN_RENDER | CAN_MOVE | CAN_ROTATE | CAN_BETARGETED | HAS_AI,

    /// <summary>Represents a debris object</summary>
    DEBRIS = CAN_RENDER | CAN_MOVE | CAN_ROTATE,

    /// <summary>Represents a static actor object</summary>
    STATIC_ACTOR = CAN_RENDER | CAN_BETARGETED | CAN_BECOLLIDED,

    /// <summary>Represents a mindless actor object</summary>
    MINDLESS_ACTOR = CAN_RENDER | CAN_MOVE | CAN_ROTATE | CAN_BETARGETED | CAN_BECOLLIDED,

    /// <summary>Represents an actor onject</summary>
    ACTOR = CAN_RENDER | CAN_MOVE | CAN_ROTATE | CAN_BETARGETED | CAN_BECOLLIDED | HAS_AI
  }

  /// <summary>
  /// Provides extension methods for ComponentMask enum
  /// </summary>
  public static class ComponentMaskExt
  {
    /// <summary>Returns whether a component mask contains a subset</summary>
    /// <param name="mask">The mask to compare</param>
    /// <param name="subset">The subset to compare</param>
    /// <returns>Returns true if the mask contains all bits of the subset, false if otherwise</returns>
    public static bool Has(this ComponentMask mask, ComponentMask subset) { return (mask & subset) == subset; }
  }
}
