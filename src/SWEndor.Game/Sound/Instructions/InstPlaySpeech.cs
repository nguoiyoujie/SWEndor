namespace SWEndor.Game.Sound
{
  public partial class SoundManager
  {
    private class InstPlaySpeech : InstBase
    {
      public string Name;
      public float Volume;

      public void Process(SoundManager s)
      {
        //Channel fmodchannel;
        s.fmodsystem.playSound(s.speech[Name].GetSound(true), s.speechgrp, true, out s.current_speech_channel);
        s.current_speech_channel.setVolume(Volume * s.MasterSpeechVolume);
        s.current_speech_channel.setCallback(s.m_scb);
        s.current_speech_channel.setPaused(false);
      }
    }
  }
}
