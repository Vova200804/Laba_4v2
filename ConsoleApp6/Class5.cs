using System;

namespace TextFileProcessor
{
  public class TMemento
  {
    public string Content { get; private set; }
    public DateTime Timestamp { get; private set; }

    public TMemento(string content)
    {
      this.Content = content;
      Timestamp = DateTime.Now;
    }
  }
}