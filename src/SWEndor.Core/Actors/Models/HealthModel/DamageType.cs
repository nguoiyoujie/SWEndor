namespace SWEndor.Actors
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

    // proj
    MISSILE,
    TORPEDO,
  }
}
