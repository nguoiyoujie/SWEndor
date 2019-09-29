namespace SWEndor.Scenarios
{
  public enum MoodStates //: byte
  {
    AMBIENT = 0,
    ENGAGEMENT = 1,
    //SUCCESS = 10,
    //FAILURE = 11,

    // Negative numbers represent interrupting states for intermittent tracks
    DESTROY_FIGHTER = -1,
    //DESTROYED = -2,
    DESTROY_SHIP = -3,

    ALLY_FIGHTER_ARRIVED = -11,
    ALLY_SHIP_ARRIVED = -12,
    ALLY_FIGHTER_LOST = -13,
    ALLY_SHIP_LOST = -14,
    ENEMY_FIGHTER_ARRIVED = -21,
    ENEMY_SHIP_ARRIVED = -22,
    NEUTRAL_FIGHTER_ARRIVED = -31,
    NEUTRAL_SHIP_ARRIVED = -32,
  }
}
