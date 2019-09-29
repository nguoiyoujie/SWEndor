using SWEndor.Models;

namespace SWEndor.Explosions
{
  public partial class ExplosionInfo
  {
    public CreationState CreationState { get { return State.CreationState; } }
    public bool Planned { get { return State.CreationState == CreationState.PLANNED; } }
    public bool Generated { get { return State.CreationState == CreationState.GENERATED; } }
    public bool Active { get { return State.CreationState == CreationState.ACTIVE; } }
    public bool Disposing { get { return State.CreationState == CreationState.DISPOSING; } }
    public bool DisposingOrDisposed { get { return State.CreationState < 0; } }
    public bool Disposed { get { return State.CreationState == CreationState.DISPOSED; } }

    public ComponentMask Mask { get { return State.ComponentMask; } }

    public void AdvanceDeathOneLevel() { State.AdvanceDeathOneLevel(this); }

    public void SetGenerated() { State.SetGenerated(); }
    public void SetActivated() { State.SetActivated(); }
    public void SetDisposing() { State.SetDisposing(); }
    public void SetDisposed() { State.SetDisposed(); }
    public void ResetPlanned() { State.ResetPlanned(); }

    public void SetState_Dead() { State.MakeDead(this); }
    public void SetState_Dying() { State.MakeDying(this); }
    public void SetState_Normal() { State.MakeNormal(this); }

    public ActorState ActorState { get { return State.ActorState; } }
    public bool IsDying { get { return State.IsDying; } }
    public bool IsDead { get { return State.IsDead; } }
    public bool IsDyingOrDead { get { return State.IsDyingOrDead; } }

    public float CreationTime { get { return State.CreationTime; } }
  }
}