using System;
using System.Collections.Generic;
using System.IO;
using TorrentUWP.BEncoding;
using Xunit;

namespace TorrentUWP.Tests
{
    public class BEncodedListTets
    {
        private readonly BEncodedList _target;

        public BEncodedListTets()
        {
            _target = new BEncodedList();
        }

        [Fact]
        public void ShouldAddValueToCollection()
        {
            var item = new BEncodedString(string.Empty);
            _target.Add(item);

            Assert.Contains(_target, x => x == item);
        }

        [Fact]
        public void ShouldAddValuesToCollection()
        {
            var item = new BEncodedString(string.Empty);
            var items = new List<BEncodedValue> { item };
            _target.AddRange(items);

            Assert.Contains(_target, x => x == item);
        }

        [Fact]
        public void ShouldClearValuesInCollection()
        {
            var item = new BEncodedString(string.Empty);
            _target.Add(item);

            Assert.Contains(_target, x => x == item);

            _target.Clear();

            Assert.DoesNotContain(_target, x => x == item);
        }

        [Fact]
        public void ShouldContainIfAdded()
        {
            var item = new BEncodedString(string.Empty);
            _target.Add(item);

            Assert.True(_target.Contains(item));
        }

        [Fact]
        public void ShouldCopyToArray()
        {
            var item = new BEncodedString(string.Empty);
            _target.Add(item);

            var array = new BEncodedString[1];
            _target.CopyTo(array, 0);

            Assert.Contains(array, x=>x == item);
        }

        [Fact]
        public void ConvertInternalListToBytes()
        {
            var item = new BEncodedString(string.Empty);
            _target.Add(item);

            var buffer = new byte[4];
            var written = _target.Encode(buffer, 0);

            Assert.Equal(buffer[0], (byte)'l');
            Assert.Equal(buffer[1], (byte)'0');
            Assert.Equal(buffer[2], (byte)':');
            Assert.Equal(buffer[3], (byte)'e');
            Assert.Equal(written, 4);
        }

        [Fact]
        public void TwoObjectsShouldBeEqualIfContainsTheSame()
        {
            var item = new BEncodedString(string.Empty);
            _target.Add(item);

            var second = new BEncodedList();
            second.Add(new BEncodedString(string.Empty));

            Assert.Equal(_target, second);
        }

        [Fact]
        public void ShouldInsertInTheBegin()
        {
            var item = new BEncodedString(string.Empty);
            var item2 = new BEncodedString("0");
            _target.Add(item);

            _target.Insert(0, item2);

            Assert.True(_target[0] == item2);
        }

        [Fact]
        public void ShouldReturnCorretNumberOfBytes()
        {
            var item = new BEncodedString(string.Empty);
            _target.Add(item);

            Assert.Equal(_target.LengthInBytes(), 4);
        }

        [Fact]
        public void ShouldRemoveItem()
        {
            var item = new BEncodedString(string.Empty);
            _target.Add(item);

            _target.Remove(item);

            Assert.DoesNotContain(item, _target);
        }

        [Fact]
        public void ShouldRemoveAtItem()
        {
            var item = new BEncodedString(string.Empty);
            _target.Add(item);

            _target.RemoveAt(0);

            Assert.DoesNotContain(item, _target);
        }

        [Fact]
        public void ShouldReturnStringRepresentation()
        {
            var item = new BEncodedString(string.Empty);
            _target.Add(item);

            var list = _target.ToString();

            Assert.Equal(list, "l0:e");
        }
    }
}
