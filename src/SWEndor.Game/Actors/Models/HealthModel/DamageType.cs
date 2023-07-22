namespace SWEndor.Game.Actors
{
  public enum DamageType : byte
  {
    // special
    NONE,
    ALWAYS_100PERCENT,

    // hull
    COLLISION,

    // laser
    LASER,
    TURBOLASER, // heavy laser

    // proj
    MISSILE,
    TORPEDO,
  }
}
