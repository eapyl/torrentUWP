using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorrentUWP.BEncoding
{
    public static class Toolbox
    {
        public static bool ByteMatch(byte[] array1, int offset1, byte[] array2, int offset2, int count)
        {
            if (array1 == null)
                throw new ArgumentNullException(nameof(array1));
            if (array2 == null)
                throw new ArgumentNullException(nameof(array2));

            // If either of the arrays is too small, they're not equal
            if ((array1.Length - offset1) < count || (array2.Length - offset2) < count)
                return false;

            // Check if any elements are unequal
            for (var i = 0; i < count; i++)
                if (array1[offset1 + i] != array2[offset2 + i])
                    return false;

            return true;
        }

        public static bool ByteMatch(byte[] array1, byte[] array2)
        {
            if (array1 == null)
                throw new ArgumentNullException(nameof(array1));
            if (array2 == null)
                throw new ArgumentNullException(nameof(array2));

            return array1.Length == array2.Length && ByteMatch(array1, 0, array2, 0, array1.Length);
        }
    }
}
