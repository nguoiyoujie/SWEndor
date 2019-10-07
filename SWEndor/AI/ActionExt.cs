using SWEndor.Actors;
using SWEndor.AI.Actions;

namespace SWEndor.AI
{
  public static class ActionExt
  {
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

    public static void ForceClearQueue(this ActorInfo actor)
    {
      ActionInfo action = actor.CurrentAction;
      while (action != null)
      {
        action.Complete = true;
        action = action.NextAction;
      }
    }

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
