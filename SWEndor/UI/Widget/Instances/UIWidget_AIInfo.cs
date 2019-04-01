using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.AI.Actions;

namespace SWEndor.UI
{
  public class UIWidget_AIInfo : UIWidget
  {
    public UIWidget_AIInfo() : base("aiinfo") { }

    public override bool Visible
    {
      get
      {
        return (!Screen2D.Instance().ShowPage
            && PlayerInfo.Instance().Actor != null
            && PlayerInfo.Instance().Actor.ActorState != ActorState.DEAD
            && PlayerInfo.Instance().Actor.ActorState != ActorState.DYING
            && !(PlayerInfo.Instance().Actor.TypeInfo is InvisibleCameraATI)
            && Screen2D.Instance().ShowUI
            && PlayerInfo.Instance().PlayerAIEnabled);
      }
    }

    public override void Draw()
    {
      ActorInfo p = PlayerInfo.Instance().Actor;
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

      Engine.Instance().TVScreen2DImmediate.Action_Begin2D();

      if (p.CurrentAction != null)
      {
        TV_3DVECTOR pos = p.GetPosition(); // p.GetRelativePositionXYZ(0, 0, p.TypeInfo.max_dimensions.z + p.ProspectiveCollisionScanDistance);

        TV_3DVECTOR targetpos = new TV_3DVECTOR();
        if (p.CurrentAction is AttackActor)
        {
          targetpos = ((AttackActor)p.CurrentAction).Target_Position;
          TV_3DVECTOR targetactpos = ((AttackActor)p.CurrentAction).Target_Actor.GetPosition();
          Engine.Instance().TVScreen2DImmediate.Draw_Line3D(pos.x, pos.y, pos.z, targetpos.x, targetpos.y, targetpos.z, new TV_COLOR(1, 0, 0, 1).GetIntColor());
          Engine.Instance().TVScreen2DImmediate.Draw_Box3D(targetpos - new TV_3DVECTOR(25, 25, 25), targetpos + new TV_3DVECTOR(25, 25, 25), new TV_COLOR(1, 0.5f, 0.2f, 1).GetIntColor());
          Engine.Instance().TVScreen2DImmediate.Draw_Box3D(targetactpos - new TV_3DVECTOR(50, 50, 50), targetactpos + new TV_3DVECTOR(50, 50, 50), new TV_COLOR(1, 0.5f, 0.2f, 1).GetIntColor());
        }
        else if (p.CurrentAction is Move)
        {
          targetpos = ((Move)p.CurrentAction).Target_Position;
          Engine.Instance().TVScreen2DImmediate.Draw_Line3D(pos.x, pos.y, pos.z, targetpos.x, targetpos.y, targetpos.z, new TV_COLOR(0.5f, 1, 0.5f, 1).GetIntColor());
        }
        else if (p.CurrentAction is ForcedMove)
        {
          targetpos = ((ForcedMove)p.CurrentAction).Target_Position;
          Engine.Instance().TVScreen2DImmediate.Draw_Line3D(pos.x, pos.y, pos.z, targetpos.x, targetpos.y, targetpos.z, new TV_COLOR(1, 1, 0.5f, 1).GetIntColor());
        }
        else if (p.CurrentAction is Rotate)
        {
          targetpos = ((Rotate)p.CurrentAction).Target_Position;
          Engine.Instance().TVScreen2DImmediate.Draw_Line3D(pos.x, pos.y, pos.z, targetpos.x, targetpos.y, targetpos.z, new TV_COLOR(0.5f, 1, 1, 1).GetIntColor());
        }
        else if (p.CurrentAction is FollowActor)
        {
          targetpos = ((FollowActor)p.CurrentAction).Target_Actor.GetPosition();
          Engine.Instance().TVScreen2DImmediate.Draw_Line3D(pos.x, pos.y, pos.z, targetpos.x, targetpos.y, targetpos.z, new TV_COLOR(0.5f, 0.5f, 1, 1).GetIntColor());
        }
        else if (p.CurrentAction is Evade)
        {
          targetpos = ((Evade)p.CurrentAction).Target_Position;
          Engine.Instance().TVScreen2DImmediate.Draw_Line3D(pos.x, pos.y, pos.z, targetpos.x, targetpos.y, targetpos.z, new TV_COLOR(1, 0.3f, 0, 1).GetIntColor());
        }
        else if (p.CurrentAction is AvoidCollisionRotate)
        {
          targetpos = ((AvoidCollisionRotate)p.CurrentAction).Target_Position;
          Engine.Instance().TVScreen2DImmediate.Draw_Line3D(pos.x, pos.y, pos.z, targetpos.x, targetpos.y, targetpos.z, new TV_COLOR(1, 0.2f, 0.6f, 1).GetIntColor());
          targetpos = p.ProspectiveCollisionImpact + p.ProspectiveCollisionNormal * 250;
          Engine.Instance().TVScreen2DImmediate.Draw_Line3D(p.ProspectiveCollisionImpact.x, p.ProspectiveCollisionImpact.y, p.ProspectiveCollisionImpact.z, targetpos.x, targetpos.y, targetpos.z, new TV_COLOR(1, 1, 1, 1).GetIntColor());
        }

        /*
        TV_3DVECTOR prostart = p.GetRelativePositionXYZ(0, 0, p.TypeInfo.max_dimensions.z + 10);
        TV_3DVECTOR proend0 = p.GetRelativePositionXYZ(0, 0, p.TypeInfo.max_dimensions.z + 10 + p.ProspectiveCollisionScanDistance);

        Engine.Instance().TVScreen2DImmediate.Draw_Line3D(prostart.x
                                                        , prostart.y
                                                        , prostart.z
                                                        , proend0.x
                                                        , proend0.y
                                                        , proend0.z
                                                        , new TV_COLOR(1, 0.5f, 0.2f, 1).GetIntColor()
                                                        );
        */
      }

      Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(loc.x - 5, loc.y - 5, loc.x + 405, loc.y + 40 / 3 * lines + 5, new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
      Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(Engine.Instance().ScreenWidth / 2 - 80, Engine.Instance().ScreenHeight / 2 - 25, Engine.Instance().ScreenWidth / 2 - 40, Engine.Instance().ScreenHeight / 2 + 5, new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
      Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(Engine.Instance().ScreenWidth / 2 + 45, Engine.Instance().ScreenHeight / 2 - 25, Engine.Instance().ScreenWidth / 2 + 55 + 10 * name.Length, Engine.Instance().ScreenHeight / 2 + 5, new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
      Engine.Instance().TVScreen2DImmediate.Action_End2D();

      Engine.Instance().TVScreen2DText.Action_BeginText();
      Engine.Instance().TVScreen2DText.TextureFont_DrawText(actiontext
        , loc.x, loc.y, new TV_COLOR(0.6f, 0.8f, 0.6f, 1).GetIntColor(), Font.GetFont("Text_08").ID);

      Engine.Instance().TVScreen2DText.TextureFont_DrawText("AI"
        , Engine.Instance().ScreenWidth / 2 - 75, Engine.Instance().ScreenHeight / 2 - 20, new TV_COLOR(1, 0.5f, 0.2f, 1).GetIntColor(), Font.GetFont("Text_14").ID);

      Engine.Instance().TVScreen2DText.TextureFont_DrawText(name
        , Engine.Instance().ScreenWidth / 2 + 50, Engine.Instance().ScreenHeight / 2 - 20, new TV_COLOR(1, 0.5f, 0.2f, 1).GetIntColor(), Font.GetFont("Text_14").ID);
      Engine.Instance().TVScreen2DText.Action_EndText();

    }
  }
}
