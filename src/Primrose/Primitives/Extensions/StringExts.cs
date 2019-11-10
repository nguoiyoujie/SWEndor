using System;
using System.Collections.Generic;
using System.Text;

namespace Primrose.Primitives.Extensions
{
  /// <summary>
  /// Provides extension methods for string values
  /// </summary>
  public static class StringExts
  {
    // cached
    private static string[] NewLines = new string[] { "\r\n", "\r", "\n" };
    private static char[] Spaces = new char[] { ' ' };

    /// <summary>
    /// Provides a multiline representation of a string
    /// </summary>
    /// <param name="input">The input string</param>
    /// <param name="maxLineLength">The maximum length of each line</param>
    /// <returns>A multiline representation of a string. Each new line is preceded by a newline "\n" character</returns>
    public static string Multiline(this string input, int maxLineLength)
    {
      string[] lines = input.Split(NewLines, StringSplitOptions.None);
      for (int i = 0; i < lines.Length; i++)
        lines[i] = string.Join("\n", lines[i].SplitToLines(maxLineLength));

      return string.Join("\n", lines);
    }

    private static IEnumerable<string> SplitToLines(this string stringToSplit, int maxLineLength)
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

    /// <summary>
    /// Scrambles a string by rearranging its characters in a random order.
    /// </summary>
    /// <param name="str">The input string</param>
    /// <param name="rand">The random object</param>
    /// <returns>A string with its characters scrambled</returns>
    /// <exception cref="ArgumentNullException"><paramref name="rand"/> cannot be null</exception>
    public static string Scramble(this string str, Random rand)
    {
      if (rand == null) throw new ArgumentNullException("rand");

      StringBuilder jumble = new StringBuilder(str);
      int length = jumble.Length;
      for (int i = length - 1; i > 0; i--)
      {
        int j = rand.Next(i);
        char temp = jumble[j];
        jumble[j] = jumble[i];
        jumble[i] = temp;
      }
      return jumble.ToString();
    }

    /// <summary>Replaces one or more format items in a specified string with the string representation of a specified object.</summary>
    /// <typeparam name="T1"></typeparam>
    /// <param name="fmt">A composite format string.</param>
    /// <param name="o1">The object to format.</param>
    /// <returns>A copy of format in which any format items are replaced by the string representation of the respective arguments</returns>
    /// <exception cref="ArgumentNullException">format is null.</exception>
    /// <exception cref="FormatException">The format item in format is invalid.-or- The index of a format item is not zero.</exception>
    public static string F<T1>(this string fmt, T1 o1) { return string.Format(fmt, o1); }

    /// <summary>Replaces one or more format items in a specified string with the string representation of a specified object.</summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="fmt">A composite format string.</param>
    /// <param name="o1">The object to format.</param>
    /// <param name="o2">The object to format.</param>
    /// <returns>A copy of format in which any format items are replaced by the string representation of the respective arguments</returns>
    /// <exception cref="ArgumentNullException">format is null.</exception>
    /// <exception cref="FormatException">The format item in format is invalid.-or- The index of a format item is not zero.</exception>
    public static string F<T1, T2>(this string fmt, T1 o1, T2 o2) { return string.Format(fmt, o1, o2); }

    /// <summary>Replaces one or more format items in a specified string with the string representation of a specified object.</summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <param name="fmt">A composite format string.</param>
    /// <param name="o1">The object to format.</param>
    /// <param name="o2">The object to format.</param>
    /// <param name="o3">The object to format.</param>
    /// <returns>A copy of format in which any format items are replaced by the string representation of the respective arguments</returns>
    /// <exception cref="ArgumentNullException">format is null.</exception>
    /// <exception cref="FormatException">The format item in format is invalid.-or- The index of a format item is not zero.</exception>
    public static string F<T1, T2, T3>(this string fmt, T1 o1, T2 o2, T3 o3) { return string.Format(fmt, o1, o2, o3); }

    /// <summary>Replaces one or more format items in a specified string with the string representation of a specified object.</summary>
    /// <param name="fmt">A composite format string.</param>
    /// <param name="o1">The object to format.</param>
    /// <returns>A copy of format in which any format items are replaced by the string representation of the respective arguments</returns>
    /// <exception cref="ArgumentNullException">format is null.</exception>
    /// <exception cref="FormatException">The format item in format is invalid.-or- The index of a format item is not zero.</exception>
    public static string F(this string fmt, object o1) { return string.Format(fmt, o1); }

    /// <summary>Replaces one or more format items in a specified string with the string representation of a specified object.</summary>
    /// <param name="fmt">A composite format string.</param>
    /// <param name="o1">The object to format.</param>
    /// <param name="o2">The object to format.</param>
    /// <returns>A copy of format in which any format items are replaced by the string representation of the respective arguments</returns>
    /// <exception cref="ArgumentNullException">format is null.</exception>
    /// <exception cref="FormatException">The format item in format is invalid.-or- The index of a format item is not zero.</exception>
    public static string F(this string fmt, object o1, object o2) { return string.Format(fmt, o1, o2); }

    /// <summary>Replaces one or more format items in a specified string with the string representation of a specified object.</summary>
    /// <param name="fmt">A composite format string.</param>
    /// <param name="o1">The object to format.</param>
    /// <param name="o2">The object to format.</param>
    /// <param name="o3">The object to format.</param>
    /// <returns>A copy of format in which any format items are replaced by the string representation of the respective arguments</returns>
    /// <exception cref="ArgumentNullException">format is null.</exception>
    /// <exception cref="FormatException">The format item in format is invalid.-or- The index of a format item is not zero.</exception>
    public static string F(this string fmt, object o1, object o2, object o3) { return string.Format(fmt, o1, o2, o3); }

    /// <summary>Replaces one or more format items in a specified string with the string representation of a specified object.</summary>
    /// <param name="fmt">A composite format string.</param>
    /// <param name="args">The objects to format.</param>
    /// <returns>A copy of format in which any format items are replaced by the string representation of the respective arguments</returns>
    /// <exception cref="ArgumentNullException">format is null.</exception>
    /// <exception cref="FormatException">The format item in format is invalid.-or- The index of a format item is not zero.</exception>
    public static string F(this string fmt, params object[] args) { return string.Format(fmt, args); }

    /// <summary>Concatenates two specified instances of System.String.</summary>
    /// <param name="s1">The first string to concatenate.</param>
    /// <param name="s2">The second string to concatenate.</param>
    /// <returns>The concatenation of the specified strings.</returns>
    public static string C(this string s1, string s2) { return string.Concat(s1, s2); }

    /// <summary>Concatenates two specified instances of System.String.</summary>
    /// <param name="s1">The first string to concatenate.</param>
    /// <param name="s2">The second string to concatenate.</param>
    /// <param name="s3">The third string to concatenate.</param>
    /// <returns>The concatenation of the specified strings.</returns>
    public static string C(this string s1, string s2, string s3) { return string.Concat(s1, s2, s3); }

    /// <summary>Concatenates two specified instances of System.String.</summary>
    /// <param name="s1">The first string to concatenate.</param>
    /// <param name="s2">The second string to concatenate.</param>
    /// <param name="s3">The third string to concatenate.</param>
    /// <param name="s4">The fourth string to concatenate.</param>
    /// <returns>The concatenation of the specified strings.</returns>
    public static string C(this string s1, string s2, string s3, string s4) { return string.Concat(s1, s2, s3, s4); }

    /// <summary>Concatenates two specified instances of System.String.</summary>
    /// <param name="s1">The first string to concatenate.</param>
    /// <param name="s2">The second string to concatenate.</param>
    /// <param name="s3">The third string to concatenate.</param>
    /// <param name="s4">The fourth string to concatenate.</param>
    /// <param name="s5">The fifth string to concatenate.</param>
    /// <returns>The concatenation of the specified strings.</returns>
    public static string C(this string s1, string s2, string s3, string s4, string s5) { return s1.C(s2, s3, s4).C(s5); }

    /// <summary>Concatenates two specified instances of System.String.</summary>
    /// <param name="s1">The first string to concatenate.</param>
    /// <param name="s2">The second string to concatenate.</param>
    /// <param name="s3">The third string to concatenate.</param>
    /// <param name="s4">The fourth string to concatenate.</param>
    /// <param name="s5">The fifth string to concatenate.</param>
    /// <param name="s6">The sixth string to concatenate.</param>
    /// <returns>The concatenation of the specified strings.</returns>
    public static string C(this string s1, string s2, string s3, string s4, string s5, string s6) { return s1.C(s2, s3, s4).C(s5, s6); }

    /// <summary>Concatenates two specified instances of System.String.</summary>
    /// <param name="s1">The first string to concatenate.</param>
    /// <param name="s2">The second string to concatenate.</param>
    /// <param name="s3">The third string to concatenate.</param>
    /// <param name="s4">The fourth string to concatenate.</param>
    /// <param name="s5">The fifth string to concatenate.</param>
    /// <param name="s6">The sixth string to concatenate.</param>
    /// <param name="s7">The seventh string to concatenate.</param>
    /// <returns>The concatenation of the specified strings.</returns>
    public static string C(this string s1, string s2, string s3, string s4, string s5, string s6, string s7) { return s1.C(s2, s3, s4).C(s5, s6, s7); }

    /// <summary>Concatenates two specified instances of System.String.</summary>
    /// <param name="s1">The first string to concatenate.</param>
    /// <param name="s2">The second string to concatenate.</param>
    /// <param name="s3">The third string to concatenate.</param>
    /// <param name="s4">The fourth string to concatenate.</param>
    /// <param name="s5">The fifth string to concatenate.</param>
    /// <param name="s6">The sixth string to concatenate.</param>
    /// <param name="s7">The seventh string to concatenate.</param>
    /// <param name="s8">The eighth string to concatenate.</param>
    /// <returns>The concatenation of the specified strings.</returns>
    public static string C(this string s1, string s2, string s3, string s4, string s5, string s6, string s7, string s8) { return s1.C(s2, s3, s4).C(s5, s6, s7).C(s8); }

    /// <summary>Concatenates two specified instances of System.String.</summary>
    /// <param name="s1">The first string to concatenate.</param>
    /// <param name="s2">The second string to concatenate.</param>
    /// <param name="s3">The third string to concatenate.</param>
    /// <param name="s4">The fourth string to concatenate.</param>
    /// <param name="s5">The fifth string to concatenate.</param>
    /// <param name="s6">The sixth string to concatenate.</param>
    /// <param name="s7">The seventh string to concatenate.</param>
    /// <param name="s8">The eighth string to concatenate.</param>
    /// <param name="s9">The ninth string to concatenate.</param>
    /// <returns>The concatenation of the specified strings.</returns>
    public static string C(this string s1, string s2, string s3, string s4, string s5, string s6, string s7, string s8, string s9) { return s1.C(s2, s3, s4).C(s5, s6, s7).C(s8, s9); }

    /// <summary>Concatenates two specified instances of System.String.</summary>
    /// <param name="s1">The first string to concatenate.</param>
    /// <param name="s2">The second string to concatenate.</param>
    /// <param name="s3">The third string to concatenate.</param>
    /// <param name="s4">The fourth string to concatenate.</param>
    /// <param name="s5">The fifth string to concatenate.</param>
    /// <param name="s6">The sixth string to concatenate.</param>
    /// <param name="s7">The seventh string to concatenate.</param>
    /// <param name="s8">The eighth string to concatenate.</param>
    /// <param name="s9">The ninth string to concatenate.</param>
    /// <param name="s10">The tenth string to concatenate.</param>
    /// <returns>The concatenation of the specified strings.</returns>
    public static string C(this string s1, string s2, string s3, string s4, string s5, string s6, string s7, string s8, string s9, string s10) { return s1.C(s2, s3, s4).C(s5, s6, s7).C(s8, s9, s10); }
  }
}
