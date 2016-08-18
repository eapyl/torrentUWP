using System;
using System.IO;

namespace TorrentUWP.BEncoding
{
    public class RawReader : Stream
    {
        private readonly Stream _input;
        private readonly byte[] _peeked;
        private bool _hasPeek;

        public RawReader(Stream input)
            : this(input, true)
        {
        }

        public RawReader(Stream input, bool strictDecoding)
        {
            _input = input;
            _peeked = new byte[1];
            StrictDecoding = strictDecoding;
        }

        public override bool CanRead => _input.CanRead;
        public override bool CanSeek => _input.CanSeek;
        public override bool CanWrite => false;
        public override long Length => _input.Length;

        public override long Position
        {
            get
            {
                if (_hasPeek)
                    return _input.Position - 1;
                return _input.Position;
            }
            set
            {
                if (value != Position)
                {
                    _hasPeek = false;
                    _input.Position = value;
                }
            }
        }

        public bool StrictDecoding { get; }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public int PeekByte()
        {
            if (!_hasPeek)
                _hasPeek = Read(_peeked, 0, 1) == 1;
            return _hasPeek ? _peeked[0] : -1;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var read = 0;
            if (_hasPeek && count > 0)
            {
                _hasPeek = false;
                buffer[offset] = _peeked[0];
                offset++;
                count--;
                read++;
            }
            read += _input.Read(buffer, offset, count);
            return read;
        }

        public override int ReadByte()
        {
            if (_hasPeek)
            {
                _hasPeek = false;
                return _peeked[0];
            }
            return base.ReadByte();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long val;
            if (_hasPeek && origin == SeekOrigin.Current)
                val = _input.Seek(offset - 1, origin);
            else
                val = _input.Seek(offset, origin);
            _hasPeek = false;
            return val;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}