using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Types;
using System.Collections.Generic;

namespace SWEndor.UI
{
  public class UIWidget_Debug_ObjectInfo : UIWidget
  {
    public UIWidget_Debug_ObjectInfo() : base("debug_objectinfo") { }

    public override bool Visible
    {
      get
      {
        return false; // Settings.GameDebug;
      }
    }

    public override void Draw()
    {
      Engine.Instance().TVScreen2DText.Action_BeginText();

      TV_2DVECTOR loc = new TV_2DVECTOR(30, 375);
      string swingcount = "";
      Dictionary<string, int> wingcount = new Dictionary<string, int>();
      foreach (ActorInfo a in ActorFactory.Instance().GetHoldingList())
      {
        if (a != null && a.CreationState == CreationState.ACTIVE)
        {
          if (!wingcount.ContainsKey("All Objects"))
            wingcount.Add("All Objects", 1);
          else
            wingcount["All Objects"]++;
        }
        if (a != null && a.TypeInfo is ProjectileGroup && a.CreationState == CreationState.ACTIVE && a.Faction != null)
        {
          if (!wingcount.ContainsKey("Projectiles"))
            wingcount.Add("Projectiles", 1);
          else
            wingcount["Projectiles"]++;
        }
        if (a != null && (a.TypeInfo is FighterGroup || a.TypeInfo is TIEGroup) && a.CreationState == CreationState.ACTIVE && a.Faction != null)
        {
          if (!wingcount.ContainsKey(a.Faction.Name + " Wings"))
            wingcount.Add(a.Faction.Name + " Wings", 1);
          else
            wingcount[a.Faction.Name + " Wings"]++;
        }
      }
      foreach (KeyValuePair<string, int> kvp in wingcount)
      {
        swingcount += kvp.Key + ": " + kvp.Value + "\n";
      }
      Engine.Instance().TVScreen2DText.TextureFont_DrawText(swingcount
      , loc.x, loc.y, new TV_COLOR(0.6f, 0.8f, 0.6f, 1).GetIntColor());

      Engine.Instance().TVScreen2DText.Action_EndText();
    }
  }
}
