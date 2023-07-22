using SWEndor.Game.Actors;
using SWEndor.Game.Models;

namespace SWEndor
{
  public delegate void GameEvent();
  public delegate void GameEvent<T>(T arg);
  public delegate void GameEvent<T1, T2>(T1 a1, T2 a2);
  public delegate void GameEvent<T1, T2, T3>(T1 a1, T2 a2, T3 a3);
  public delegate void ActorEvent(ActorInfo actor);
  public delegate void ActorAttackedEvent(ActorInfo actor, ActorInfo victim);
  public delegate void ActorAttackEvent(ActorInfo actor, ActorInfo attacker);
  public delegate void HitEvent(ActorInfo actor, ActorInfo attacker);
  public delegate void ActorStateChangeEvent(ActorInfo actor, ActorState state);
  public delegate void ShipSpawnEvent(ShipSpawnEventArg eventArg);
}
