using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Universal.Torrent.Bencoding
{
    /// <summary>
    /// Class representing a BEncoded Dictionary
    /// </summary>
    public class BEncodedDictionary : BEncodedValue, IDictionary<BEncodedString, BEncodedValue>
    {
        private readonly SortedDictionary<BEncodedString, BEncodedValue> _dictionary;

        public BEncodedDictionary()
        {
            _dictionary = new SortedDictionary<BEncodedString, BEncodedValue>();
        }

        public int Count => _dictionary.Count;

        public bool IsReadOnly => false;

        public ICollection<BEncodedString> Keys => _dictionary.Keys;

        public ICollection<BEncodedValue> Values => _dictionary.Values;

        public BEncodedValue this[BEncodedString key]
        {
            get { return _dictionary[key]; }
            set { _dictionary[key] = value; }
        }

        public static BEncodedDictionary DecodeTorrent(byte[] bytes)
        {
            return DecodeTorrent(new MemoryStream(bytes));
        }

        public static BEncodedDictionary DecodeTorrent(Stream s)
        {
            return DecodeTorrent(new RawReader(s));
        }

        public static BEncodedDictionary DecodeTorrent(RawReader reader)
        {
            var torrent = new BEncodedDictionary();
            if (reader.ReadByte() != 'd')
                throw new BEncodingException("Invalid data found. Aborting"); // Remove the leading 'd'

            while ((reader.PeekByte() != -1) && (reader.PeekByte() != 'e'))
            {
                var key = (BEncodedString)Decode(reader);

                BEncodedValue value;
                if (reader.PeekByte() == 'd')
                {
                    value = new BEncodedDictionary();
                    ((BEncodedDictionary)value).DecodeInternal(reader, key.Text.ToLower().Equals("info"));
                }
                else
                    value = Decode(reader); // the value is a BEncoded value

                torrent._dictionary.Add(key, value);
            }

            if (reader.ReadByte() != 'e') // remove the trailing 'e'
                throw new BEncodingException("Invalid data found. Aborting");

            return torrent;
        }

        public void Add(BEncodedString key, BEncodedValue value)
        {
            _dictionary.Add(key, value);
        }

        public void Add(KeyValuePair<BEncodedString, BEncodedValue> item)
        {
            _dictionary.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool Contains(KeyValuePair<BEncodedString, BEncodedValue> item)
        {
            return _dictionary.ContainsKey(item.Key) && _dictionary[item.Key].Equals(item.Value);
        }

        public bool ContainsKey(BEncodedString key)
        {
            return _dictionary.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<BEncodedString, BEncodedValue>[] array, int arrayIndex)
        {
            _dictionary.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Encodes the dictionary to a byte[]
        /// </summary>
        /// <param name="buffer">The buffer to encode the data to</param>
        /// <param name="offset">The offset to start writing the data to</param>
        /// <returns></returns>
        public override int Encode(byte[] buffer, int offset)
        {
            var written = 0;

            //Dictionaries start with 'd'
            buffer[offset] = (byte)'d';
            written++;

            foreach (var keypair in this)
            {
                written += keypair.Key.Encode(buffer, offset + written);
                written += keypair.Value.Encode(buffer, offset + written);
            }

            // Dictionaries end with 'e'
            buffer[offset + written] = (byte)'e';
            written++;
            return written;
        }

        public override bool Equals(object obj)
        {
            var other = obj as BEncodedDictionary;

            if (_dictionary.Count != other?._dictionary.Count)
                return false;

            foreach (var keypair in _dictionary)
            {
                BEncodedValue val;
                if (!other.TryGetValue(keypair.Key, out val))
                    return false;

                if (!keypair.Value.Equals(val))
                    return false;
            }

            return true;
        }

        public IEnumerator<KeyValuePair<BEncodedString, BEncodedValue>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        public override int GetHashCode()
        {
            var result = 0;
            foreach (var keypair in _dictionary)
            {
                result ^= keypair.Key.GetHashCode();
                result ^= keypair.Value.GetHashCode();
            }

            return result;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        /// <summary>
        /// Returns the size of the dictionary in bytes using UTF8 encoding
        /// </summary>
        /// <returns></returns>
        public override int LengthInBytes()
        {
            var length = 0;
            length += 1; // Dictionaries start with 'd'

            foreach (var keypair in _dictionary)
            {
                length += keypair.Key.LengthInBytes();
                length += keypair.Value.LengthInBytes();
            }
            length += 1; // Dictionaries end with 'e'
            return length;
        }

        public bool Remove(BEncodedString key)
        {
            return _dictionary.Remove(key);
        }

        public bool Remove(KeyValuePair<BEncodedString, BEncodedValue> item)
        {
            return _dictionary.Remove(item.Key);
        }

        public override string ToString()
        {
            return Encoding.UTF8.GetString(Encode());
        }

        public bool TryGetValue(BEncodedString key, out BEncodedValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        /// <summary>
        /// </summary>
        /// <param name="reader"></param>
        internal override void DecodeInternal(RawReader reader)
        {
            DecodeInternal(reader, reader.StrictDecoding);
        }

        private void DecodeInternal(RawReader reader, bool strictDecoding)
        {
            BEncodedString oldkey = null;

            if (reader.ReadByte() != 'd')
                throw new BEncodingException("Invalid data found. Aborting"); // Remove the leading 'd'

            while ((reader.PeekByte() != -1) && (reader.PeekByte() != 'e'))
            {
                var key = (BEncodedString)Decode(reader);

                if (oldkey != null && oldkey.CompareTo(key) > 0)
                    if (strictDecoding)
                        throw new BEncodingException(string.Format(
                            "Illegal BEncodedDictionary. The attributes are not ordered correctly. Old key: {0}, New key: {1}",
                            oldkey, key));

                oldkey = key;
                var value = Decode(reader);
                _dictionary.Add(key, value);
            }

            if (reader.ReadByte() != 'e') // remove the trailing 'e'
                throw new BEncodingException("Invalid data found. Aborting");
        }
    }
}