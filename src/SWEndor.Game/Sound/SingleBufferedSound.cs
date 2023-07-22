using System.Collections.Generic;

namespace SWEndor.Game.Sound
{
  public struct SingleBufferedSound
  {
    private FMOD.Sound _s1;
    public SingleBufferedSound(FMOD.System fmodsystem, string soundfile)
    {
      fmodsystem.createStream(soundfile, FMOD.MODE.LOWMEM | FMOD.MODE.CREATESTREAM | FMOD.MODE.ACCURATETIME, out _s1);
    }

    public FMOD.Sound GetSound(bool toggleSwitch)
    {
      return _s1;
    }

    public IEnumerable<FMOD.Sound> GetSounds()
    {
      yield return _s1;
    }
  }
}
