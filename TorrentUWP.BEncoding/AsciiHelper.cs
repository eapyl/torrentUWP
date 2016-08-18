using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorrentUWP.BEncoding
{
    public static class AsciiHelper
    {
        public static int Write(byte[] buffer, int offset, byte value)
        {
            buffer[offset] = value;
            return 1;
        }

        public static int WriteAscii(byte[] buffer, int offset, string text)
        {
            for (var i = 0; i < text.Length; i++)
                Write(buffer, offset + i, (byte)text[i]);
            return text.Length;
        }

        public static int Write(byte[] buffer, int offset, byte[] value)
        {
            return Write(buffer, offset, value, 0, value.Length);
        }

        public static int Write(byte[] dest, int destOffset, byte[] src, int srcOffset, int count)
        {
            Buffer.BlockCopy(src, srcOffset, dest, destOffset, count);
            return count;
        }
    }
}
