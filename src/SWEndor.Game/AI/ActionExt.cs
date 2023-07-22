using SWEndor.Game.Actors;
using SWEndor.Game.AI.Actions;

namespace SWEndor.Game.AI
{
  /// <summary>
  /// Provides extension methods for feeding actions to an actor
  /// </summary>
  public static class ActionExt
  {
    /// <summary>Unblocks one Lock action</summary>
    public static void UnlockOne(this ActorInfo actor)
    {
      ActionInfo action = actor.CurrentAction;
      while (action != null)
      {
        if (action is Lock)
        {
          action.Complete = true;
          return;
        }
        action = action.NextAction;
      }
    }

    /// <summary>Clears the actor's action queue, until an uninterruptible action is found</summary>
    public static void ClearQueue(this ActorInfo actor)
    {
      ActionInfo action = actor.CurrentAction;
      while (action != null && action.CanInterrupt)
      {
        ActionInfo next = action.NextAction;
        action.Dispose();
        action = next;
      }
      actor.CurrentAction = action;
    }

    /// <summary>Clears the actor's action queue regardless of whether the action is uninterruptible</summary>
    public static void ForceClearQueue(this ActorInfo actor)
    {
      ActionInfo action = actor.CurrentAction;
      while (action != null)
      {
        ActionInfo next = action.NextAction;
        action.Dispose();
        action = next;
      }
      actor.CurrentAction = null;
    }

    /// <summary>Queues an action at the front of the action queue, replacing the current action</summary>
    public static void QueueFirst(this ActorInfo actor, ActionInfo action)
    {
      if (actor.CurrentAction == null)
        actor.CurrentAction = action;
      else
      {
        ActionInfo aend = actor.CurrentAction;
        ActionInfo amid = action;
        actor.CurrentAction = action;
        while (amid.NextAction != null)
        {
          amid = amid.NextAction;
        }
        amid.NextAction = aend;
      }
    }

    /// <summary>Queues an action immediately after the current action</summary>
    public static void QueueNext(this ActorInfo actor, ActionInfo action)
    {
      if (actor.CurrentAction == null)
        actor.CurrentAction = action;
      else
      {
        ActionInfo aend = actor.CurrentAction.NextAction;
        ActionInfo amid = action;
        actor.CurrentAction.NextAction = action;
        while (amid.NextAction != null)
        {
          amid = amid.NextAction;
        }
        amid.NextAction = aend;
      }
    }

    /// <summary>Queues an action at the back of the action queue</summary>
    public static void QueueLast(this ActorInfo actor, ActionInfo action)
    {
      if (actor.CurrentAction == null)
        actor.CurrentAction = action;
      else
      {
        ActionInfo aend = actor.CurrentAction;
        while (aend.NextAction != null)
        {
          aend = aend.NextAction;
        }
        aend.NextAction = action;
      }
    }

    /// <summary>Processes the current action, moving to the next if the current action is complete</summary>
    public static void Run(this ActorInfo actor)
    {
      ActionInfo action = actor.CurrentAction;

      if (action == null)
      {
        actor.CurrentAction = actor.Squad.GetNewAction(actor, actor.Engine) ?? Idle.GetOrCreate();
      }
      else
      {
        if (action.Complete)
        {
          if (action.NextAction == null)
          {
            actor.CurrentAction = actor.Squad.GetNewAction(actor, actor.Engine) ?? Idle.GetOrCreate();
            action.Dispose();
          }
          else
          {
            actor.CurrentAction = action.NextAction;
            action.Dispose();
          }
        }
        else
        {
          action.Process(actor.Engine, actor);
        }
      }
    }
  }
}
