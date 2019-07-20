using System;
using SWEndor.Actors;
using SWEndor.AI.Actions;
using SWEndor.Primitives.Traits;

namespace SWEndor.AI
{
  public interface IActionable
  {
    ActionInfo CurrentAction { get; }

    void ReleaseLock<A>(A self) where A : ITraitOwner;
    void Lock<A>(A self) where A : ITraitOwner;
    void Clear<A>(A self) where A : ITraitOwner;
    void ForceClear<A>(A self) where A : ITraitOwner;
    void QueueFirst<A>(A self, ActionInfo action) where A : ITraitOwner;
    void QueueNext<A>(A self, ActionInfo action) where A : ITraitOwner;
    void QueueLast<A>(A self, ActionInfo action) where A : ITraitOwner;
    void Run<A>(A self) where A : ITraitOwner;
  }

  public class Actionable : IActionable
  {
    public ActionInfo CurrentAction { get; private set; }

    public void Clear<A>(A self) where A : ITraitOwner
    {
      if (CurrentAction == null)
        return;

      ActionInfo aend = CurrentAction;
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

    public void ForceClear<A>(A self) where A : ITraitOwner
    {
      CurrentAction = null;
    }

    public void Lock<A>(A self) where A : ITraitOwner
    {
      QueueFirst(self, new Lock());
    }

    public void QueueLast<A>(A self, ActionInfo action) where A : ITraitOwner
    {
      // currentAction, nextAction, ..., [NEW ACTION]
      if (CurrentAction == null)
        CurrentAction = action;
      else
      {
        ActionInfo aend = CurrentAction;
        int limit = 99;
        while (aend.NextAction != null && limit > 0)
        {
          aend = aend.NextAction;
          limit--;
        }
        aend.NextAction = action;
      }
    }

    public void QueueNext<A>(A self, ActionInfo action) where A : ITraitOwner
    {
      // currentAction, [NEW ACTION], nextAction, ...
      if (CurrentAction == null)
        CurrentAction = action;
      else
      {
        ActionInfo aend = CurrentAction.NextAction;
        ActionInfo amid = action;
        CurrentAction.NextAction = action;
        int limit = 99;
        while (amid.NextAction != null && limit > 0)
        {
          amid = amid.NextAction;
          limit--;
        }
        amid.NextAction = aend;
      }
    }

    public void QueueFirst<A>(A self, ActionInfo action) where A : ITraitOwner
    {
      // [NEW ACTION], currentAction, nextAction, ...
      if (CurrentAction == null)
        CurrentAction = action;
      else
      {
        ActionInfo aend = CurrentAction;
        ActionInfo amid = action;
        CurrentAction = action;
        int limit = 99;
        while (amid.NextAction != null && limit > 0)
        {
          amid = amid.NextAction;
          limit--;
        }
        amid.NextAction = aend;
      }
    }

    public void ReleaseLock<A>(A self) where A : ITraitOwner
    {
      ActionInfo action = CurrentAction;
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

    public void Run<A>(A self) where A : ITraitOwner
    {
      if (CurrentAction == null)
      {
        CurrentAction = new Idle();
      }
      else
      {
        if (CurrentAction.Complete)
        {
          if (CurrentAction.NextAction == null)
          {
            CurrentAction = new Idle();
          }
          else
          {
            ActionInfo action = CurrentAction;
            CurrentAction = action.NextAction;
            action.Dispose();
          }
        }
        else
        {
          //CurrentAction.Process(actor.Engine, actor.ID);
        }
      }
    }
  }


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

    public static void ForceClearQueue(this ActorInfo actor)
    {
      if (actor != null)
      {
        actor.CurrentAction = null;
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
        while (amid.NextAction != null && limit > 0)
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
        while (aend.NextAction != null && limit > 0)
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
        actor.CurrentAction = new Idle();
      }
      else
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
