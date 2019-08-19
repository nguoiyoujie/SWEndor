using MTV3D65;
using SWEndor.Actors;
using System;
using System.Collections.Generic;

namespace SWEndor.UI.Widgets
{
  public class Debug_ObjectInfo : Widget
  {
    public Debug_ObjectInfo(Screen2D owner) : base(owner, "debug_objectinfo") { }

    public override bool Visible
    {
      get
      {
        return false; // Settings.GameDebug;
      }
    }

    public override void Draw()
    {
      TVScreen2DText.Action_BeginText();

      TV_2DVECTOR loc = new TV_2DVECTOR(30, 375);
      string swingcount = "";
      Dictionary<string, int> wingcount = new Dictionary<string, int>();

      Action<Engine, ActorInfo> action = new Action<Engine, ActorInfo>(
        (_, a) =>
        {
          if (a != null && a.CreationState == CreationState.ACTIVE)
          {
            if (!wingcount.ContainsKey("All Objects"))
              wingcount.Add("All Objects", 1);
            else
              wingcount["All Objects"]++;
          }
          if (a != null && a.TypeInfo is ActorTypes.Groups.Projectile && a.CreationState == CreationState.ACTIVE && a.Faction != null)
          {
            if (!wingcount.ContainsKey("Projectiles"))
              wingcount.Add("Projectiles", 1);
            else
              wingcount["Projectiles"]++;
          }
          if (a != null && (a.TypeInfo is ActorTypes.Groups.Fighter) && a.CreationState == CreationState.ACTIVE && a.Faction != null)
          {
            if (!wingcount.ContainsKey(a.Faction.Name + " Wings"))
              wingcount.Add(a.Faction.Name + " Wings", 1);
            else
              wingcount[a.Faction.Name + " Wings"]++;
          }
        });

      ActorFactory.DoEach(action);

      foreach (KeyValuePair<string, int> kvp in wingcount)
      {
        swingcount += kvp.Key + ": " + kvp.Value + "\n";
      }
      TVScreen2DText.TextureFont_DrawText(swingcount
      , loc.x, loc.y, new TV_COLOR(0.6f, 0.8f, 0.6f, 1).GetIntColor());

      TVScreen2DText.Action_EndText();
    }
  }
}
