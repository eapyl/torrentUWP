using System;

namespace TorrentUWP.BEncoding
{
    /// <summary>
    /// Class representing a BEncoded number
    /// </summary>
    public class BEncodedNumber : BEncodedValue, IComparable<BEncodedNumber>
    {
        /// <summary>
        /// The value of the BEncodedNumber
        /// </summary>
        public long Number { get; set; }

        public BEncodedNumber()
            : this(0)
        {
        }

        /// <summary>
        /// Create a new BEncoded number with the given value
        /// </summary>
        /// <param name="value">The inital value of the BEncodedNumber</param>
        public BEncodedNumber(long value)
        {
            Number = value;
        }

        public static implicit operator BEncodedNumber(long value)
        {
            return new BEncodedNumber(value);
        }

        public int CompareTo(object other)
        {
            if (other is BEncodedNumber || other is long || other is int)
                return CompareTo((BEncodedNumber)other);

            return -1;
        }

        public int CompareTo(BEncodedNumber other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return Number.CompareTo(other.Number);
        }

        public int CompareTo(long other)
        {
            return Number.CompareTo(other);
        }

        /// <summary>
        /// Encodes this number to the supplied byte[] starting at the supplied offset
        /// </summary>
        /// <param name="buffer">The buffer to write the data to</param>
        /// <param name="offset">The offset to start writing the data at</param>
        /// <returns></returns>
        public override int Encode(byte[] buffer, int offset)
        {
            var number = Number;

            var written = offset;
            buffer[written++] = (byte)'i';

            if (number < 0)
            {
                buffer[written++] = (byte)'-';
                number = -number;
            }
            // Reverse the number '12345' to get '54321'
            long reversed = 0;
            for (var i = number; i != 0; i /= 10)
                reversed = reversed * 10 + i % 10;

            // Write each digit of the reversed number to the array. We write '1' first, then '2', etc
            for (var i = reversed; i != 0; i /= 10)
                buffer[written++] = (byte)(i % 10 + '0');

            if (number == 0)
                buffer[written++] = (byte)'0';

            // If the original number ends in one or more zeros, they are lost when we reverse the
            // number. We add them back in here.
            for (var i = number; i % 10 == 0 && number != 0; i /= 10)
                buffer[written++] = (byte)'0';

            buffer[written++] = (byte)'e';
            return written - offset;
        }

        /// <summary>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var obj2 = obj as BEncodedNumber;

            return Number == obj2?.Number;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Number.GetHashCode();
        }

        /// <summary>
        /// Returns the length of the encoded string in bytes
        /// </summary>
        /// <returns></returns>
        public override int LengthInBytes()
        {
            var number = Number;
            var count = 2; // account for the 'i' and 'e'

            if (number == 0)
                return count + 1;

            if (number < 0)
            {
                number = -number;
                count++;
            }
            for (var i = number; i != 0; i /= 10)
                count++;

            return count;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return (Number.ToString());
        }

        /// <summary>
        /// Decodes a BEncoded number from the supplied RawReader
        /// </summary>
        /// <param name="reader">RawReader containing a BEncoded Number</param>
        internal override void DecodeInternal(RawReader reader)
        {
            var sign = 1;
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            if (reader.ReadByte() != 'i') // remove the leading 'i'
                throw new BEncodingException("Invalid data found. Aborting.");

            if (reader.PeekByte() == '-')
            {
                sign = -1;
                reader.ReadByte();
            }

            int letter;
            while (((letter = reader.PeekByte()) != -1) && letter != 'e')
            {
                if (letter < '0' || letter > '9')
                    throw new BEncodingException("Invalid number found.");
                Number = Number * 10 + (letter - '0');
                reader.ReadByte();
            }
            if (reader.ReadByte() != 'e') //remove the trailing 'e'
                throw new BEncodingException("Invalid data found. Aborting.");

            Number *= sign;
        }
    }
}