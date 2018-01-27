using System;
using Godot;

namespace GodotCSTools
{
    /// <summary>
    /// Wrapper for <see cref="Godot.File"/> that allows it to be used as a <see cref="System.IO.Stream"/>.
    /// </summary>
    public class GodotFileStream : System.IO.Stream, IDisposable
    {
        private File _file;
        private File.ModeFlags _flags;

        /// <summary>
        /// Create a new instance of the <see cref="GodotFileStream"/> class.
        /// </summary>
        /// <param name="path">The path to the file to open. This accepts Godot-style <code>user://</code> and <code>res://</code> paths.</param>
        /// <param name="flags">File flags.</param>
        /// <param name="compressionMode">If not null, this will enable compression/decompression.</param>
        public GodotFileStream(string path, File.ModeFlags flags, File.CompressionMode? compressionMode = null)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            _file = new File();
            _flags = flags;
            Error result;
            if (compressionMode.HasValue)
            {
                result = _file.OpenCompressed(path, (int)flags, (int)compressionMode.Value);
            }
            else
            {
                result = _file.Open(path, (int)flags);
            }

            if (result != Error.Ok)
            {
                throw new System.IO.IOException($"Unable to open \"{path}\": {result}");
            }
        }

        public override bool CanRead => _flags == File.ModeFlags.Read || _flags == File.ModeFlags.ReadWrite || _flags == File.ModeFlags.WriteRead;

        public override bool CanSeek => true;

        public override bool CanWrite => _flags == File.ModeFlags.Write || _flags == File.ModeFlags.ReadWrite || _flags == File.ModeFlags.WriteRead;

        public override long Length => _file.GetLen();

        public override long Position
        {
            get => _file.GetPosition();
            set => _file.Seek((int)value);
        }

        public override void Flush()
        {
            // no-op
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (!_file.IsOpen())
                throw new ObjectDisposedException($"File has been closed");

            if (!CanRead)
                throw new NotSupportedException($"Cannot Read on a GodotFileStream with flags {_flags}");

            var data = _file.GetBuffer(count);
            Buffer.BlockCopy(data, 0, buffer, offset, data.Length);
            return data.Length;
        }

        public override long Seek(long offset, System.IO.SeekOrigin origin)
        {
            if (!_file.IsOpen())
                throw new ObjectDisposedException($"File has been closed");

            switch (origin)
            {
                case System.IO.SeekOrigin.Begin:
                    _file.Seek((int)offset);
                    break;

                case System.IO.SeekOrigin.Current:
                    _file.Seek(_file.GetPosition() + (int)offset);
                    break;

                case System.IO.SeekOrigin.End:
                    _file.SeekEnd((int)offset);
                    break;
            }

            return _file.GetPosition();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException("GodotFileStream does not support SetLength");
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (!_file.IsOpen())
                throw new ObjectDisposedException($"File has been closed");

            if (!CanWrite)
                throw new NotSupportedException($"Cannot Write on a GodotFileStream with flags {_flags}");

            var data = new byte[count];
            Buffer.BlockCopy(buffer, offset, data, 0, count);
            _file.StoreBuffer(data);
        }

        protected override void Dispose(bool disposing)
        {
            _file.Close();
        }
    }
}
