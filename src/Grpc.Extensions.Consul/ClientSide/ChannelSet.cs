using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Grpc.Extensions.Consul.ClientSide
{
    public class ChannelSet : IEnumerable<ChannelEntry>
    {
        private int _currentIndex;
        private readonly ChannelEntry[] _items;

        public ChannelSet() : this(Array.Empty<ChannelEntry>())
        {
        }

        public ChannelSet(IEnumerable<ChannelEntry> items)
        {
            _items = items.ToArray();
        }

        public ChannelEntry Next()
        {
            if (_items.Length < 1)
            {
                throw new Exception($"ChannelSet 为空。");
            }
            return _items[_currentIndex++ % _items.Length];
        }

        public bool HasChannel()
        {
            return _items.Length > 0;
        }

        public IEnumerator<ChannelEntry> GetEnumerator()
        {
            return _items.AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }
    }
}