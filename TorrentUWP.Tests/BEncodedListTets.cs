using System;
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
    }
}
