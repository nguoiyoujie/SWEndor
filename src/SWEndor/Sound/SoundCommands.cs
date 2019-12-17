namespace SWEndor.Sound
{
  public partial class SoundManager
  {
    public bool SetSoundSingle(string name, bool interrupt = true, float volume = 1.0f, bool loop = true)//, uint position = 0)
    {
      m_queuedInstructions.Enqueue(new InstPlaySoundSingle { Name = name, Loop = loop, Interrupt = interrupt, Volume = volume });
      return true;
    }

    public bool SetSound(string name, bool interrupt = true, float volume = 1.0f, bool loop = true)//, uint position = 0)
    {
      m_queuedInstructions.Enqueue(new InstPlaySound { Name = name, Loop = loop, Interrupt = interrupt, Volume = volume });
      return true;
    }

    public void SetSoundStop(string name)
    {
      m_queuedInstructions.Enqueue(new InstStopOneSound { Name = name });
    }

    public void SetSoundStopAll()
    {
      m_queuedInstructions.Enqueue(new InstStopAllSound());
    }
  }
}
