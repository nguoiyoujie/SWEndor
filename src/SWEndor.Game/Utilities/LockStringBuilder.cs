using System.Text;

namespace SWEndor.Game
{
  public class LockStringBuilder
  {
    private readonly StringBuilder _builder;
    private readonly object _locker = new object();

    public LockStringBuilder() { _builder = new StringBuilder(); }

    public LockStringBuilder(int capacity) { _builder = new StringBuilder(capacity); }

    public LockStringBuilder(string value) { _builder = new StringBuilder(value); }

    public LockStringBuilder(int capacity, int maxcapacity) { _builder = new StringBuilder(capacity, maxcapacity); }

    public int Capacity { get { lock (_locker) { return _builder.Capacity; } } set { lock (_locker) { _builder.Capacity = value; } } }

    public int Length { get { lock (_locker) { return _builder.Length; } } set { lock (_locker) { _builder.Length = value; } } }

    public int MaxCapacity { get { lock (_locker) { return _builder.MaxCapacity; } } }

    public char this[int index] { get { lock (_locker) { return _builder[index]; } } }

    public StringBuilder Clear() { lock (_locker) { return _builder.Clear(); } }

    public StringBuilder Append(string text) { lock (_locker) { return _builder.Append(text); } }

    public StringBuilder AppendLine(string line) { lock (_locker) { return _builder.AppendLine(line); } }

    public StringBuilder Remove(int index, int length) { lock (_locker) { return _builder.Remove(index, length); } }

    public override string ToString() { lock (_locker) { return _builder.ToString(); } }

  }
}