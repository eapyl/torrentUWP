
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TorrentUWP.BEncoding
{
    /// <summary>
    /// Class representing a BEncoded list
    /// </summary>
    public class BEncodedList : BEncodedValue, IList<BEncodedValue>
    {
        private readonly List<BEncodedValue> _list;

        public int Count => _list.Count;

        public bool IsReadOnly => false;

        public BEncodedValue this[int index]
        {
            get { return _list[index]; }
            set { _list[index] = value; }
        }

        public void Add(BEncodedValue item)
        {
            _list.Add(item);
        }

        public void AddRange(IEnumerable<BEncodedValue> collection)
        {
            _list.AddRange(collection);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(BEncodedValue item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(BEncodedValue[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Encodes the list to a byte[]
        /// </summary>
        /// <param name="buffer">The buffer to encode the list to</param>
        /// <param name="offset">The offset to start writing the data at</param>
        /// <returns></returns>
        public override int Encode(byte[] buffer, int offset)
        {
            var written = 0;
            buffer[offset] = (byte)'l';
            written++;
            written = _list.Aggregate(written, (current, t) => current + t.Encode(buffer, offset + current));
            buffer[offset + written] = (byte)'e';
            written++;
            return written;
        }

        public override bool Equals(object obj)
        {
            var other = obj as BEncodedList;

            if (other == null)
                return false;

            return !_list.Where((t, i) => !t.Equals(other._list[i])).Any();
        }

        public IEnumerator<BEncodedValue> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public override int GetHashCode()
        {
            return _list.Aggregate(0, (current, t) => current ^ t.GetHashCode());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int IndexOf(BEncodedValue item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, BEncodedValue item)
        {
            _list.Insert(index, item);
        }

        /// <summary>
        /// Returns the size of the list in bytes
        /// </summary>
        /// <returns></returns>
        public override int LengthInBytes()
        {
            var length = 0;

            length += 1; // Lists start with 'l'
            length += _list.Sum(t => t.LengthInBytes());

            length += 1; // Lists end with 'e'
            return length;
        }

        /// <summary>
        /// Create a new BEncoded List with default capacity
        /// </summary>
        public BEncodedList()
            : this(new List<BEncodedValue>())
        {
        }

        /// <summary>
        /// Create a new BEncoded List with the supplied capacity
        /// </summary>
        /// <param name="capacity">The initial capacity</param>
        public BEncodedList(int capacity)
            : this(new List<BEncodedValue>(capacity))
        {
        }

        public BEncodedList(IEnumerable<BEncodedValue> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            _list = new List<BEncodedValue>(list);
        }

        private BEncodedList(List<BEncodedValue> value)
        {
            _list = value;
        }

        public bool Remove(BEncodedValue item)
        {
            return _list.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public override string ToString()
        {
            return Encoding.UTF8.GetString(Encode());
        }

        /// <summary>
        /// Decodes a BEncodedList from the given StreamReader
        /// </summary>
        /// <param name="reader"></param>
        internal override void DecodeInternal(RawReader reader)
        {
            if (reader.ReadByte() != 'l') // Remove the leading 'l'
                throw new BEncodingException("BEncoded List should be started by 'l'");

            while ((reader.PeekByte() != -1) && (reader.PeekByte() != 'e'))
                _list.Add(Decode(reader));

            if (reader.ReadByte() != 'e') // Remove the trailing 'e'
                throw new BEncodingException("BEncoded List should be started by 'e'");
        }
    }
}