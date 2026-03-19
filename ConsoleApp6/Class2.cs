using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace TextFileProcessor
{
  [Serializable]
  public class TDocument
  {
    public string FilePath { get; private set; }
    public string Content { get; set; }
    public DateTime LastModified { get; private set; }

    private TDocument()
    {

    }

    public TDocument(string path)
    {
      if (!File.Exists(path))
      {
        throw new Exception($"File not found: {path}");
      }

      FilePath = Path.GetFullPath(path);
      Content = File.ReadAllText(path);
      LastModified = File.GetLastWriteTime(path);
    }

    public TDocument(string path, string initialContent)
    {
      FilePath = Path.GetFullPath(path);
      Content = initialContent;
      LastModified = DateTime.Now;
    }

    public void Save()
    {
      string directory;
      directory = Path.GetDirectoryName(FilePath);

      if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
      {
        Directory.CreateDirectory(directory);
      }

      File.WriteAllText(FilePath, Content);
      LastModified = File.GetLastWriteTime(FilePath);
    }

    public TMemento CreateMemento()
    {
      TMemento memento;
      memento = new TMemento(Content);

      return memento;
    }

    public void RestoreMemento(TMemento memento)
    {
      Content = memento.Content;
    }

    public void BinarySerialize(string targetPath)
    {
      FileStream stream;
      stream = new FileStream(targetPath, FileMode.Create);

      using (stream)
      {
        BinaryWriter writer;
        writer = new BinaryWriter(stream, Encoding.UTF8);

        using (writer)
        {
          writer.Write(FilePath ?? "");
          writer.Write(Content ?? "");
          writer.Write(LastModified.Ticks);
        }
      }
    }

    public static TDocument BinaryDeserialize(string path)
    {
      FileStream stream;
      stream = new FileStream(path, FileMode.Open);

      using (stream)
      {
        BinaryReader reader;
        reader = new BinaryReader(stream, Encoding.UTF8);

        using (reader)
        {
          TDocument document;
          document = new TDocument
          {
            FilePath = reader.ReadString(),
            Content = reader.ReadString(),
            LastModified = new DateTime(reader.ReadInt64())
          };

          return document;
        }
      }
    }

    public void XmlSerialize(string targetPath)
    {
      StreamWriter writer;
      writer = new StreamWriter(targetPath);

      using (writer)
      {
        XmlSerializer serializer;
        serializer = new XmlSerializer(typeof(TDocument));

        serializer.Serialize(writer, this);
      }
    }

    public static TDocument XmlDeserialize(string path)
    {
      StreamReader reader;
      reader = new StreamReader(path);

      using (reader)
      {
        XmlSerializer serializer;
        serializer = new XmlSerializer(typeof(TDocument));

        TDocument document;
        document = (TDocument)serializer.Deserialize(reader);

        return document;
      }
    }

    public override string ToString()
    {
      string fileName;
      fileName = Path.GetFileName(FilePath);

      return $"{fileName}: {Content.Length} chars, modified {LastModified:HH:mm:ss}";
    }

    public string GetFullPath()
    {
      return FilePath;
    }
  }
}