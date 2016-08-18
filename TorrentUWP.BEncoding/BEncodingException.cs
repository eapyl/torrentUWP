using System;

namespace TorrentUWP.BEncoding
{
    public class BEncodingException : Exception
    {
        public BEncodingException()
        {
        }

        public BEncodingException(string message)
            : base(message)
        {
        }

        public BEncodingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}