using System;
using System.Collections.Generic;
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
    }
}
