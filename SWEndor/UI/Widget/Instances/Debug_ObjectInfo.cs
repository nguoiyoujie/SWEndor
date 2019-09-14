using MTV3D65;
using SWEndor.Actors;
using System;
using System.Collections.Generic;
using System.Text;

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

    StringBuilder sb = new StringBuilder();
    public override void Draw()
    {
      TVScreen2DText.Action_BeginText();

      TV_2DVECTOR loc = new TV_2DVECTOR(30, 375);
      sb.Clear();
      Dictionary<string, int> wingcount = new Dictionary<string, int>();

      Action<Engine, ActorInfo, Dictionary<string, int>> action = new Action<Engine, ActorInfo, Dictionary<string, int>>(
        (_, a, wc) =>
        {
          if (a != null)
          {
            if (a.Active)
            {
              if (!wc.ContainsKey("All Objects"))
                wc.Add("All Objects", 1);
              else
                wc["All Objects"]++;
            }
            if (a.TypeInfo is ActorTypes.Groups.Projectile && a.Active && a.Faction != null)
            {
              if (!wc.ContainsKey("Projectiles"))
                wc.Add("Projectiles", 1);
              else
                wc["Projectiles"]++;
            }
            if ((a.TypeInfo is ActorTypes.Groups.Fighter) && a.Active && a.Faction != null)
            {
              string w = a.Faction.Name + " Wings";
              if (!wc.ContainsKey(w))
                wc.Add(w, 1);
              else
                wc[w]++;
            }
          }
        });

      ActorFactory.DoEach(action, wingcount);

      foreach (KeyValuePair<string, int> kvp in wingcount)
      {
        sb.Append(kvp.Key);
        sb.Append(": ");
        sb.Append(kvp.Value);
        sb.AppendLine();
      }
      TVScreen2DText.TextureFont_DrawText(sb.ToString()
      , loc.x, loc.y, new TV_COLOR(0.6f, 0.8f, 0.6f, 1).GetIntColor());

      TVScreen2DText.Action_EndText();
    }
  }
}
