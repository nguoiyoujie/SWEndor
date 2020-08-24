using SWEndor.Game.Actors;
using SWEndor.Game.Core;
using Primrose.Primitives;
using Primrose.Primitives.Factories;
using SWEndor.Game.Sound;

namespace SWEndor.Game.AI.Actions
{
  internal class SetMood : ActionInfo
  {
    private static ObjectPool<SetMood> _pool;
    static SetMood() { _pool = ObjectPool<SetMood>.CreateStaticPool(() => { return new SetMood(); }, (a) => { a.Reset(); }); }

    private SetMood() : base("SetMood") { CanInterrupt = false; }

    public static SetMood GetOrCreate(MoodState mood, bool squadLeaderOnly)
    {
      SetMood h = _pool.GetNew();

      h.Mood = mood;
      h.SquadLeaderOnly = squadLeaderOnly;
      h.IsDisposed = false;
      return h;
    }

    public override void Reset()
    {
      base.Reset();
    }

    public override void Return()
    {
      base.Return();
      _pool.Return(this);

    }

    // parameters
    public MoodState Mood = 0;
    public bool SquadLeaderOnly = false;

    public override string ToString()
    {
      return string.Join(",", new string[]
      {
          Name
        , Mood.GetEnumName()
        , SquadLeaderOnly.ToString()
        , Complete.ToString()
      });
    }

    public override void Process(Engine engine, ActorInfo actor)
    {
      if (!SquadLeaderOnly || actor.Squad.IsNull || actor.Squad.Leader == actor)
        engine.SoundManager.SetMood(Mood);
      Complete = true;
    }
  }
}
