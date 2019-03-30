namespace SWEndor.Terminal
{
  public enum TCommandFeedbackType
  {
    NORMAL = 0,
    ERROR = 1
  }

  public struct TCommandFeedback
  {
    public readonly TCommandFeedbackType ContentType;
    public readonly object Content;

    public TCommandFeedback(TCommandFeedbackType type, object obj)
    {
      ContentType = type;
      Content = obj;
    }

    public bool IsError() { return ContentType == TCommandFeedbackType.ERROR; }

    private static TCommandFeedback m_null = new TCommandFeedback(TCommandFeedbackType.NORMAL, null);
    public static TCommandFeedback NULL
    {
      get { return m_null; }
    }
  }
}