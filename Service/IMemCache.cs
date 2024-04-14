using Memory_Cache.Model;

namespace Memory_Cache;

public interface IMemCache
{
    T? Get<T>(string key);
    void Set<T>(string key, T value, CacheOptions options);
    void Remove(string key);
}