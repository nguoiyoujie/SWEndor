using MTV3D65;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWEndor
{
  public static class Utilities
  {
    public static string Multiline(string input, int maxLineLength)
    {
      string[] lines = input.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
      for (int i = 0; i < lines.Length; i++)
        lines[i] = string.Join("\n", SplitToLines(lines[i], maxLineLength));

      return string.Join("\n", lines);
    }

    public static IEnumerable<string> SplitToLines(string stringToSplit, int maxLineLength)
    {
      string[] words = stringToSplit.Split(' ');
      StringBuilder line = new StringBuilder();
      foreach (string word in words)
      {
        if (word.Length + line.Length <= maxLineLength)
        {
          line.Append(word + " ");
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
          line.Append(overflow + " ");
        }
      }
      yield return line.ToString().Trim();
    }

    public static TV_3DVECTOR GetRotation(TV_3DVECTOR direction)
    {
      float x = Globals.Engine.TrueVision.TVMathLibrary.Direction2Ang(-direction.y, Globals.Engine.TrueVision.TVMathLibrary.TVVec2Length(new TV_2DVECTOR(direction.z, direction.x)));
      float y = Globals.Engine.TrueVision.TVMathLibrary.Direction2Ang(direction.x, direction.z);

      return new TV_3DVECTOR(x, y, 0);
    }

    public static TV_3DVECTOR GetDirection(TV_3DVECTOR rotation)
    {
      float x = (float)(Math.Cos(rotation.x / 180 * Globals.PI) * Math.Sin(rotation.y / 180 * Globals.PI));
      float y = -(float)Math.Sin(rotation.x / 180 * Globals.PI);
      float z = (float)(Math.Cos(rotation.x / 180 * Globals.PI) * Math.Cos(rotation.y / 180 * Globals.PI));

      return new TV_3DVECTOR(x, y, z);
    }

    public static string ToString(TV_3DVECTOR vector)
    {
      return string.Format("(VEC:{0},{1},{2})", vector.x, vector.y, vector.z);
    }

    public static void Clamp(ref float value, float min, float max)
    {
      if (max == min)
      {
        value = min;
        return;
      }
      else if (max < min)
      {
        float temp = max;
        max = min;
        min = temp;
      }

      if (value > max)
        value = max;
      else if (value < min)
        value = min;
    }

    public static void Modulus(ref float value, float min, float max)
    {
      if (max == min)
      {
        value = min;
        return;
      }
      else if (max < min)
      {
        float temp = max;
        max = min;
        min = temp;
      }

      value %= max - min;

      if (value > max)
        value -= max - min;
      else if (value < min)
        value += max - min;
    }

    public static void Clamp(ref TV_3DVECTOR point, TV_3DVECTOR minBound, TV_3DVECTOR maxBound)
    {
      TV_3DVECTOR ret = point - minBound;
      TV_3DVECTOR vol = maxBound - minBound;
      if (ret.x < 0)
        ret.x = 0;
      else if (ret.x > vol.x)
        ret.x = vol.x;

      if (ret.y < 0)
        ret.y = 0;
      else if (ret.y > vol.y)
        ret.y = vol.y;

      if (ret.z < 0)
        ret.z = 0;
      else if (ret.z > vol.z)
        ret.z = vol.z;

      ret += minBound;
    }

    public static TV_3DVECTOR ModulusBox(TV_3DVECTOR point, TV_3DVECTOR minBound, TV_3DVECTOR maxBound)
    {
      TV_3DVECTOR ret = point - minBound;
      TV_3DVECTOR vol = maxBound - minBound;
      ret.x %= vol.x;
      if (ret.x < 0)
        ret.x += vol.x;

      ret.y %= vol.y;
      if (ret.y < 0)
        ret.y += vol.y;

      ret.z %= vol.z;
      if (ret.z < 0)
        ret.z += vol.z;

      ret += minBound;

      return ret;
    }

    public static char TVKeyToChar(bool shift, CONST_TV_KEY key)
    {
      switch (key)
      {
        case CONST_TV_KEY.TV_KEY_0:
        case CONST_TV_KEY.TV_KEY_NUMPAD0:
          return '0';
        case CONST_TV_KEY.TV_KEY_1:
        case CONST_TV_KEY.TV_KEY_NUMPAD1:
          return '1';
        case CONST_TV_KEY.TV_KEY_2:
        case CONST_TV_KEY.TV_KEY_NUMPAD2:
          return '2';
        case CONST_TV_KEY.TV_KEY_3:
        case CONST_TV_KEY.TV_KEY_NUMPAD3:
          return '3';
        case CONST_TV_KEY.TV_KEY_4:
        case CONST_TV_KEY.TV_KEY_NUMPAD4:
          return '4';
        case CONST_TV_KEY.TV_KEY_5:
        case CONST_TV_KEY.TV_KEY_NUMPAD5:
          return '5';
        case CONST_TV_KEY.TV_KEY_6:
        case CONST_TV_KEY.TV_KEY_NUMPAD6:
          return '6';
        case CONST_TV_KEY.TV_KEY_7:
        case CONST_TV_KEY.TV_KEY_NUMPAD7:
          return '7';
        case CONST_TV_KEY.TV_KEY_8:
        case CONST_TV_KEY.TV_KEY_NUMPAD8:
          return '8';
        case CONST_TV_KEY.TV_KEY_9:
        case CONST_TV_KEY.TV_KEY_NUMPAD9:
          return '9';

        case CONST_TV_KEY.TV_KEY_A:
          return shift ? 'A' : 'a';
        case CONST_TV_KEY.TV_KEY_B:
          return shift ? 'B' : 'b';
        case CONST_TV_KEY.TV_KEY_C:
          return shift ? 'C' : 'c';
        case CONST_TV_KEY.TV_KEY_D:
          return shift ? 'D' : 'd';
        case CONST_TV_KEY.TV_KEY_E:
          return shift ? 'E' : 'e';
        case CONST_TV_KEY.TV_KEY_F:
          return shift ? 'F' : 'f';
        case CONST_TV_KEY.TV_KEY_G:
          return shift ? 'G' : 'g';
        case CONST_TV_KEY.TV_KEY_H:
          return shift ? 'H' : 'h';
        case CONST_TV_KEY.TV_KEY_I:
          return shift ? 'I' : 'i';
        case CONST_TV_KEY.TV_KEY_J:
          return shift ? 'J' : 'j';
        case CONST_TV_KEY.TV_KEY_K:
          return shift ? 'K' : 'k';
        case CONST_TV_KEY.TV_KEY_L:
          return shift ? 'L' : 'l';
        case CONST_TV_KEY.TV_KEY_M:
          return shift ? 'M' : 'm';
        case CONST_TV_KEY.TV_KEY_N:
          return shift ? 'N' : 'n';
        case CONST_TV_KEY.TV_KEY_O:
          return shift ? 'O' : 'o';
        case CONST_TV_KEY.TV_KEY_P:
          return shift ? 'P' : 'p';
        case CONST_TV_KEY.TV_KEY_Q:
          return shift ? 'Q' : 'q';
        case CONST_TV_KEY.TV_KEY_R:
          return shift ? 'R' : 'r';
        case CONST_TV_KEY.TV_KEY_S:
          return shift ? 'S' : 's';
        case CONST_TV_KEY.TV_KEY_T:
          return shift ? 'T' : 't';
        case CONST_TV_KEY.TV_KEY_U:
          return shift ? 'U' : 'u';
        case CONST_TV_KEY.TV_KEY_V:
          return shift ? 'V' : 'v';
        case CONST_TV_KEY.TV_KEY_W:
          return shift ? 'W' : 'w';
        case CONST_TV_KEY.TV_KEY_X:
          return shift ? 'X' : 'x';
        case CONST_TV_KEY.TV_KEY_Y:
          return shift ? 'Y' : 'y';
        case CONST_TV_KEY.TV_KEY_Z:
          return shift ? 'Z' : 'z';

        case CONST_TV_KEY.TV_KEY_APOSTROPHE:
          return '\'';
        case CONST_TV_KEY.TV_KEY_ADD:
          return '+';
        case CONST_TV_KEY.TV_KEY_MINUS:
        case CONST_TV_KEY.TV_KEY_SUBTRACT:
          return '-';
        case CONST_TV_KEY.TV_KEY_MULTIPLY:
          return '*';
        case CONST_TV_KEY.TV_KEY_SLASH:
        case CONST_TV_KEY.TV_KEY_DIVIDE:
          return '/';
        case CONST_TV_KEY.TV_KEY_BACKSLASH:
          return '\\';
        case CONST_TV_KEY.TV_KEY_COLON:
          return ':';
        case CONST_TV_KEY.TV_KEY_NUMPADCOMMA:
        case CONST_TV_KEY.TV_KEY_COMMA:
          return ',';
        case CONST_TV_KEY.TV_KEY_NUMPADPERIOD:
        case CONST_TV_KEY.TV_KEY_PERIOD:
          return '.';
        case CONST_TV_KEY.TV_KEY_NUMPADEQUALS:
        case CONST_TV_KEY.TV_KEY_EQUALS:
          return '=';
        case CONST_TV_KEY.TV_KEY_LEFTBRACKET:
          return '(';
        case CONST_TV_KEY.TV_KEY_RIGHTBRACKET:
          return ')';
        case CONST_TV_KEY.TV_KEY_SPACE:
          return ' ';
        default:
          return '\0'; 
      }
    }

    /// <summary>
    /// Checks if a value evaluates to True or False
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool ToBool(this object value)
    {
      return !(value == null
              || value.ToString().Length == 0
              || "0".Equals(value.ToString().ToLower()) // no, if you put "000" this check does not pass, please don't try to be clever.
              || "false".Equals(value.ToString().ToLower())
              || "no".Equals(value.ToString().ToLower())
              );
    }
  }
}
