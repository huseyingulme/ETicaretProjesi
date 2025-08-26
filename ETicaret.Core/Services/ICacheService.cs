namespace ETicaret.Core.Services
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key) where T : class;
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class;
        Task RemoveAsync(string key);
        Task RemoveByPatternAsync(string pattern);
        Task<bool> ExistsAsync(string key);
        Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null) where T : class;
        Task<IEnumerable<string>> GetKeysAsync(string pattern);
        Task ClearAllAsync();
        Task<long> GetMemoryUsageAsync();
        Task<Dictionary<string, object>> GetCacheStatisticsAsync();
    }
}
