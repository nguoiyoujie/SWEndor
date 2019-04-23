using SWEndor.FileFormat.INI;
using System.Collections.Generic;

namespace SWEndor.Sound
{
  public partial class SoundManager
  {
    public partial class Piece
    {
      public static class Parser
      {
        public static Piece Parse(INIFile file, string sectionname)
        {
          Piece p = new Piece();
          p.SoundName = file.GetStringValue(sectionname, "Sound", "");
          p.CutIn = file.GetBoolValue(sectionname, "CutIn", false);
          p.EntryPosition = file.GetUIntValue(sectionname, "Entry", 0);
          p.ExitPositions = file.GetUIntList(sectionname, "Exit", new uint[0]);
          p.EndPosition = file.GetUIntValue(sectionname, "End", 0);

          int maxmood = 16; //placeholder
          p.MoodTransitions = new string[maxmood][];
          for (int i = 0; i < maxmood; i++)
          {
            string[] headers = file.GetStringList(sectionname, "Mood", new string[0]);
            //p.MoodTransitions[i] = new string[headers.Length];
            List<string> addlist = new List<string>();
            for (int j = 0; j < headers.Length; j++)
            {
              string snd = file.GetStringValue(headers[j], "Sound", null);
              if (snd != null && new List<string>(SoundManager.Instance().GetMusicNames()).Contains(snd))
              {
                addlist.Add(snd);
              }
            }
            p.MoodTransitions[i] = addlist.ToArray();
          }
          return p;
        }
      }
    }
  }
}
