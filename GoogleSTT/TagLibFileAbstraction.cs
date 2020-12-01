using System.IO;

namespace GoogleSTT
{
    class TagLibFileAbstraction : TagLib.File.IFileAbstraction
    {
        public string Name { get; private set; }

        public Stream ReadStream { get; private set; }

        public Stream WriteStream { get; private set; }

        public int AudioSampleRate { get { return TagLib.File.Create(this).Properties.AudioSampleRate; } }

        public TagLibFileAbstraction(string name, byte[] bytes)
        {
            Name = name;

            var stream = new MemoryStream(bytes);

            ReadStream = stream;
            WriteStream = stream;
        }

        public void CloseStream(Stream stream)
        {
            stream.Dispose();
        }
    }
}
