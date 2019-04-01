using FMOD;

namespace SWEndor.Sound
{
  public partial class SoundManager
  {
    public class InstStopAllSound : InstBase
    {
      public override void Process(SoundManager s)
      {
        foreach (string name in s.sounds.Keys)
        {
          ChannelGroup soundgrp = s.soundgrps[name];

          Channel fmodchannel;
          soundgrp.getChannel(0, out fmodchannel);
          fmodchannel.stop();
        }
      }
    }
  }
}
