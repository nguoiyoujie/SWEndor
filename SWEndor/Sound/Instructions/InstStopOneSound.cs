using FMOD;

namespace SWEndor.Sound
{
  public partial class SoundManager
  {
    public class InstStopOneSound : InstBase
    {
      public string Name;

      public override void Process(SoundManager s)
      {
        if (s.sounds.ContainsKey(Name))
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
