using ETicaret.Core.Services;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace ETicaret.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<CacheService> _logger;
        private readonly Dictionary<string, DateTime> _cacheTimestamps;

        public CacheService(IMemoryCache memoryCache, ILogger<CacheService> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
            _cacheTimestamps = new Dictionary<string, DateTime>();
        }

        public async Task<T?> GetAsync<T>(string key) where T : class
        {
            try
            {
                if (_memoryCache.TryGetValue(key, out var cachedValue))
                {
                    if (cachedValue is T directValue)
                    {
                        _logger.LogDebug("Cache hit for key: {Key}", key);
                        return directValue;
                    }

                    if (cachedValue is string jsonValue)
                    {
                        var deserializedValue = JsonSerializer.Deserialize<T>(jsonValue);
                        _logger.LogDebug("Cache hit for key: {Key} (deserialized)", key);
                        return deserializedValue;
                    }
                }

                _logger.LogDebug("Cache miss for key: {Key}", key);
                return await Task.FromResult<T?>(null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cache value for key: {Key}", key);
                return null;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
        {
            try
            {
                var options = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(30),
                    Priority = CacheItemPriority.Normal
                };

                options.RegisterPostEvictionCallback((evictedKey, evictedValue, reason, state) =>
                {
                    _logger.LogDebug("Cache evicted: {Key}, Reason: {Reason}", evictedKey, reason);
                    _cacheTimestamps.Remove(evictedKey?.ToString() ?? "");
                });

                _memoryCache.Set(key, value, options);
                _cacheTimestamps[key] = DateTime.UtcNow;

                _logger.LogDebug("Cache set for key: {Key}, Expiration: {Expiration}", key, expiration);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cache value for key: {Key}", key);
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                _memoryCache.Remove(key);
                _cacheTimestamps.Remove(key);
                _logger.LogDebug("Cache removed for key: {Key}", key);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache value for key: {Key}", key);
            }
        }

        public async Task RemoveByPatternAsync(string pattern)
        {
            try
            {
                var keysToRemove = _cacheTimestamps.Keys
                    .Where(key => key.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                foreach (var key in keysToRemove)
                {
                    _memoryCache.Remove(key);
                    _cacheTimestamps.Remove(key);
                }

                _logger.LogDebug("Cache removed for pattern: {Pattern}, Count: {Count}", pattern, keysToRemove.Count);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache values for pattern: {Pattern}", pattern);
            }
        }

        public async Task<bool> ExistsAsync(string key)
        {
            try
            {
                var exists = _memoryCache.TryGetValue(key, out _);
                _logger.LogDebug("Cache exists check for key: {Key}, Result: {Exists}", key, exists);
                return await Task.FromResult(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking cache existence for key: {Key}", key);
                return false;
            }
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null) where T : class
        {
            try
            {
                var cachedValue = await GetAsync<T>(key);
                if (cachedValue != null)
                {
                    return cachedValue;
                }

                var newValue = await factory();
                if (newValue != null)
                {
                    await SetAsync(key, newValue, expiration);
                }

                return newValue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetOrSet for key: {Key}", key);
                return await factory();
            }
        }

        public async Task<IEnumerable<string>> GetKeysAsync(string pattern)
        {
            try
            {
                var keys = _cacheTimestamps.Keys
                    .Where(key => string.IsNullOrEmpty(pattern) || key.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                _logger.LogDebug("Retrieved {Count} keys for pattern: {Pattern}", keys.Count, pattern);
                return await Task.FromResult(keys);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting keys for pattern: {Pattern}", pattern);
                return Enumerable.Empty<string>();
            }
        }

        public async Task ClearAllAsync()
        {
            try
            {
                if (_memoryCache is MemoryCache memoryCache)
                {
                    memoryCache.Compact(1.0);
                }

                _cacheTimestamps.Clear();
                _logger.LogInformation("All cache cleared");
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing all cache");
            }
        }

        public async Task<long> GetMemoryUsageAsync()
        {
            try
            {
                // Bu metod gerçek memory kullanımını hesaplamak için daha gelişmiş bir implementasyon gerektirir
                // Şimdilik cache entry sayısını döndürüyoruz
                var memoryUsage = _cacheTimestamps.Count * 1024; // Tahmini değer
                return await Task.FromResult(memoryUsage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting memory usage");
                return 0;
            }
        }

        public async Task<Dictionary<string, object>> GetCacheStatisticsAsync()
        {
            try
            {
                var statistics = new Dictionary<string, object>
                {
                    ["TotalEntries"] = _cacheTimestamps.Count,
                    ["OldestEntry"] = _cacheTimestamps.Values.Any() ? _cacheTimestamps.Values.Min() : (DateTime?)null,
                    ["NewestEntry"] = _cacheTimestamps.Values.Any() ? _cacheTimestamps.Values.Max() : (DateTime?)null,
                    ["MemoryUsage"] = await GetMemoryUsageAsync()
                };

                return await Task.FromResult(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cache statistics");
                return new Dictionary<string, object>();
            }
        }
    }
}
