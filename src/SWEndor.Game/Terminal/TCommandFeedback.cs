namespace SWEndor.Game.Terminal
{
  public enum TCommandFeedbackType
  {
    NORMAL = 0,
    ERROR = 1
  }

  public struct TCommandFeedback
  {
    public readonly TCommandFeedbackType ContentType;
    public readonly string Content;

    public TCommandFeedback(TCommandFeedbackType type, string response)
    {
      ContentType = type;
      Content = response;
    }

    public bool IsError() { return ContentType == TCommandFeedbackType.ERROR; }

    private static TCommandFeedback m_null = new TCommandFeedback(TCommandFeedbackType.NORMAL, null);
    public static TCommandFeedback NULL
    {
      get { return m_null; }
    }
  }
}