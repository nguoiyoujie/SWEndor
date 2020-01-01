using FMOD;

namespace SWEndor.Sound
{
  public partial class SoundManager
  {
    private class InstStopOneSound : InstBase
    {
      public string Name;

      public void Process(SoundManager s)
      {
        if (s.sounds.Contains(Name))
        {
          ChannelGroup soundgrp = s.soundgrps[Name];

          Channel fmodchannel;
          soundgrp.getChannel(0, out fmodchannel);
          fmodchannel.stop();
        }
      }
    }
  }
}
