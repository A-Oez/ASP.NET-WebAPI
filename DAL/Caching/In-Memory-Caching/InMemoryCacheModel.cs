namespace DAL.Caching
{
    public static class InMemoryCacheModel
    {
        //optional
        private static IMemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());

        public static void Add(string cacheKey, int value)
        {
            var cacheExpiryOptions = new MemoryCacheEntryOptions()
            {
                AbsoluteExpiration = DateTime.Now.AddSeconds(50),
                Priority = CacheItemPriority.High,
                SlidingExpiration = TimeSpan.FromSeconds(20),
            };
            _memoryCache.Set(cacheKey, value, cacheExpiryOptions);
        }

        public static int Get(string cacheKey)
        {
            var result = _memoryCache.Get(cacheKey);

            return Convert.ToInt32(result);
        }

        public static void Remove(string cacheKey, int value)
        {
            _memoryCache.Remove(cacheKey);
        }
    }
}
