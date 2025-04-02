namespace Services.Interface
{
    public interface ICacheService
    {
        Task SetCacheResponseAsync(string key, object reponse, TimeSpan timeOut);
        Task<string?> GetCacheResponseAsync(string key);
        Task RemoveCacheResponseAsync(string pattern);
    }
}
