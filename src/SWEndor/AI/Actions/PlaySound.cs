using Primrose.Primitives.Factories;
using SWEndor.Actors;
using SWEndor.Core;

namespace SWEndor.AI.Actions
{
  internal class PlaySound : ActionInfo
  {
    internal static int _count = 0;
    private static ObjectPool<PlaySound> _pool;
    static PlaySound() { _pool = ObjectPool<PlaySound>.CreateStaticPool(() => { return new PlaySound(); }, (a) => { a.Reset(); }); }

    private PlaySound() : base("PlaySound") { CanInterrupt = false; }

    public static PlaySound GetOrCreate(string name, bool squadLeaderOnly, bool interrupt = true)
    {
      PlaySound h = _pool.GetNew();
      _count++;
      h.SoundName = name;
      h.Interrupt = interrupt;
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
      _count--;
    }

    // parameters
    public string SoundName;
    public bool Interrupt;
    public bool SquadLeaderOnly = false;

    public override string ToString()
    {
      return string.Join(",", new string[]
      {
          Name
        , SoundName
        , Interrupt.ToString()
        , Complete.ToString()
      });
    }

    public override void Process(Engine engine, ActorInfo actor)
    {
      if (!SquadLeaderOnly || actor.Squad.IsNull || actor.Squad.Leader == actor)
        engine.SoundManager.SetSound(SoundName, Interrupt);
      Complete = true;
    }
  }
}
