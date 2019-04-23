using SWEndor.FileFormat.INI;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.Sound
{
  public partial class SoundManager
  {
    public partial class Piece
    {
      public class Factory
      {
        private static Dictionary<string, Piece> list = new Dictionary<string, Piece>();

        public static void Register(Piece atype)
        {
          if (list.ContainsKey(atype.SoundName))
          {
            atype = list[atype.SoundName];
          }
          else
          {
            list.Add(atype.SoundName, atype);
          }
        }

        public static Piece Get(string name)
        {
          if (list.ContainsKey(name))
            return list[name];
          else
            return null;
        }

        public static void Remove(string name)
        {
          if (list.ContainsKey(name))
            list.Remove(name);
        }

        public static void LoadFromINI(string filepath)
        {
          if (File.Exists(filepath))
          {
            INIFile f = new INIFile(filepath);
            foreach (string s in f.Sections.Keys)
            {
              if (s != INIFile.PreHeaderSectionName)
              {
                Piece p = Parser.Parse(f, s);
                Register(p);
                p.UpdateSound();
              }
            }
          }
        }
      }
    }
  }
}
