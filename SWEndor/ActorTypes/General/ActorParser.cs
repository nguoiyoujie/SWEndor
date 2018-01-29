using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace SWEndor
{
  public class ActorParser
  {
    public ActorParser(ActorInfo actor)
    {
      m_actor = actor;
      Scheme = "";
    }

    private ActorInfo m_actor = null;
    public ActorInfo Actor { get { return m_actor; } }
    public string Scheme { get; private set; }

    public void Generate(StringBuilder builder)
    {


      builder.AppendLine(string.Format("[Actor:{0}:{1}]", Actor.TypeInfo.Name, Actor.ID));

      //
      builder.AppendLine(string.Format("IsCombatObject={0}", Actor.IsCombatObject.ToString()));        //bool IsCombatObject
      builder.AppendLine(string.Format("OnTimedLife={0}", Actor.OnTimedLife.ToString()));              //bool OnTimedLife
      builder.AppendLine(string.Format("TimedLife={0}", Actor.TimedLife.ToString()));                  //float TimedLife
      builder.AppendLine(string.Format("Strength={0}", Actor.Strength.ToString()));                    //float Strength
      builder.AppendLine(string.Format("Speed={0}", Actor.Speed.ToString()));                          //float Speed
      builder.AppendLine(string.Format("XTurnAngle={0}", Actor.XTurnAngle.ToString()));                //float XTurnAngle
      builder.AppendLine(string.Format("YTurnAngle={0}", Actor.YTurnAngle.ToString()));                //float YTurnAngle
      builder.AppendLine(string.Format("ApplyZBalance={0}", Actor.ApplyZBalance.ToString()));          //bool ApplyZBalance
      //builder.AppendLine(string.Format("Weight={0}", Actor.Weight.ToString()));                        //float Weight
      builder.AppendLine(string.Format("EnteredCombatZone={0}", Actor.EnteredCombatZone.ToString()));  //bool EnteredCombatZone

      builder.AppendLine(string.Format("CreationState={0}", Actor.CreationState.ToString()));                                        //ActorState State 
      builder.AppendLine(string.Format("ActorState={0}", Actor.ActorState.ToString()));                                        //ActorState State 
      builder.AppendLine(string.Format("prevActorState={0}", Actor.prevActorState.ToString()));                                //ActorState prevState
      builder.AppendLine(string.Format("Scale={0}", Utilities.ToString(Actor.Scale)));                               //TV_3DVECTOR Scale
      builder.AppendLine(string.Format("prevScale={0}", Utilities.ToString(Actor.prevScale)));                       //TV_3DVECTOR prevScale
      builder.AppendLine(string.Format("Position={0}", Utilities.ToString(Actor.Position)));                         //TV_3DVECTOR Position
      builder.AppendLine(string.Format("PrevPosition={0}", Utilities.ToString(Actor.PrevPosition)));                 //TV_3DVECTOR PrevPosition
      builder.AppendLine(string.Format("Rotation={0}", Utilities.ToString(Actor.Rotation)));                  // TV_3DVECTOR Rotation
      builder.AppendLine(string.Format("PrevRotation={0}", Utilities.ToString(Actor.PrevRotation)));                  // TV_3DVECTOR PrevRotation

      string parents = "";
      foreach (ActorInfo p in Actor.GetAllParents(1))
      {
        parents += (parents.Length == 0) ? p.ID.ToString() : ("," + p.ID);
      }
      builder.AppendLine(string.Format("Parent={0}", parents));

      string children = "";
      foreach (ActorInfo c in Actor.GetAllChildren(1))
      {
        children += (children.Length == 0) ? c.ID.ToString() : ("," + c.ID);
      }
      builder.AppendLine(string.Format("Children={0}", children));


      foreach (string key in Actor.GetStateBKeys())
      {
        builder.AppendLine(string.Format("StateB.{0}={1}", key, Actor.GetStateB(key)));
      }
      foreach (string key in Actor.GetStateFKeys())
      {
        builder.AppendLine(string.Format("StateF.{0}={1}", key, Actor.GetStateF(key)));
      }
      foreach (string key in Actor.GetCustomStateSKeys())
      {
        builder.AppendLine(string.Format("StateS.{0}={1}", key, Actor.GetStateS(key)));
      }

      //Event
      foreach (string ae in Actor.TickEvents)
      {
        builder.AppendLine(string.Format("TickEvents={0}", ae));
      }
      foreach (string ae in Actor.CreatedEvents)
      {
        builder.AppendLine(string.Format("CreatedEvents={0}", ae));
      }
      foreach (string ae in Actor.DestroyedEvents)
      {
        builder.AppendLine(string.Format("DestroyedEvents={0}", ae));
      }
      foreach (string ae in Actor.HitEvents)
      {
        builder.AppendLine(string.Format("HitEvents={0}", ae));
      }
      foreach (string ae in Actor.ActorStateChangeEvents)
      {
        builder.AppendLine(string.Format("ActorStateChangeEvents={0}", ae));
      }

      //Actions / AI
      ActionInfo action = Actor.CurrentAction;
      while (action != null)
      {
        builder.AppendLine(string.Format("Action={0}", action.ToString()));
        action = action.NextAction;
      }


      //Complete
      builder.AppendLine(string.Format(""));
    }
  }
}
