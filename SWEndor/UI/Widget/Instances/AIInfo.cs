using MTV3D65;
using SWEndor.Actors;
using SWEndor.AI.Actions;

namespace SWEndor.UI.Widgets
{
  public class AIInfo : Widget
  {
    public AIInfo(Screen2D owner) : base(owner, "aiinfo") { }

    public override bool Visible
    {
      get
      {
        ActorInfo p = PlayerInfo.Actor;
        return (!Owner.ShowPage
          && p != null
          && !p.IsDyingOrDead
          && Owner.ShowUI
          && PlayerInfo.PlayerAIEnabled);
      }
    }

    public override void Draw()
    {
      ActorInfo p = PlayerInfo.Actor;
      if (p == null || !p.Active)
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

      TVScreen2DImmediate.Action_Begin2D();

      if (p.CurrentAction != null)
      {
        TV_3DVECTOR pos = p.GetRelativePositionXYZ(0, 0, p.TypeInfo.MeshData.MaxDimensions.z); //p.GetGlobalPosition(); // p.GetRelativePositionXYZ(0, 0, p.TypeInfo.max_dimensions.z + p.ProspectiveCollisionScanDistance);
        TV_3DVECTOR targetpos = p.AIData.GetTargetPos(p);
        TVScreen2DImmediate.Draw_Box3D(targetpos - new TV_3DVECTOR(25, 25, 25), targetpos + new TV_3DVECTOR(25, 25, 25), new TV_COLOR(1, 0.5f, 0.2f, 1).GetIntColor());
        TVScreen2DImmediate.Draw_Line3D(pos.x, pos.y, pos.z, targetpos.x, targetpos.y, targetpos.z, new TV_COLOR(1, 0, 0, 1).GetIntColor());
      }

      TVScreen2DImmediate.Draw_FilledBox(loc.x - 5, loc.y - 5, loc.x + 405, loc.y + 40 / 3 * lines + 5, new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
      TVScreen2DImmediate.Draw_FilledBox(Engine.ScreenWidth / 2 - 80, Engine.ScreenHeight / 2 - 25, Engine.ScreenWidth / 2 - 40, Engine.ScreenHeight / 2 + 5, new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
      TVScreen2DImmediate.Draw_FilledBox(Engine.ScreenWidth / 2 + 45, Engine.ScreenHeight / 2 - 25, Engine.ScreenWidth / 2 + 55 + 10 * name.Length, Engine.ScreenHeight / 2 + 5, new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
      TVScreen2DImmediate.Action_End2D();

      TVScreen2DText.Action_BeginText();
      TVScreen2DText.TextureFont_DrawText(actiontext
        , loc.x, loc.y, new TV_COLOR(0.6f, 0.8f, 0.6f, 1).GetIntColor(), FontFactory.Get(Font.T08).ID);

      TVScreen2DText.TextureFont_DrawText("AI"
        , Engine.ScreenWidth / 2 - 75, Engine.ScreenHeight / 2 - 20, new TV_COLOR(1, 0.5f, 0.2f, 1).GetIntColor(), FontFactory.Get(Font.T14).ID);

      TVScreen2DText.TextureFont_DrawText(name
        , Engine.ScreenWidth / 2 + 50, Engine.ScreenHeight / 2 - 20, new TV_COLOR(1, 0.5f, 0.2f, 1).GetIntColor(), FontFactory.Get(Font.T14).ID);
      TVScreen2DText.Action_EndText();

    }
  }
}
