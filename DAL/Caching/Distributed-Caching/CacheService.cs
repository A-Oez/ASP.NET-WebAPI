namespace DAL.Caching
{
    public class CacheService : ICacheService
    {
        private IDatabase _cacheDB;
        private readonly IConfiguration _configuration;

        public CacheService(IConfiguration configuration)
        {
            _configuration = configuration;

            try
            {
                var redis = ConnectionMultiplexer.Connect(_configuration.GetSection("RedisPort:Port").Value);
                _cacheDB = redis.GetDatabase();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Environment.Exit(1);
            }
        }

        public T GetData<T>(string key)
        {
            var value = _cacheDB.StringGet(key);
            if(!string.IsNullOrEmpty(value))
                return JsonSerializer.Deserialize<T>(value);
             
            return default;
        }

        public object RemoveData(string key)
        {
            var _exist = _cacheDB.KeyExists(key);

            if(_exist)
                return _cacheDB.KeyDelete(key);

            return false;
        }

        public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            var expirtyTime = expirationTime.DateTime.Subtract(DateTime.Now);
            return _cacheDB.StringSet(key, JsonSerializer.Serialize(value), expirtyTime);
        }
    }
}
