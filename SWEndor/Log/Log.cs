using SWEndor.Primitives.Extensions;
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
          info = AddLogger(channel);

      return info;
    }

    public static void Write(string channel, string value)
    {
      GetWriter(channel)?.WriteLine(value);
    }

    public static void Write(string channel, string formatvalue, params object[] args) { GetWriter(channel)?.WriteLine(formatvalue, args); }

    internal static void Write<T>(string channel, LogType type, T arg)
    {
      TextWriter tw = GetWriter(channel);
      if (tw == null)
        return;

      InnerPreWrite(tw, type);
      string s = string.Empty;
      if (LogDecorator.Decorator.TryGetValue(type, out s))
        tw.Write(s, arg);
      tw.WriteLine();
    }

    internal static void Write<T1, T2>(string channel, LogType type, T1 a1, T2 a2)
    {
      TextWriter tw = GetWriter(channel);
      if (tw == null)
        return;

      InnerPreWrite(tw, type);
      string s = string.Empty;
      if (LogDecorator.Decorator.TryGetValue(type, out s))
        tw.Write(s, a1, a2);
      tw.WriteLine();
    }

    internal static void Write<T1, T2, T3>(string channel, LogType type, T1 a1, T2 a2, T3 a3)
    {
      TextWriter tw = GetWriter(channel);
      if (tw == null)
        return;

      InnerPreWrite(tw, type);
      string s = string.Empty;
      if (LogDecorator.Decorator.TryGetValue(type, out s))
        tw.Write(s, a1, a2, a3);
      tw.WriteLine();
    }

    internal static void Write(string channel, LogType type, params object[] args)
    {
      TextWriter tw = GetWriter(channel);
      if (tw == null)
        return;

      InnerPreWrite(tw, type);
      string s = string.Empty;
      if (LogDecorator.Decorator.TryGetValue(type, out s))
        tw.Write(s, args);
      tw.WriteLine();
    }

    private static void InnerPreWrite(TextWriter tw, LogType type)
    {
      tw.Write(DateTime.Now.ToString("s"));
      tw.Write("\t");
      tw.Write(((int)type).ToString("x"));
      tw.Write("\t");
    }

    public static void WriteErr(string channel, Exception ex)
    {
      TextWriter tw = GetWriter(channel);

      if (tw == null)
        return;

      tw.Write("Fatal Error occured at ");
      tw.WriteLine(DateTime.Now.ToString("s"));
      tw.WriteLine("----------------------------------------------------------------");
      tw.Write("Message: ");
      tw.WriteLine(ex.Message);
      tw.WriteLine();
      tw.WriteLine(ex.StackTrace);
      tw.WriteLine();
      tw.WriteLine();
    }
  }
}
