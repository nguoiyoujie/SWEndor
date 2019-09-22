﻿using FMOD;

namespace SWEndor.Sound
{
  public partial class SoundManager
  {
    private class InstPrepMusic : InstBase
    {
      public string Name;

      public void Process(SoundManager s)
      {
        if (Name == s.m_currMusic)
        {
          Name += "%";
        }

        if (s.music.ContainsKey(Name))
        {
          Channel dummy = null;
          s.fmodsystem.playSound(s.music[Name], s.musicgrp, true, out dummy);
        }
      }
    }
  }
}