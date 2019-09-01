﻿using SWEndor.Actors.Data;

namespace SWEndor.Actors.Components
{
  /// <summary>
  /// Implementation of IDyingMoveComponent that kills the actor immediately
  /// </summary>
  public class DyingKill : IDyingMoveComponent
  {
    public static readonly DyingKill Instance = new DyingKill();
    private DyingKill() { }

    public void Initialize(ActorInfo actor, ref MoveData data) { actor.SetState_Dead(); }
    public void Update(ActorInfo actor, ref MoveData data, float time) { }
  }
}
