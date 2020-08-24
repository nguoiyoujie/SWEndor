using Primrose.Primitives.Factories;
using SWEndor.Game.Actors;
using SWEndor.Game.Core;

namespace SWEndor.Game.AI.Actions
{
  internal class PlaySound : ActionInfo
  {

    private static ObjectPool<PlaySound> _pool;
    static PlaySound() { _pool = ObjectPool<PlaySound>.CreateStaticPool(() => { return new PlaySound(); }, (a) => { a.Reset(); }); }

    private PlaySound() : base("PlaySound") { CanInterrupt = false; }

    public static PlaySound GetOrCreate(string name, bool squadLeaderOnly)
    {
      PlaySound h = _pool.GetNew();

      h.SoundName = name;
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
    public string SoundName;
    public bool SquadLeaderOnly = false;

    public override string ToString()
    {
      return string.Join(",", new string[]
      {
          Name
        , SoundName
        , Complete.ToString()
      });
    }

    public override void Process(Engine engine, ActorInfo actor)
    {
      if (!SquadLeaderOnly || actor.Squad.IsNull || actor.Squad.Leader == actor)
        engine.SoundManager.SetSound(SoundName);
      Complete = true;
    }
  }
}
