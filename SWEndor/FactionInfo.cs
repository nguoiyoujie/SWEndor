using MTV3D65;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWEndor
{
  public class FactionInfo
  {
    private static Dictionary<string, FactionInfo> list = new Dictionary<string, FactionInfo>();
    public static Dictionary<string, FactionInfo> GetList() { return list; }
    public static FactionInfo Get(string key) { return list.ContainsKey(key) ? list[key] : null; }
    public static FactionInfo Neutral = new FactionInfo("Neutral", new TV_COLOR(1, 1, 1, 1));

    private FactionInfo(string name, TV_COLOR color)
    {
      Name = name;
      Color = color;
    }

    public static void Reset()
    {
      list = new Dictionary<string, FactionInfo>();
    }

    public static FactionInfo AddFaction(string name, TV_COLOR color)
    {
      if (!list.ContainsKey(name))
      {
        list.Add(name, new FactionInfo(name, color));
      }
      return list[name];
    }

    public bool IsAlliedWith(FactionInfo faction)
    {
      return (this == faction || Allies.Contains(faction));
    }

    public string Name { get; private set; }
    public TV_COLOR Color = new TV_COLOR(1,1,1,1);
    public bool AutoAI = false;

    public List<FactionInfo> Allies = new List<FactionInfo>();
  }
}
