using Primrose.Primitives.Factories;
using Primitives.FileFormat.INI;
using System.Collections.Generic;

namespace SWEndor.Sound
{
  public partial class SoundManager
  {
    public partial class Piece
    {
      public static class Parser
      {
        public static void LoadFromINI(INIFile file, Piece p, string sectionname)
        {
          p.Name = sectionname;
          p.SoundName = file.GetString(sectionname, "Sound", "");
          p.EntryPosition = file.GetUInt(sectionname, "Entry", 0);
          p.ExitPositions = file.GetUIntArray(sectionname, "Exit", new uint[0]);
          p.EndPosition = file.GetUInt(sectionname, "End", 0);
          p.IntermissionTransitions = file.GetIntArray(sectionname, "Intermission", new int[0]);
          p.IsInterrupt = file.GetBool(sectionname, "IsInterrupt", p.IntermissionTransitions.Length > 0);

          //p.ChangeMood = file.GetInt(sectionname, "SwitchMood", -1);

          // AddMood=n,piece
          Registry<int, List<string>> temp = new Registry<int, List<string>>();
          p.MoodTransitions = new Registry<int, string[]>();
          p.ChangeMood = new Registry<int, int>();
          foreach (INIFile.INISection.INILine line in file.GetSection(sectionname).Lines)
          {
            if (line.Key == "AddMood")
            {
              string[] v = line.Value.Split(INIFile.DefaultDelimiter);
              if (v.Length > 1)
              {
                int m = 0;
                if (int.TryParse(v[0], out m))
                {
                  if (!p.MoodTransitions.Contains(m))
                    temp.Add(m, new List<string>());

                  for (int i = 1; i < v.Length; i++)
                    temp[m].Add(v[i]);
                }
              }
            }
            else if (line.Key == "ChangeMood")
            {
              string[] v = line.Value.Split(INIFile.DefaultDelimiter);
              if (v.Length == 2)
              {
                int m = 0;
                int m2 = 0;
                if (int.TryParse(v[0], out m) && int.TryParse(v[1], out m2))
                  p.ChangeMood.Put(m, m2);
              }
            }
          }
          foreach (int i in temp.GetKeys())
            if (temp[i].Count > 0)
              p.MoodTransitions[i] = temp[i].ToArray();
        }
      }
    }
  }
}
