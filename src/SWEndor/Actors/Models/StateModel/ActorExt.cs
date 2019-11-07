using SWEndor.Models;

namespace SWEndor.Actors
{
  public partial class ActorInfo
  {
    public CreationState CreationState { get { return State.CreationState; } }
    public bool Planned { get { return State.CreationState == CreationState.PLANNED; } }
    public bool Generated { get { return State.CreationState == CreationState.GENERATED; } }
    public bool Active { get { return State.CreationState == CreationState.ACTIVE; } }
    public bool MarkedDisposing { get { return State.CreationState <= CreationState.PREDISPOSE; } }
    public bool Disposing { get { return State.CreationState == CreationState.DISPOSING; } }
    public bool DisposingOrDisposed { get { return State.CreationState <= CreationState.DISPOSING; } }
    public bool Disposed { get { return State.CreationState == CreationState.DISPOSED; } }

    public ComponentMask Mask { get { return State.ComponentMask; } }

    public void AdvanceDeathOneLevel()
    {
      State.AdvanceDeathOneLevel(this);
#if DEBUG
      if (Logged)
        Log.Write(Log.DEBUG, LogType.ACTOR_CREATIONSTATECHANGED, this, State.CreationState);
#endif
    }

    public void SetGenerated()
    {
      State.SetGenerated();
#if DEBUG
  if (Logged)
        Log.Write(Log.DEBUG, LogType.ACTOR_CREATIONSTATECHANGED, this, State.CreationState); 
#endif
    }

    public void SetActivated() { State.SetActivated();
#if DEBUG
     if (Logged)
        Log.Write(Log.DEBUG, LogType.ACTOR_CREATIONSTATECHANGED, this, State.CreationState);
#endif
    }

    public void SetPreDispose()
    {
      State.SetPreDispose();
#if DEBUG
      if (Logged)
        Log.Write(Log.DEBUG, LogType.ACTOR_CREATIONSTATECHANGED, this, State.CreationState);
#endif
    }

    public void SetDisposing() { State.SetDisposing();
#if DEBUG
      if (Logged)
        Log.Write(Log.DEBUG, LogType.ACTOR_CREATIONSTATECHANGED, this, State.CreationState);
#endif
    }

    public void SetDisposed() { State.SetDisposed();
#if DEBUG
      if (Logged)
        Log.Write(Log.DEBUG, LogType.ACTOR_CREATIONSTATECHANGED, this, State.CreationState);
#endif
    }

    public void ResetPlanned() { State.ResetPlanned();
#if DEBUG
     if (Logged)
        Log.Write(Log.DEBUG, LogType.ACTOR_CREATIONSTATECHANGED, this, State.CreationState);
#endif
    }

    public void SetState_Dead()
    {
      State.MakeDead(this);
#if DEBUG
    if (Logged)
        Log.Write(Log.DEBUG, LogType.ACTOR_ACTORSTATECHANGED, this, State.ActorState);
#endif
    }

    public void SetState_Dying()
    {
      State.MakeDying(this);
#if DEBUG
    if (Logged)
        Log.Write(Log.DEBUG, LogType.ACTOR_ACTORSTATECHANGED, this, State.ActorState);
#endif
    }

    public void SetState_Normal()
    {
      State.MakeNormal(this);
#if DEBUG
      if (Logged)
        Log.Write(Log.DEBUG, LogType.ACTOR_ACTORSTATECHANGED, this, State.ActorState);
#endif
    }


    public ActorState ActorState { get { return State.ActorState; } }
    public bool IsDying { get { return State.IsDying; } }
    public bool IsDead { get { return State.IsDead; } }
    public bool IsDyingOrDead { get { return State.IsDyingOrDead; } }

    public float CreationTime { get { return State.CreationTime; } }
  }
}