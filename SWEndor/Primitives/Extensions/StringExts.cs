using System;
using System.Collections.Generic;
using System.Text;

namespace SWEndor.Primitives.Extensions
{
  public static class StringExts
  {
    public static string[] NewLines = new string[] { "\r\n", "\r", "\n" };
    public static char[] Spaces = new char[] { ' ' };

    public static string Multiline(this string input, int maxLineLength)
    {
      string[] lines = input.Split(NewLines, StringSplitOptions.None);
      for (int i = 0; i < lines.Length; i++)
        lines[i] = string.Join("\n", lines[i].SplitToLines(maxLineLength));

      return string.Join("\n", lines);
    }

    public static IEnumerable<string> SplitToLines(this string stringToSplit, int maxLineLength)
    {
      string[] words = stringToSplit.Split(Spaces);
      StringBuilder line = new StringBuilder();
      foreach (string word in words)
      {
        if (word.Length + line.Length <= maxLineLength)
        {
          line.Append(word);
          line.Append(" ");
        }
        else
        {
          if (line.Length > 0)
          {
            yield return line.ToString().Trim();
            line.Clear();
          }
          string overflow = word;
          while (overflow.Length > maxLineLength)
          {
            yield return overflow.Substring(0, maxLineLength);
            overflow = overflow.Substring(maxLineLength);
          }
          line.Append(overflow);
          line.Append(" ");
        }
      }
      yield return line.ToString().Trim();
    }

    public static string[] Split(this string s, char c)
    {
      return new List<string>(s.InnerSplit(c)).ToArray();
    }

    public static IEnumerable<string> InnerSplit(this string s, char c)
    {
      int index = 0;
      for (int i = 0; i < s.Length; i++)
      {
        if (s[i] == c)
        {
          yield return s.Substring(index, i - index);
          index = i + 1;
        }
      }

      yield return s.Substring(index, s.Length - index);
    }

    public static string F<T1>(this string fmt, T1 o1) { return string.Format(fmt, o1); }
    public static string F<T1, T2>(this string fmt, T1 o1, T2 o2) { return string.Format(fmt, o1, o2); }
    public static string F<T1, T2, T3>(this string fmt, T1 o1, T2 o2, T3 o3) { return string.Format(fmt, o1, o2, o3); }

    public static string Scramble(this string str)
    {
      StringBuilder jumble = new StringBuilder(str);
      int length = jumble.Length;
      for (int i = length - 1; i > 0; i--)
      {
        int j = Globals.Engine.Random.Next(i);
        char temp = jumble[j];
        jumble[j] = jumble[i];
        jumble[i] = temp;
      }
      return jumble.ToString();
    }


    // avoiding params
    public static string C(this string s1, string s2) { return string.Concat(s1, s2); }
    public static string C(this string s1, string s2, string s3) { return string.Concat(s1, s2, s3); }
    public static string C(this string s1, string s2, string s3, string s4) { return string.Concat(s1, s2, s3, s4); }
    public static string C(this string s1, string s2, string s3, string s4, string s5) { return s1.C(s2, s3, s4).C(s5); }
    public static string C(this string s1, string s2, string s3, string s4, string s5, string s6) { return s1.C(s2, s3, s4).C(s5, s6); }
    public static string C(this string s1, string s2, string s3, string s4, string s5, string s6, string s7) { return s1.C(s2, s3, s4).C(s5, s6, s7); }
    public static string C(this string s1, string s2, string s3, string s4, string s5, string s6, string s7, string s8) { return s1.C(s2, s3, s4).C(s5, s6, s7).C(s8); }
    public static string C(this string s1, string s2, string s3, string s4, string s5, string s6, string s7, string s8, string s9) { return s1.C(s2, s3, s4).C(s5, s6, s7).C(s8, s9); }
    public static string C(this string s1, string s2, string s3, string s4, string s5, string s6, string s7, string s8, string s9, string s10) { return s1.C(s2, s3, s4).C(s5, s6, s7).C(s8, s9, s10); }

    public static string F(this string fmt, object o) { return string.Format(fmt, o); }
    public static string F(this string fmt, object o1, object o2) { return string.Format(fmt, o1, o2); }
    public static string F(this string fmt, object o1, object o2, object o3) { return string.Format(fmt, o1, o2, o3); }
    public static string F(this string fmt, params object[] args) { return string.Format(fmt, args); }
  }
}
