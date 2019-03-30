using SWEndor.Actors;
using SWEndor.AI.Actions;

namespace SWEndor.AI
{
  public static class ActionManager
  {
    public static void Unlock(ActorInfo actor)
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

    public static void ClearQueue(ActorInfo actor)
    {
      if (actor.CurrentAction == null)
      {
      }
      else
      {
        ActionInfo aend = actor.CurrentAction;
        int limit = 99;
        while (aend.NextAction != null && limit > 0)
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

    public static void ForceClearQueue(ActorInfo actor)
    {
      actor.CurrentAction = null;
    }


    public static void QueueFirst(ActorInfo actor, ActionInfo action)
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

    public static void QueueNext(ActorInfo actor, ActionInfo action)
    {
      if (actor.CurrentAction == null)
        actor.CurrentAction = action;
      else
      {
        ActionInfo aend = actor.CurrentAction.NextAction;
        ActionInfo amid = action;
        actor.CurrentAction.NextAction = action;
        int limit = 99;
        while (amid.NextAction != null && limit > 0)
        {
          amid = amid.NextAction;
          limit--;
        }
        amid.NextAction = aend;
      }
    }

    public static void QueueLast(ActorInfo actor, ActionInfo action)
    {
      if (actor.CurrentAction == null)
        actor.CurrentAction = action;
      else
      {
        ActionInfo aend = actor.CurrentAction;
        int limit = 99;
        while (aend.NextAction != null && limit > 0)
        {
          aend = aend.NextAction;
          limit--;
        }
        aend.NextAction = action;
      }
    }

    public static void Run(ActorInfo actor, ActionInfo action)
    {
      if (action == null)
      {
        actor.CurrentAction = new Idle();
      }
      else
      {
        using (new PerfElement("action_" + action.Name))
        {
          if (action.Complete)
          {
            if (action.NextAction == null)
            {
              actor.CurrentAction = new Idle();
            }
            else
            {
              actor.CurrentAction = action.NextAction;
              //Run(actor, actor.CurrentAction); // ?
            }
          }
          else
          {
            action.Process(actor);
          }
        }
      }
    }
  }
}
