﻿using Primrose.Primitives.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace SWEndor
{
  public static class Logger
  {
    public const string INITERROR = "initerror";
    public const string ERROR = "error";
    public const string DEBUG = "debug";

    private const string ext = "log";
    private static readonly Dictionary<string, TextWriter> Loggers = new Dictionary<string, TextWriter>();
    private static ConcurrentQueue<LogItem> Queue = new ConcurrentQueue<LogItem>();

    private static IEnumerable<string> GetFilename(string baseFilename)
    {
      Directory.CreateDirectory(Globals.LogPath);

      for (int i = 0; ; i++)
        yield return Path.Combine(Globals.LogPath, i > 0 ? "{0}.{1}".F(baseFilename, i) : baseFilename);
    }

    public static TextWriter Add(string channel)
    {
      return Add(channel, "{0}.{1}".F(channel, ext));
    }

    public static TextWriter Add(string channel, string baseFilename)
    {
      TextWriter info;
      if (Loggers.TryGetValue(channel, out info))
        return info;

      foreach (string filename in GetFilename(baseFilename))
      {
        try
        {
          StreamWriter writer = File.CreateText(filename);
          writer.AutoFlush = true;
          info = TextWriter.Synchronized(writer);
          Loggers.Add(channel, info);
          return info;
        }
        catch (IOException) { } // if error, continue and attempt generating the next filename
      }

      return info;
    }

    static TextWriter GetWriter(string channel)
    {
      TextWriter info;
      lock (Loggers)
        if (!Loggers.TryGetValue(channel, out info))
          info = Add(channel);

      return info;
    }

    internal static void Log<T>(string channel, LogType type, T arg)
    {
      string s = string.Empty;
      if (LogDecorator.Decorator.TryGetValue(type, out s))
        Queue.Enqueue(new LogItem(channel, type, s.F(arg)));
    }

    internal static void Log<T1, T2>(string channel, LogType type, T1 a1, T2 a2)
    {
      string s = string.Empty;
      if (LogDecorator.Decorator.TryGetValue(type, out s))
        Queue.Enqueue(new LogItem(channel, type, s.F(a1, a2)));
    }

    internal static void Log<T1, T2, T3>(string channel, LogType type, T1 a1, T2 a2, T3 a3)
    {
      string s = string.Empty;
      if (LogDecorator.Decorator.TryGetValue(type, out s))
        Queue.Enqueue(new LogItem(channel, type, s.F(a1, a2, a3)));
    }

    internal static void Log(string channel, LogType type, params object[] args)
    {
      string s = string.Empty;
      if (LogDecorator.Decorator.TryGetValue(type, out s))
        Queue.Enqueue(new LogItem(channel, type, s.F(args)));
    }

    internal static void DoWrite()
    {
      LogItem item;
      while (Queue.TryDequeue(out item))
      {
        TextWriter tw = GetWriter(item.Channel);
        if (tw != null)
        {
          DoWrite(tw, item.Type, item.Message);
        }
      }
    }

    private static void DoWrite(TextWriter tw, LogType type, string message)
    {
      tw.Write(DateTime.Now.ToString("s"));
      tw.Write("\t");
      tw.Write("{0,-30}".F(type));
      tw.Write("\t");
      tw.Write(message);
      tw.WriteLine();
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
