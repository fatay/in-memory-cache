using System.Collections.Concurrent;
using Memory_Cache.Model;

namespace Memory_Cache;

public class MemCache : IMemCache
{
    private class CacheItem
    {
        public object? Value { get; set; }
        public DateTime? AbsoluteExpiration { get; set; }
        public TimeSpan? SlidingExpiration { get; set; }
        public DateTime LastAccessTime { get; set; }
    }

    private readonly ConcurrentDictionary<string, CacheItem> _cache = new();

    public T? Get<T>(string key)
    {
        if (!_cache.TryGetValue(key, out var item)) 
            return default;
        
        if (IsExpired(item))
        {
            _cache.TryRemove(key, out _);
            return default;
        }

        item.LastAccessTime = DateTime.Now;
        return (T)item.Value!;

    }

    public void Set<T>(string key, T value, CacheOptions options)
    {
        if (value != null)
        {
            var cacheItem = new CacheItem
            {
                Value = value,
                AbsoluteExpiration = options.AbsoluteExpiration.HasValue ? DateTime.Now.Add(options.AbsoluteExpiration.Value) : null,
                SlidingExpiration = options.SlidingExpiration,
                LastAccessTime = DateTime.Now
            };

            _cache[key] = cacheItem;
        }
    }

    public void Remove(string key)
    {
        _cache.TryRemove(key, out _);
    }

    private bool IsExpired(CacheItem item)
    {
        var now = DateTime.Now;

        // Check absolute expiration
        bool isAbsoluteExpired = item.AbsoluteExpiration.HasValue && item.AbsoluteExpiration.Value < now;

        // Check sliding expiration
        bool isSlidingExpired = item.SlidingExpiration.HasValue && 
                                item.LastAccessTime.Add(item.SlidingExpiration.Value) < now;

        return isAbsoluteExpired || isSlidingExpired;
    }

}