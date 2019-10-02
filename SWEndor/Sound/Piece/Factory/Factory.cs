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
        private static Dictionary<string, string> soundnamelist = new Dictionary<string, string>();
        private static Dictionary<int, List<Piece>> tran_list = new Dictionary<int, List<Piece>>();

        public static int Count
        {
          get { return list.Count; }
        }

        private static void Register(Piece atype)
        {
          if (!list.ContainsKey(atype.SoundName))
          {
            if (!soundnamelist.ContainsKey(atype.Name))
              soundnamelist.Add(atype.Name, atype.SoundName);

            list.Add(atype.SoundName, atype);

            foreach (int t in atype.IntermissionTransitions)
            {
              if (!tran_list.ContainsKey(t))
                tran_list.Add(t, new List<Piece>());

              tran_list[t].Add(atype);
            }
          }
        }

        public static Piece Get(string name)
        {
          if (list.ContainsKey(name))
            return list[name];
          else if (soundnamelist.ContainsKey(name))
            return list[soundnamelist[name]];
          else
            return null;
        }

        public static Piece[] GetPieces(int transition)
        {
          if (!tran_list.ContainsKey(transition))
            return new Piece[0];

          return tran_list[transition].ToArray();
        }

        /*
        public static void Remove(string name)
        {
          if (list.ContainsKey(name))
            list.Remove(name);
        }
        */

        public static void LoadFromINI(string filepath)
        {
          if (File.Exists(filepath))
          {
            INIFile f = new INIFile(filepath);
            foreach (string s in f.Sections) // Parallel leads to bug in Register/list.Add
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
