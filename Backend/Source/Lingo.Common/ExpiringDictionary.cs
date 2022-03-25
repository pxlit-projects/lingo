using System.Collections.Concurrent;

namespace Lingo.Common
{
    //DO NOT TOUCH THIS FILE!!
    public class ExpiringDictionary<TKey, TValue>
    {
        private readonly TimeSpan _entryLifeSpan;
        private readonly ConcurrentDictionary<TKey, Entry> _entries;
        private DateTimeOffset _lastExpirationScan;

        public IReadOnlyList<TValue> Values
        {
            get
            {
                return _entries.Values.Select(entry => entry.Value).ToList();
            }
        }

        public ExpiringDictionary()
        {
            _entries = new ConcurrentDictionary<TKey, Entry>();
            _entryLifeSpan = TimeSpan.FromSeconds(180);
        }

        public ExpiringDictionary(TimeSpan entryLifeSpan)
        {
            _entryLifeSpan = entryLifeSpan;
            _entries = new ConcurrentDictionary<TKey, Entry>();
        }

        public void AddOrReplace(TKey key, TValue value)
        {
            _entries.AddOrUpdate(key, k => new Entry(value, _entryLifeSpan), (k, oldValue) => new Entry(value, _entryLifeSpan));
            StartScanForExpiredEntries();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            bool result = false;
            value = default;
            if (_entries.TryGetValue(key, out Entry entry))
            {
                value = entry.Value;
                result = true;
            }

            StartScanForExpiredEntries();

            return result;
        }

        public bool TryRemove(TKey key, out TValue removedValue)
        {
            if (_entries.TryRemove(key, out Entry removedEntry))
            {
                removedValue = removedEntry.Value;
                return true;
            }

            removedValue = default;
            return false;
        }

        private void StartScanForExpiredEntries()
        {
            var now = DateTimeOffset.Now;
            if (TimeSpan.FromSeconds(30) < now - _lastExpirationScan)
            {
                _lastExpirationScan = now;
                Task.Factory.StartNew(state => ScanForExpiredItems((ConcurrentDictionary<TKey, Entry>)state), _entries,
                    CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
            }
        }

        private static void ScanForExpiredItems(ConcurrentDictionary<TKey, Entry> dictionary)
        {
            var now = DateTimeOffset.Now;
            foreach (var keyValue in dictionary)
            {
                if (keyValue.Value.IsExpired(now))
                {
                    dictionary.TryRemove(keyValue.Key, out Entry _);
                }
            }
        }

        private class Entry
        {
            private DateTimeOffset _expiration { get; }

            public TValue Value { get; }

            public Entry(TValue value, TimeSpan lifeTime)
            {
                Value = value;
                _expiration = DateTimeOffset.Now.Add(lifeTime);
            }

            public bool IsExpired(DateTimeOffset now)
            {
                return now >= _expiration;
            }
        }
    }
}