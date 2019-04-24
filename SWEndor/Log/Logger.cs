using System;
using System.IO;

namespace SWEndor.Log
{
  public class Logger // placeholder to hold errlog, an actual log class will be established here
  {
    public static void GenerateErrLog(Exception ex, string errorfilename)
    {
      string errlogpath = Path.Combine(Globals.LogPath, errorfilename);

      using (StreamWriter sw = new StreamWriter(errlogpath, false))
      {
        sw.WriteLine(string.Format("Fatal Error occured at {0:s}", DateTime.Now.ToString()));
        sw.WriteLine("----------------------------------------------------------------");
        sw.WriteLine(string.Format("Message: {0}", ex.Message));
        sw.WriteLine();
        sw.WriteLine(string.Format("{0}", ex.StackTrace));
        sw.WriteLine();
        sw.WriteLine();
      }
    }
  }
}
