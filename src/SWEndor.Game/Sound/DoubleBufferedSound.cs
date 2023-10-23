namespace SWEndor.Game.Sound
{
  public struct DoubleBufferedSound
  {
    private FMOD.Sound _s1;
    private FMOD.Sound _s2;
    private FMOD.Sound[] _s;
    private bool _switch;
    public DoubleBufferedSound(FMOD.System fmodsystem, string soundfile)
    {
      fmodsystem.createStream(soundfile, FMOD.MODE.LOWMEM | FMOD.MODE.CREATESTREAM | FMOD.MODE.ACCURATETIME, out _s1);
      fmodsystem.createStream(soundfile, FMOD.MODE.LOWMEM | FMOD.MODE.CREATESTREAM | FMOD.MODE.ACCURATETIME, out _s2);
      _s = new FMOD.Sound[] { _s1, _s2 };
      _switch = false;
    }

    public FMOD.Sound GetSound(bool toggleSwitch)
    {
      FMOD.Sound s = _switch ? _s2 : _s1;
      if (toggleSwitch)
        _switch = !_switch;
      return s;
    }

    public FMOD.Sound[] GetSounds()
    {
      return _s;
    }
  }
}
