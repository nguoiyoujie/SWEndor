namespace SWEndor
{
  internal struct LogItem
  {
    public string Channel;
    public LogType Type;
    public string Message;

    public LogItem(string channel, LogType type, string message)
    {
      Channel = channel;
      Type = type;
      Message = message;
    }
  }
}
