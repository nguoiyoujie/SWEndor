using FMOD;

namespace SWEndor.Sound
{
  public partial class SoundManager
  {
    private class InstPlaySoundSingle : InstBase
    {
      public string Name;
      public bool Interrupt;
      public bool Loop;
      public float Volume;

      public void Process(SoundManager s)
      {
        ChannelGroup soundgrp = s.soundgrps[Name];

        Channel fmodchannel;
        soundgrp.getChannel(0, out fmodchannel);
        soundgrp.setVolume(Volume * s.SFXVolume);
        fmodchannel.setLoopCount((Loop) ? -1 : 0);

        bool bp;
        soundgrp.isPlaying(out bp);
        if (!bp || Interrupt)
        {
          fmodchannel.stop();
          s.fmodsystem.playSound(s.sounds[Name], soundgrp, false, out fmodchannel);
        }
      }
    }
  }
}
