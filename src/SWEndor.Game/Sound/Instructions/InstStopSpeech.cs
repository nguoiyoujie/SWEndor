namespace SWEndor.Game.Sound
{
  public partial class SoundManager
  {
    private class InstStopSpeech : InstBase
    {
      public float FadeTime = 0.25f;

      public void Process(SoundManager s)
      {
        if (s.current_speech_channel == null)
          return;
        s.current_speech_channel.setCallback(null);
        s.current_speech_channel.stop();
      }
    }
  }
}
