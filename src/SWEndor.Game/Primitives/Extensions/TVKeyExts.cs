using MTV3D65;

namespace SWEndor.Game.Primitives.Extensions
{
  public static class TVKeyExts
  {
    public static char TVKeyToChar(this CONST_TV_KEY key, bool shift)
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
  }
}
