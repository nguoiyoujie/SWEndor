using MTV3D65;
using SWEndor.Sound;
using SWEndor.Terminal;

namespace SWEndor.Input.Context
{
  public class MenuInputContext : AInputContext
  {
    public override void HandleKeyBuffer(TV_KEYDATA keydata)
    {
      if (keydata.Pressed > 0)
      {
        if (Screen2D.Instance().CurrentPage?.OnKeyPress((CONST_TV_KEY)keydata.Key) ?? false)
          SoundManager.Instance().SetSound("button_1");

        // Terminal
        if (InputManager.Instance().CTRL && keydata.Key.Equals((int)CONST_TV_KEY.TV_KEY_T))
          TConsole.Visible = true;

        // Terminal
        if (keydata.Key.Equals((int)CONST_TV_KEY.TV_KEY_0))
        {
          SoundManager.Instance().QueueMusic("trofix", 2142);
          trofix = true;
          SoundManager.Instance().SetSound("button_1");
        }

        if (keydata.Key.Equals((int)CONST_TV_KEY.TV_KEY_1))
        {
          if (trofix)
            SoundManager.Instance().QueueMusic("trofix", 64286);

          if (waitfix)
            SoundManager.Instance().QueueMusic("waitfix", 41922);

          SoundManager.Instance().QueueMusic("rebfix", 2142);
          trofix = false;
          waitfix = false;
          SoundManager.Instance().SetSound("button_1");
        }

        if (keydata.Key.Equals((int)CONST_TV_KEY.TV_KEY_2))
        {
          SoundManager.Instance().SetInterruptMusic("dynamic\\S-EMP-SM");
          SoundManager.Instance().SetSound("button_1");
        }

        if (keydata.Key.Equals((int)CONST_TV_KEY.TV_KEY_3))
        {
          SoundManager.Instance().SetInterruptMusic("dynamic\\S-EMP-LG");
          SoundManager.Instance().QueueMusic("confix", 2142);
          trofix = false;
          waitfix = false;
          SoundManager.Instance().SetSound("button_1");
        }

        if (keydata.Key.Equals((int)CONST_TV_KEY.TV_KEY_4))
        {
          SoundManager.Instance().SetInterruptMusic("dynamic\\rebelfighterarrv");
          SoundManager.Instance().QueueMusic("rebfix", 2142);
          trofix = false;
          waitfix = false;
          SoundManager.Instance().SetSound("button_1");
        }

        if (keydata.Key.Equals((int)CONST_TV_KEY.TV_KEY_5))
        {
          SoundManager.Instance().SetInterruptMusic("dynamic\\rebelcapshiparrv");
          SoundManager.Instance().SetSound("button_1");
        }

        if (keydata.Key.Equals((int)CONST_TV_KEY.TV_KEY_6))
        {
          if (trofix)
            SoundManager.Instance().QueueMusic("trofix", 64287);

          if (waitfix)
            SoundManager.Instance().QueueMusic("waitfix", 41923);

          SoundManager.Instance().QueueMusic("waitfix", 2142);
          trofix = false;
          waitfix = true;
          SoundManager.Instance().SetSound("button_1");
        }

        if (keydata.Key.Equals((int)CONST_TV_KEY.TV_KEY_7))
        {
          if (trofix)
            SoundManager.Instance().QueueMusic("trofix", 64287);

          if (waitfix)
            SoundManager.Instance().QueueMusic("waitfix", 41923);

          SoundManager.Instance().QueueMusic("trofix", 2142);
          trofix = true;
          waitfix = false;
          SoundManager.Instance().SetSound("button_1");
        }

        if (keydata.Key.Equals((int)CONST_TV_KEY.TV_KEY_8))
        {
          if (trofix)
            SoundManager.Instance().QueueMusic("trofix", 64287);

          if (waitfix)
            SoundManager.Instance().QueueMusic("waitfix", 41923);

          SoundManager.Instance().QueueMusic("polfix", 2142);
          trofix = false;
          waitfix = false;
          SoundManager.Instance().SetSound("button_1");
        }
      }
    }

    public bool trofix = true;
    public bool waitfix = false;
  }
}
