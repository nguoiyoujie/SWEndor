namespace SWEndor.Game.Sound
{
  public partial class SoundManager
  {
    public bool SetSoundSingle(string name, bool interrupt = true, float volume = 1.0f, bool loop = true)//, uint position = 0)
    {
      m_queuedInstructions.Enqueue(new InstPlaySoundSingle { Name = name, Loop = loop, Interrupt = interrupt, Volume = volume });
      return true;
    }

    public bool SetSound(string name, float volume = 1.0f, bool loop = true)//, uint position = 0)
    {
      m_queuedInstructions.Enqueue(new InstPlaySound { Name = name, Loop = loop, Volume = volume });
      return true;
    }

    public void StopSound(string name)
    {
      m_queuedInstructions.Enqueue(new InstStopOneSound { Name = name });
    }

    public void StopAllSounds()
    {
      m_queuedInstructions.Enqueue(new InstStopAllSound());
    }
  }
}
