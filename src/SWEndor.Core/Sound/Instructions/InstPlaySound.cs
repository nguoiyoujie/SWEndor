using FMOD;

namespace SWEndor.Sound
{
  public partial class SoundManager
  {
    private class InstPlaySound : InstBase
    {
      public string Name;
      public bool Loop;
      public float Volume;

      public void Process(SoundManager s)
      {
        Channel fmodchannel;
        s.fmodsystem.playSound(s.sounds[Name].GetSound(true), null, false, out fmodchannel);
        fmodchannel.setVolume(Volume * s.SFXVolume);
        fmodchannel.setLoopCount((Loop) ? -1 : 0);
      }
    }
  }
}
