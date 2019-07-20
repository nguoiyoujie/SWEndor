using SWEndor.Primitives;
using System;
using System.Collections.Generic;
using System.IO;

namespace SWEndor
{
  public static class Log
  {
    public const string INITERROR = "initerror";
    public const string ERROR = "error";
    public const string DEBUG = "debug";

    private const string ext = "log";
    private static readonly Dictionary<string, TextWriter> Loggers = new Dictionary<string, TextWriter>();

    public static void Init()
    {
      AddLogger(INITERROR, "initerror.log");
      AddLogger(ERROR, "error.log");
      AddLogger(DEBUG, "debug.log");
    }

    private static IEnumerable<string> GetFilename(string baseFilename)
    {
      Directory.CreateDirectory(Globals.LogPath);

      for (int i = 0; ; i++)
        yield return Path.Combine(Globals.LogPath, i > 0 ? "{0}.{1}".F(baseFilename, i) : baseFilename);
    }

    public static void AddLogger(string channel)
    {
      AddLogger(channel, "{0}.{1}".F(channel, ext));
    }

    public static void AddLogger(string channel, string baseFilename)
    {
      if (Loggers.ContainsKey(channel))
        return;

      foreach (string filename in GetFilename(baseFilename))
        try
        {
          StreamWriter writer = File.CreateText(filename);
          writer.AutoFlush = true;
          Loggers.Add(channel, TextWriter.Synchronized(writer));
          return;
        }
        catch (IOException) { } // if error, continue and attempt generating the next filename
    }

    static TextWriter GetWriter(string channel)
    {
      TextWriter info;
      lock (Loggers)
        if (!Loggers.TryGetValue(channel, out info))
          throw new ArgumentException("Tried logging to non-existent channel " + channel, "channelName");

      return info;
    }

    public static void Write(string channel, string value)
    {
      GetWriter(channel)?.WriteLine(value);
    }

    public static void Write(string channel, string formatvalue, params object[] args)
    {
      GetWriter(channel)?.WriteLine(formatvalue, args);
    }

    public static void WriteErr(string channel, Exception ex)
    {
      TextWriter tw = GetWriter(channel);

      if (tw == null)
        return;

      tw.WriteLine("Fatal Error occured at {0:s}".F(DateTime.Now.ToString()));
      tw.WriteLine("----------------------------------------------------------------");
      tw.WriteLine("Message: {0}", ex.Message);
      tw.WriteLine();
      tw.WriteLine(ex.StackTrace);
      tw.WriteLine();
      tw.WriteLine();
    }
  }
}
