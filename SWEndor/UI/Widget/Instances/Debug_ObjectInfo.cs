﻿using MTV3D65;
using SWEndor.Actors;
using System.Collections.Generic;

namespace SWEndor.UI.Widgets
{
  public class Debug_ObjectInfo : Widget
  {
    public Debug_ObjectInfo() : base("debug_objectinfo") { }

    public override bool Visible
    {
      get
      {
        return false; // Settings.GameDebug;
      }
    }

    public override void Draw()
    {
      Globals.Engine.TVScreen2DText.Action_BeginText();

      TV_2DVECTOR loc = new TV_2DVECTOR(30, 375);
      string swingcount = "";
      Dictionary<string, int> wingcount = new Dictionary<string, int>();
      foreach (int actorID in ActorInfo.Factory.GetHoldingList())
      {
        ActorInfo a = ActorInfo.Factory.Get(actorID);
        if (a != null && a.CreationState == CreationState.ACTIVE)
        {
          if (!wingcount.ContainsKey("All Objects"))
            wingcount.Add("All Objects", 1);
          else
            wingcount["All Objects"]++;
        }
        if (a != null && a.TypeInfo is ActorTypes.Group.Projectile && a.CreationState == CreationState.ACTIVE && a.Faction != null)
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
      }
      foreach (KeyValuePair<string, int> kvp in wingcount)
      {
        swingcount += kvp.Key + ": " + kvp.Value + "\n";
      }
      Globals.Engine.TVScreen2DText.TextureFont_DrawText(swingcount
      , loc.x, loc.y, new TV_COLOR(0.6f, 0.8f, 0.6f, 1).GetIntColor());

      Globals.Engine.TVScreen2DText.Action_EndText();
    }
  }
}
