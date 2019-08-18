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

    //public static void Init()
    //{
      //AddLogger(INITERROR, "initerror.log");
      //AddLogger(ERROR, "error.log");
      //AddLogger(DEBUG, "debug.log");
    //}

    private static IEnumerable<string> GetFilename(string baseFilename)
    {
      Directory.CreateDirectory(Globals.LogPath);

      for (int i = 0; ; i++)
        yield return Path.Combine(Globals.LogPath, i > 0 ? "{0}.{1}".F(baseFilename, i) : baseFilename);
    }

    public static TextWriter AddLogger(string channel)
    {
      return AddLogger(channel, "{0}.{1}".F(channel, ext));
    }

    public static TextWriter AddLogger(string channel, string baseFilename)
    {
      TextWriter info;
      if (Loggers.TryGetValue(channel, out info))
        return info;

      foreach (string filename in GetFilename(baseFilename))
        try
        {
          StreamWriter writer = File.CreateText(filename);
          writer.AutoFlush = true;
          info = TextWriter.Synchronized(writer);
          Loggers.Add(channel, info);
          return info;
        }
        catch (IOException) { } // if error, continue and attempt generating the next filename

      return info;
    }

    static TextWriter GetWriter(string channel)
    {
      TextWriter info;
      lock (Loggers)
        if (!Loggers.TryGetValue(channel, out info))
        {
          //throw new ArgumentException("Tried logging to non-existent channel " + channel, "channelName");
          info = AddLogger(channel);
        }

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

    internal static void Write(string channel, LogType type, params object[] args)
    {
      string s = string.Empty;
      if (LogDecorator.Decorator.TryGetValue(type, out s))
      {
        GetWriter(channel)?.Write("{0:s}\t{1:x}\t".F(DateTime.Now, type));
        GetWriter(channel)?.WriteLine(s, args);
      }
      else
      {
        GetWriter(channel)?.WriteLine("{0:s}\t{1:x}\t".F(DateTime.Now, type));
      }
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
