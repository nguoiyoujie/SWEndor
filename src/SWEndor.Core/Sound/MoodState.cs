﻿namespace SWEndor.Sound
{
  public enum MoodState //: byte
  {
    AMBIENT = 0,
    AMBIENT_2 = 1,
    AMBIENT_3 = 2,
    AMBIENT_4 = 3,
    ENGAGEMENT = 4,

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
