namespace SWEndor.Game.Sound
{
  public partial class SoundManager
  {
    public bool PreloadMusic(string name)
    {
      m_queuedInstructions.Enqueue(new InstPrepMusic { Name = name });
      return true;
    }

    public bool SetMusic(string name, bool loop = false, uint position_ms = 0, uint end_ms = 0)
    {
      IntrMusic = null;
      m_queuedInstructions.Enqueue(new InstPlayMusic { Name = name, Loop = loop, Position_ms = position_ms, End_ms = end_ms });
      if (loop)
      {
        m_musicLoop.Name = name;
        m_musicLoop.Position = position_ms;
      }
      return true;
    }

    public void SetMusicLoop(string name, uint position_ms = 0)
    {
      m_musicLoop.Name = name;
      m_musicLoop.Position = position_ms;
    }

    public void SetMusicDyn(string name)
    {
      IntrMusic = null;
      Piece p = Piece.Factory.Get(name);
      SetMusic(p.SoundName, false, p.EntryPosition);
      Dynamic.PrepDynNext(this, p.SoundName);
    }

    public void StopMusic()
    {
      IntrMusic = null;
      m_queuedInstructions.Enqueue(new InstStopMusic());
    }

    public void PauseMusic()
    {
      m_queuedInstructions.Enqueue(new InstPauseMusic());
    }

    public void ResumeMusic()
    {
      m_queuedInstructions.Enqueue(new InstResumeMusic());
    }

    public void AddMusicSyncPoint(string name, string label, uint position_ms)
    {
      m_queuedInstructions.Enqueue(new InstAddSyncPoint { Name = name, Label = label, Position_ms = position_ms });
    }

    public void SetInterruptMusic(string name, uint position_ms = 0, uint end_ms = 0)
    {
      m_queuedInstructions.Enqueue(new InstPlayMusic { Name = name, isInterruptMusic = true, Position_ms = position_ms, End_ms = end_ms });
    }

    public void QueueInterruptMusic(Piece piece)
    {
      m_musicQueue.Enqueue(new SoundStartInfo { Name = piece.SoundName, Position = piece.EntryPosition, IsInterrupt = piece.IsInterrupt });
    }
  }
}
