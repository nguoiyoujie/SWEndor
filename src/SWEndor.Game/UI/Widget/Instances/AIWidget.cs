using MTV3D65;
using SWEndor.Game.Actors;
using SWEndor.Game.AI.Actions;
using System.Collections.Generic;

namespace SWEndor.Game.UI.Widgets
{
  public class AIWidget : Widget
  {
    private readonly List<string> m_actiontext = new List<string>();

    public AIWidget(Screen2D owner) : base(owner, "aiinfo") { }

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
      m_actiontext.Clear();
      while (action != null)
      {
        m_actiontext.Add(action.ToString());
        action = action.NextAction;
      }

      int lines = m_actiontext.Count;

      TVScreen2DImmediate.Action_Begin2D();

      if (p.CurrentAction != null)
      {
        TV_3DVECTOR pos = p.GetRelativePositionXYZ(0, 0, p.MaxDimensions.z); //p.GetGlobalPosition(); // p.GetRelativePositionXYZ(0, 0, p.TypeInfo.max_dimensions.z + p.ProspectiveCollisionScanDistance);
        TV_3DVECTOR targetpos = p.AI.Target.Position;
        TVScreen2DImmediate.Draw_Box3D(targetpos - new TV_3DVECTOR(25, 25, 25), targetpos + new TV_3DVECTOR(25, 25, 25), new TV_COLOR(1, 0.5f, 0.2f, 1).GetIntColor());
        TVScreen2DImmediate.Draw_Line3D(pos.x, pos.y, pos.z, targetpos.x, targetpos.y, targetpos.z, new TV_COLOR(1, 0, 0, 1).GetIntColor());
      }

      TVScreen2DImmediate.Draw_FilledBox(loc.x - 5, loc.y - 5, loc.x + 405, loc.y + 40 / 3 * lines + 5, new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
      TVScreen2DImmediate.Draw_FilledBox(Owner.ScreenCenter.x - 80, Owner.ScreenCenter.y - 25, Owner.ScreenCenter.x - 40, Owner.ScreenCenter.y + 5, new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
      TVScreen2DImmediate.Draw_FilledBox(Owner.ScreenCenter.x + 45, Owner.ScreenCenter.y - 25, Owner.ScreenCenter.x + 55 + 10 * name.Length, Owner.ScreenCenter.y + 5, new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
      TVScreen2DImmediate.Action_End2D();

      TVScreen2DText.Action_BeginText();
      TVScreen2DText.TextureFont_DrawText(string.Join("\n", m_actiontext)
        , loc.x, loc.y, new TV_COLOR(0.6f, 0.8f, 0.6f, 1).GetIntColor(), FontFactory.Get(Font.T08).ID);

      TVScreen2DText.TextureFont_DrawText("AI"
        , Owner.ScreenCenter.x - 75, Owner.ScreenCenter.y - 20, new TV_COLOR(1, 0.5f, 0.2f, 1).GetIntColor(), FontFactory.Get(Font.T14).ID);

      TVScreen2DText.TextureFont_DrawText(name
        , Owner.ScreenCenter.x + 50, Owner.ScreenCenter.y - 20, new TV_COLOR(1, 0.5f, 0.2f, 1).GetIntColor(), FontFactory.Get(Font.T14).ID);
      TVScreen2DText.Action_EndText();

    }
  }
}
