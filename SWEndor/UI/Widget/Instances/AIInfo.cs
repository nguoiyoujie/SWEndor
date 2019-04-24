using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.AI.Actions;
using SWEndor.Player;

namespace SWEndor.UI.Widgets
{
  public class AIInfo : Widget
  {
    public AIInfo() : base("aiinfo") { }

    public override bool Visible
    {
      get
      {
        return (!Globals.Engine.Screen2D.ShowPage
            && Globals.Engine.PlayerInfo.Actor != null
            && Globals.Engine.PlayerInfo.Actor.ActorState != ActorState.DEAD
            && Globals.Engine.PlayerInfo.Actor.ActorState != ActorState.DYING
            && !(Globals.Engine.PlayerInfo.Actor.TypeInfo is InvisibleCameraATI)
            && Globals.Engine.Screen2D.ShowUI
            && Globals.Engine.PlayerInfo.PlayerAIEnabled);
      }
    }

    public override void Draw()
    {
      ActorInfo p = Globals.Engine.PlayerInfo.Actor;
      if (p == null || p.CreationState != CreationState.ACTIVE)
        return;

      TV_2DVECTOR loc = new TV_2DVECTOR(10, 175);

      ActionInfo action = p.CurrentAction;
      string name = (action != null) ? action.Name.ToUpper() : "NULL";
      string actiontext = "";
      while (action != null)
      {
        actiontext += "\n" + action.ToString();
        action = action.NextAction;
      }

      int lines = actiontext.Split('\n').Length;

      Globals.Engine.TVScreen2DImmediate.Action_Begin2D();

      if (p.CurrentAction != null)
      {
        TV_3DVECTOR pos = p.GetPosition(); // p.GetRelativePositionXYZ(0, 0, p.TypeInfo.max_dimensions.z + p.ProspectiveCollisionScanDistance);

        TV_3DVECTOR targetpos = new TV_3DVECTOR();
        if (p.CurrentAction is AttackActor)
        {
          ActorInfo target = ActorInfo.Factory.Get(((AttackActor)p.CurrentAction).Target_ActorID);
          if (target != null)
          {
            targetpos = ((AttackActor)p.CurrentAction).Target_Position;
            TV_3DVECTOR targetactpos = target.GetPosition();
            Globals.Engine.TVScreen2DImmediate.Draw_Line3D(pos.x, pos.y, pos.z, targetpos.x, targetpos.y, targetpos.z, new TV_COLOR(1, 0, 0, 1).GetIntColor());
            Globals.Engine.TVScreen2DImmediate.Draw_Box3D(targetpos - new TV_3DVECTOR(25, 25, 25), targetpos + new TV_3DVECTOR(25, 25, 25), new TV_COLOR(1, 0.5f, 0.2f, 1).GetIntColor());
            Globals.Engine.TVScreen2DImmediate.Draw_Box3D(targetactpos - new TV_3DVECTOR(50, 50, 50), targetactpos + new TV_3DVECTOR(50, 50, 50), new TV_COLOR(1, 0.5f, 0.2f, 1).GetIntColor());
          }
        }
        else if (p.CurrentAction is Move)
        {
          targetpos = ((Move)p.CurrentAction).Target_Position;
          Globals.Engine.TVScreen2DImmediate.Draw_Line3D(pos.x, pos.y, pos.z, targetpos.x, targetpos.y, targetpos.z, new TV_COLOR(0.5f, 1, 0.5f, 1).GetIntColor());
        }
        else if (p.CurrentAction is ForcedMove)
        {
          targetpos = ((ForcedMove)p.CurrentAction).Target_Position;
          Globals.Engine.TVScreen2DImmediate.Draw_Line3D(pos.x, pos.y, pos.z, targetpos.x, targetpos.y, targetpos.z, new TV_COLOR(1, 1, 0.5f, 1).GetIntColor());
        }
        else if (p.CurrentAction is Rotate)
        {
          targetpos = ((Rotate)p.CurrentAction).Target_Position;
          Globals.Engine.TVScreen2DImmediate.Draw_Line3D(pos.x, pos.y, pos.z, targetpos.x, targetpos.y, targetpos.z, new TV_COLOR(0.5f, 1, 1, 1).GetIntColor());
        }
        else if (p.CurrentAction is FollowActor)
        {
          ActorInfo target = ActorInfo.Factory.Get(((AttackActor)p.CurrentAction).Target_ActorID);
          if (target != null)
          {
            targetpos = target.GetPosition();
            Globals.Engine.TVScreen2DImmediate.Draw_Line3D(pos.x, pos.y, pos.z, targetpos.x, targetpos.y, targetpos.z, new TV_COLOR(0.5f, 0.5f, 1, 1).GetIntColor());
          }
        }
        else if (p.CurrentAction is Evade)
        {
          targetpos = ((Evade)p.CurrentAction).Target_Position;
          Globals.Engine.TVScreen2DImmediate.Draw_Line3D(pos.x, pos.y, pos.z, targetpos.x, targetpos.y, targetpos.z, new TV_COLOR(1, 0.3f, 0, 1).GetIntColor());
        }
        else if (p.CurrentAction is AvoidCollisionRotate)
        {
          targetpos = ((AvoidCollisionRotate)p.CurrentAction).Target_Position;
          Globals.Engine.TVScreen2DImmediate.Draw_Line3D(pos.x, pos.y, pos.z, targetpos.x, targetpos.y, targetpos.z, new TV_COLOR(1, 0.2f, 0.6f, 1).GetIntColor());
          targetpos = p.CollisionInfo.ProspectiveCollisionImpact + p.CollisionInfo.ProspectiveCollisionNormal * 250;
          Globals.Engine.TVScreen2DImmediate.Draw_Line3D(p.CollisionInfo.ProspectiveCollisionImpact.x, p.CollisionInfo.ProspectiveCollisionImpact.y, p.CollisionInfo.ProspectiveCollisionImpact.z, targetpos.x, targetpos.y, targetpos.z, new TV_COLOR(1, 1, 1, 1).GetIntColor());
        }

        /*
        TV_3DVECTOR prostart = p.GetRelativePositionXYZ(0, 0, p.TypeInfo.max_dimensions.z + 10);
        TV_3DVECTOR proend0 = p.GetRelativePositionXYZ(0, 0, p.TypeInfo.max_dimensions.z + 10 + p.ProspectiveCollisionScanDistance);

        Globals.Engine.TVScreen2DImmediate.Draw_Line3D(prostart.x
                                                        , prostart.y
                                                        , prostart.z
                                                        , proend0.x
                                                        , proend0.y
                                                        , proend0.z
                                                        , new TV_COLOR(1, 0.5f, 0.2f, 1).GetIntColor()
                                                        );
        */
      }

      Globals.Engine.TVScreen2DImmediate.Draw_FilledBox(loc.x - 5, loc.y - 5, loc.x + 405, loc.y + 40 / 3 * lines + 5, new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
      Globals.Engine.TVScreen2DImmediate.Draw_FilledBox(Globals.Engine.ScreenWidth / 2 - 80, Globals.Engine.ScreenHeight / 2 - 25, Globals.Engine.ScreenWidth / 2 - 40, Globals.Engine.ScreenHeight / 2 + 5, new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
      Globals.Engine.TVScreen2DImmediate.Draw_FilledBox(Globals.Engine.ScreenWidth / 2 + 45, Globals.Engine.ScreenHeight / 2 - 25, Globals.Engine.ScreenWidth / 2 + 55 + 10 * name.Length, Globals.Engine.ScreenHeight / 2 + 5, new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
      Globals.Engine.TVScreen2DImmediate.Action_End2D();

      Globals.Engine.TVScreen2DText.Action_BeginText();
      Globals.Engine.TVScreen2DText.TextureFont_DrawText(actiontext
        , loc.x, loc.y, new TV_COLOR(0.6f, 0.8f, 0.6f, 1).GetIntColor(), Font.Factory.Get("Text_08").ID);

      Globals.Engine.TVScreen2DText.TextureFont_DrawText("AI"
        , Globals.Engine.ScreenWidth / 2 - 75, Globals.Engine.ScreenHeight / 2 - 20, new TV_COLOR(1, 0.5f, 0.2f, 1).GetIntColor(), Font.Factory.Get("Text_14").ID);

      Globals.Engine.TVScreen2DText.TextureFont_DrawText(name
        , Globals.Engine.ScreenWidth / 2 + 50, Globals.Engine.ScreenHeight / 2 - 20, new TV_COLOR(1, 0.5f, 0.2f, 1).GetIntColor(), Font.Factory.Get("Text_14").ID);
      Globals.Engine.TVScreen2DText.Action_EndText();

    }
  }
}
