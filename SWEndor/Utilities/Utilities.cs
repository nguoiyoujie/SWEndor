using MTV3D65;
using SWEndor.Input;
using System;

namespace SWEndor
{
  public static class Utilities
  {
    public static TV_3DVECTOR GetRotation(TV_3DVECTOR direction)
    {
      float x = Engine.Instance().TVMathLibrary.Direction2Ang(-direction.y, Engine.Instance().TVMathLibrary.TVVec2Length(new TV_2DVECTOR(direction.z, direction.x)));
      float y = Engine.Instance().TVMathLibrary.Direction2Ang(direction.x, direction.z);

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

    public static char TVKeyToChar(CONST_TV_KEY key)
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
          return InputManager.Instance().SHIFT ? 'A' : 'a';
        case CONST_TV_KEY.TV_KEY_B:
          return InputManager.Instance().SHIFT ? 'B' : 'b';
        case CONST_TV_KEY.TV_KEY_C:
          return InputManager.Instance().SHIFT ? 'C' : 'c';
        case CONST_TV_KEY.TV_KEY_D:
          return InputManager.Instance().SHIFT ? 'D' : 'd';
        case CONST_TV_KEY.TV_KEY_E:
          return InputManager.Instance().SHIFT ? 'E' : 'e';
        case CONST_TV_KEY.TV_KEY_F:
          return InputManager.Instance().SHIFT ? 'F' : 'f';
        case CONST_TV_KEY.TV_KEY_G:
          return InputManager.Instance().SHIFT ? 'G' : 'g';
        case CONST_TV_KEY.TV_KEY_H:
          return InputManager.Instance().SHIFT ? 'H' : 'h';
        case CONST_TV_KEY.TV_KEY_I:
          return InputManager.Instance().SHIFT ? 'I' : 'i';
        case CONST_TV_KEY.TV_KEY_J:
          return InputManager.Instance().SHIFT ? 'J' : 'j';
        case CONST_TV_KEY.TV_KEY_K:
          return InputManager.Instance().SHIFT ? 'K' : 'k';
        case CONST_TV_KEY.TV_KEY_L:
          return InputManager.Instance().SHIFT ? 'L' : 'l';
        case CONST_TV_KEY.TV_KEY_M:
          return InputManager.Instance().SHIFT ? 'M' : 'm';
        case CONST_TV_KEY.TV_KEY_N:
          return InputManager.Instance().SHIFT ? 'N' : 'n';
        case CONST_TV_KEY.TV_KEY_O:
          return InputManager.Instance().SHIFT ? 'O' : 'o';
        case CONST_TV_KEY.TV_KEY_P:
          return InputManager.Instance().SHIFT ? 'P' : 'p';
        case CONST_TV_KEY.TV_KEY_Q:
          return InputManager.Instance().SHIFT ? 'Q' : 'q';
        case CONST_TV_KEY.TV_KEY_R:
          return InputManager.Instance().SHIFT ? 'R' : 'r';
        case CONST_TV_KEY.TV_KEY_S:
          return InputManager.Instance().SHIFT ? 'S' : 's';
        case CONST_TV_KEY.TV_KEY_T:
          return InputManager.Instance().SHIFT ? 'T' : 't';
        case CONST_TV_KEY.TV_KEY_U:
          return InputManager.Instance().SHIFT ? 'U' : 'u';
        case CONST_TV_KEY.TV_KEY_V:
          return InputManager.Instance().SHIFT ? 'V' : 'v';
        case CONST_TV_KEY.TV_KEY_W:
          return InputManager.Instance().SHIFT ? 'W' : 'w';
        case CONST_TV_KEY.TV_KEY_X:
          return InputManager.Instance().SHIFT ? 'X' : 'x';
        case CONST_TV_KEY.TV_KEY_Y:
          return InputManager.Instance().SHIFT ? 'Y' : 'y';
        case CONST_TV_KEY.TV_KEY_Z:
          return InputManager.Instance().SHIFT ? 'Z' : 'z';

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
  }
}
