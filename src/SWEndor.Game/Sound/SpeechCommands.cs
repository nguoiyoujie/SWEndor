namespace SWEndor.Game.Sound
{
  public partial class SoundManager
  {
    public bool QueueSpeech(string name)
    {
      if (CurrSpeech == null && m_speechQueue.Count == 0)
      {
        PlaySpeech(name);
      }
      else
      {
        m_speechQueue.Enqueue(name);
      }
      return true;
    }
    public bool PlaySpeech(string name, float volume = 1.0f)
    {
      CurrSpeech = name;
      m_queuedInstructions.Enqueue(new InstPlaySpeech { Name = name, Volume = volume });
      return true;
    }

    public void StopSpeech()
    {
      CurrSpeech = null;
      m_queuedInstructions.Enqueue(new InstStopSpeech());
    }
  }
}
