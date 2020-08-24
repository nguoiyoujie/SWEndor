using SWEndor.Game.Models;

namespace SWEndor.Game.Projectiles
{
  public partial class ProjectileInfo
  {
    /// <summary>The component present in this instance</summary>
    public ComponentMask Mask { get { return State.ComponentMask; } }

    /// <summary>The in-game time when the object was created or rebuilt</summary>
    public float CreationTime { get { return State.CreationTime; } }

    /// <summary>The creation state of this instance</summary>
    public CreationState CreationState { get { return State.CreationState; } }

    /// <summary>Returns whether the creation state is PLANNED</summary>
    public bool Planned { get { return State.CreationState == CreationState.PLANNED; } }

    /// <summary>Returns whether the creation state is GENERATED</summary>
    public bool Generated { get { return State.CreationState == CreationState.GENERATED; } }

    /// <summary>Returns whether the creation state is ACTIVE</summary>
    public bool Active { get { return State.CreationState == CreationState.ACTIVE; } }

    /// <summary>Returns whether the instance has been marked for disposal</summary>
    public bool MarkedDisposing { get { return State.CreationState <= CreationState.PREDISPOSE; } }

    /// <summary>Returns whether the creation state is DISPOSING</summary>
    public bool Disposing { get { return State.CreationState == CreationState.DISPOSING; } }

    /// <summary>Returns whether the creation state is DISPOSING or already DISPOSED</summary>
    public bool DisposingOrDisposed { get { return State.CreationState <= CreationState.DISPOSING; } }

    /// <summary>Returns whether the creation state is DISPOSED</summary>
    public bool Disposed { get { return State.CreationState == CreationState.DISPOSED; } }

    /// <summary>Sets the creation state to GENERATED</summary>
    public void SetGenerated() { State.SetGenerated(); }

    /// <summary>Sets the creation state to ACTIVE</summary>
    public void SetActivated() { State.SetActivated(); }

    /// <summary>Sets the creation state to PREDISPOSE</summary>
    public void SetPreDispose() { State.SetPreDispose(); }

    /// <summary>Sets the creation state to DISPOSING</summary>
    public void SetDisposing() { State.SetDisposing(); }

    /// <summary>Sets the creation state to DISPOSED</summary>
    public void SetDisposed() { State.SetDisposed(); }

    /// <summary>Sets the creation state to PLANNED</summary>
    public void ResetPlanned() { State.ResetPlanned(); }

    /// <summary>Returns the actor state of this instance</summary>
    public ActorState ActorState { get { return State.ActorState; } }

    /// <summary>Returns whether the actor state is DYING</summary>
    public bool IsDying { get { return State.IsDying; } }

    /// <summary>Returns whether the actor state is DEAD</summary>
    public bool IsDead { get { return State.IsDead; } }

    /// <summary>Returns whether the actor state is DYING or DEAD</summary>
    public bool IsDyingOrDead { get { return State.IsDyingOrDead; } }

    /// <summary>Advances the actor state one step towards the DEAD state</summary>
    public void AdvanceDeathOneLevel() { State.AdvanceDeathOneLevel(this); }

    /// <summary>Sets the actor state to DEAD</summary>
    public void SetState_Dead() { State.MakeDead(this); }

    /// <summary>Sets the actor state to DYING</summary>
    public void SetState_Dying() { State.MakeDying(this); }

    /// <summary>Sets the actor state to NORMAL</summary>
    public void SetState_Normal() { State.MakeNormal(this); }
  }
}