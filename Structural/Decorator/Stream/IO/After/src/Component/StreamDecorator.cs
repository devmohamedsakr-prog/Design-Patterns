using System;
using System.Collections.Generic;

namespace Decorator.Stream.IO.Component
{
    public abstract class DataStream
    {
        public abstract string Read();
        public abstract void Write(string data);
        public abstract int GetSize();
    }

    public class FileStream : DataStream
    {
        private readonly string _filename;
        private string _data;

        public FileStream(string filename)
        {
            _filename = filename;
            _data = $"Content of {filename}";
        }

        public override string Read() => _data;
        public override void Write(string data) => _data = data;
        public override int GetSize() => _data.Length;

        public override string ToString() => $"FileStream({_filename}, {GetSize()} bytes)";
    }

    public abstract class StreamDecorator : DataStream
    {
        protected DataStream _wrappedStream;

        public StreamDecorator(DataStream stream)
        {
            _wrappedStream = stream ?? throw new ArgumentNullException(nameof(stream));
        }
    }

    public class BufferDecorator : StreamDecorator
    {
        private readonly List<string> _buffer;
        public int BufferSize { get; set; }

        public BufferDecorator(DataStream stream, int bufferSize = 1024) : base(stream)
        {
            _buffer = new List<string>();
            BufferSize = bufferSize;
        }

        public override string Read()
        {
            var data = _wrappedStream.Read();
            _buffer.Add($"[Buffered: {DateTime.UtcNow:O}] {data.Substring(0, Math.Min(50, data.Length))}");
            return data;
        }

        public override void Write(string data)
        {
            _buffer.Add($"[Buffer Write] {data}");
            _wrappedStream.Write(data);
        }

        public override int GetSize() => _wrappedStream.GetSize() + (_buffer.Count * 100);

        public override string ToString() => $"BufferDecorator({_wrappedStream}, buffer={_buffer.Count})";
    }

    public class CompressionDecorator : StreamDecorator
    {
        public string CompressionType { get; set; }
        private int _compressionRatio;

        public CompressionDecorator(DataStream stream) : base(stream)
        {
            CompressionType = "GZIP";
            _compressionRatio = 3; // Assume 3:1 compression
        }

        public override string Read()
        {
            var data = _wrappedStream.Read();
            return $"[Decompressed: {CompressionType}] {data}";
        }

        public override void Write(string data)
        {
            _wrappedStream.Write($"[Compressed: {CompressionType}] {data}");
        }

        public override int GetSize() => _wrappedStream.GetSize() / _compressionRatio;

        public override string ToString() => $"CompressionDecorator({_wrappedStream}, type={CompressionType})";
    }

    public class EncryptionDecorator : StreamDecorator
    {
        public string Algorithm { get; set; }
        private string _key;

        public EncryptionDecorator(DataStream stream, string key = "secret") : base(stream)
        {
            Algorithm = "AES-256";
            _key = key;
        }

        public override string Read()
        {
            var data = _wrappedStream.Read();
            return $"[Decrypted: {Algorithm}] {data}";
        }

        public override void Write(string data)
        {
            _wrappedStream.Write($"[Encrypted: {Algorithm}] {data}");
        }

        public override int GetSize() => _wrappedStream.GetSize();

        public override string ToString() => $"EncryptionDecorator({_wrappedStream}, algo={Algorithm})";
    }

    public class LoggingDecorator : StreamDecorator
    {
        private readonly List<string> _logs;

        public LoggingDecorator(DataStream stream) : base(stream)
        {
            _logs = new List<string>();
        }

        public override string Read()
        {
            _logs.Add($"[READ] {DateTime.UtcNow:O}");
            return _wrappedStream.Read();
        }

        public override void Write(string data)
        {
            _logs.Add($"[WRITE] {DateTime.UtcNow:O} - {data.Length} bytes");
            _wrappedStream.Write(data);
        }

        public override int GetSize() => _wrappedStream.GetSize();

        public IReadOnlyList<string> GetLogs() => _logs.AsReadOnly();

        public override string ToString() => $"LoggingDecorator({_wrappedStream}, logs={_logs.Count})";
    }

    public class CachingDecorator : StreamDecorator
    {
        private string _cachedData;
        private bool _isCached;

        public CachingDecorator(DataStream stream) : base(stream)
        {
            _isCached = false;
        }

        public override string Read()
        {
            if (_isCached)
                return $"[Cached] {_cachedData}";

            _cachedData = _wrappedStream.Read();
            _isCached = true;
            return _cachedData;
        }

        public override void Write(string data)
        {
            _wrappedStream.Write(data);
            _cachedData = data;
            _isCached = true;
        }

        public override int GetSize() => _wrappedStream.GetSize();

        public void InvalidateCache() => _isCached = false;

        public override string ToString() => $"CachingDecorator({_wrappedStream}, cached={_isCached})";
    }
}
