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
          p.Name = sectionname;
          p.SoundName = file.GetStringValue(sectionname, "Sound", "");
          p.EntryPosition = file.GetUIntValue(sectionname, "Entry", 0);
          p.ExitPositions = file.GetUIntList(sectionname, "Exit", new uint[0]);
          p.EndPosition = file.GetUIntValue(sectionname, "End", 0);
          p.IntermissionTransitions = file.GetIntList(sectionname, "Intermission", new int[0]);

          int maxmood = 16; // TO-DO: formalize maximum mood support
          p.MoodTransitions = new string[maxmood][];
          List<string>[] mt = new List<string>[maxmood];
          for (int i = 0; i < maxmood; i++)
            mt[i] = new List<string>();

          // AddMood=n,piece
          foreach (INIFile.INISection.INILine line in file.GetSection(sectionname).Lines)
          {
            if (line.Key == "AddMood")
            {
              string[] v = line.Value.Split(INIFile.DefaultDelimiter);
              if (v.Length > 1)
              {
                int m = 0;
                if (int.TryParse(v[0], out m) && m >= 0 && m < maxmood)
                {
                  for (int i = 1; i < v.Length; i++)
                    mt[m].Add(v[i]);
                }
              }
            }
          }
          for (int i = 0; i < maxmood; i++)
            p.MoodTransitions[i] = mt[i].ToArray();

          return p;
        }
      }
    }
  }
}
