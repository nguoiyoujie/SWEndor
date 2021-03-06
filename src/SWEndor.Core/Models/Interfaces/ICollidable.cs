﻿using SWEndor.Actors;

namespace SWEndor.Models
{
  /// <summary>
  /// Represents a collidable object
  /// </summary>
  public interface ICollidable
  {
    bool CanCollide { get; }
    bool CanCollideWith(ActorInfo checkActor);
    void DoCollide(ActorInfo target, ref CollisionResultData data);
    void EnableCollision(bool enable);
  }
}