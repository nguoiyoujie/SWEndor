using SWEndor.Actors;
using SWEndor.AI.Actions;

namespace SWEndor.AI
{
  public class ActionManager
  {
    public readonly Engine Engine;
    internal ActionManager(Engine engine)
    { Engine = engine; }

    public void UnlockOne(int actorID)
    {
      ActorInfo actor = Engine.ActorFactory.Get(actorID);
      if (actor != null)
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
    }

    public void ClearQueue(int actorID)
    {
      ActorInfo actor = Engine.ActorFactory.Get(actorID);
      if (actor != null)
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
    }

    public void ForceClearQueue(int actorID)
    {
      ActorInfo actor = Engine.ActorFactory.Get(actorID);
      if (actor != null)
      {
        actor.CurrentAction = null;
      }
    }

    public void QueueFirst(int actorID, ActionInfo action)
    {
      ActorInfo actor = Engine.ActorFactory.Get(actorID);
      if (actor != null)
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
    }

    public void QueueNext(int actorID, ActionInfo action)
    {
      ActorInfo actor = Engine.ActorFactory.Get(actorID);
      if (actor != null)
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
    }

    public void QueueLast(int actorID, ActionInfo action)
    {
      ActorInfo actor = Globals.Engine.ActorFactory.Get(actorID);
      if (actor != null)
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
    }

    public void Run(int actorID, ActionInfo action)
    {
      ActorInfo actor = Engine.ActorFactory.Get(actorID);
      if (actor != null)
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
                action.Dispose();
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
}
