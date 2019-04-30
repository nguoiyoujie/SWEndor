using SWEndor.AI.Actions;
using System.Text;

/*
namespace SWEndor.Actors
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


    string parents = "";
    foreach (int i in Actor.GetAllParents(1))
    {
      ActorInfo p = Engine.ActorFactory.Get(i);
      if (p != null)
        parents += (parents.Length == 0) ? p.ID.ToString() : ("," + p.ID);
    }
    builder.AppendLine(string.Format("Parent={0}", parents));

    string children = "";
    foreach (int i in Actor.GetAllChildren(1))
    {
      ActorInfo c = Engine.ActorFactory.Get(i);
      if (c != null)
        children += (children.Length == 0) ? c.ID.ToString() : ("," + c.ID);
    }
    builder.AppendLine(string.Format("Children={0}", children));

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
*/
