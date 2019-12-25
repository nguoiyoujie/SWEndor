using FMOD;

namespace SWEndor.Sound
{
  public partial class SoundManager
  {
    private class InstPlaySound : InstBase
    {
      public string Name;
      //public bool Interrupt;
      public bool Loop;
      public float Volume;

      public void Process(SoundManager s)
      {
        /*
        ChannelGroup soundgrp = s.soundgrps[Name];

        Channel fmodchannel;
        soundgrp.getChannel(0, out fmodchannel);
        soundgrp.setVolume(Volume * s.MasterSFXVolume);
        fmodchannel.setLoopCount((Loop) ? -1 : 0);
        //fmodchannel.setMode((sound.Loop) ? MODE.LOOP_NORMAL : MODE.LOOP_OFF);

        bool bp;
        soundgrp.isPlaying(out bp);
        if (!bp || Interrupt)
        {
          fmodchannel.stop();
          s.fmodsystem.playSound(s.sounds[Name], soundgrp, false, out fmodchannel);
        }
        */
        Channel fmodchannel;
        //ChannelGroup soundgrp = s.soundgrps[Name];
        //soundgrp.setVolume(Volume * s.MasterSFXVolume);
        s.fmodsystem.playSound(s.sounds[Name], null, false, out fmodchannel);
        fmodchannel.setVolume = Volume * s.MasterSFXVolume;
        fmodchannel.setLoopCount((Loop) ? -1 : 0);
      }
    }
  }
}
