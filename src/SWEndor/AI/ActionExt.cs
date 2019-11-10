using SWEndor.Actors;
using SWEndor.AI.Actions;

namespace SWEndor.AI
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
      if (actor.CurrentAction != null)
      {
        ActionInfo aend = actor.CurrentAction;
        int limit = 99;
        while (aend?.NextAction != null && limit > 0)
        {
          if (aend.CanInterrupt)
          {
            aend.Complete = true;
            aend = aend.NextAction;
            limit--;
          }
          else
          {
            limit = 0;
          }
        }
      }
    }

    /// <summary>Clears the actor's action queue regardless of whether the action is uninterruptible</summary>
    public static void ForceClearQueue(this ActorInfo actor)
    {
      ActionInfo action = actor.CurrentAction;
      while (action != null)
      {
        action.Dispose();
        action = action.NextAction;
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
        int limit = 99;
        while (amid.NextAction != null && limit > 0)
        {
          amid = amid.NextAction;
          limit--;
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
        int limit = 99;
        while (amid?.NextAction != null && limit > 0)
        {
          amid = amid.NextAction;
          limit--;
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
        int limit = 99;
        while (aend?.NextAction != null && limit > 0)
        {
          aend = aend.NextAction;
          limit--;
        }
        aend.NextAction = action;
      }
    }

    /// <summary>Processes the current action, moving to the next if the current action is complete</summary>
    public static void Run(this ActorInfo actor, ActionInfo action)
    {
      if (action == null)
      {
        actor.CurrentAction = actor.Squad.GetNewAction(actor.Engine) ?? new Idle();
      }
      else
      {
        if (action.Complete)
        {
          if (action.NextAction == null)
          {
            actor.CurrentAction = actor.Squad.GetNewAction(actor.Engine) ?? new Idle();
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
