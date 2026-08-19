using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMWebAPIBusinessLogic.Interfaces.Core;
namespace FMWebAPIBusinessLogic.Services.Core
{
    using System.Runtime.Caching;

    using FMCore.Interfaces;

    public class CacheService : ICacheService
    {
        private readonly IFMCustomLogger _logger;
        public CacheService(IFMCustomLogger logger)
        {
            this._logger = logger;
        }

        private static MemoryCache _internalCache;
        private static object _lock = new object();

        public T Get<T>(string key) where T : class
        {
            try
            {
                this.Setup();
                var internalKey = this.GetInteralKey<T>(key);
                return _internalCache.Get(internalKey) as T;
            }
            catch (Exception ex)
            {
                this._logger.Fatal(ex, "Getting key failed for CacheService");
                throw;
            }
        }

        public void Add<T>(string key, T toSave, TimeSpan? timeout = null) where T : class
        {
            try
            {
                this.Setup();
                var internalKey = this.GetInteralKey<T>(key);

                if (timeout == null)
                {
                    timeout = new TimeSpan(0, 2, 0);
                }

                //SlidingExpiration has a max value of a year
                if (timeout > TimeSpan.FromDays(365))
                {
                    timeout = TimeSpan.FromDays(365);
                }
                var expires = DateTime.UtcNow.Add(timeout.Value);
                this._logger.Verbose("Adding the following key and value: {@key} {@value}", key, toSave);
                _internalCache.Add(
                    new CacheItem(internalKey, toSave),
                    new CacheItemPolicy()
                    {
                        AbsoluteExpiration = expires
                    });
            }
            catch (Exception e)
            {
                this._logger.Fatal(e, "Adding entry to cache failed");
                throw;
            }
        }

        private string GetInteralKey<T>(string key)
        {
            var internalKey = typeof(T).FullName + ":" + key;
            return internalKey;
        }

        /// <summary>
        /// Will initalize the internal cache by locking if necessary
        /// </summary>
        private void Setup()
        {
            if (_internalCache == null)
            {
                lock (_lock)
                {
                    if (_internalCache == null)
                    {
                        this._logger.Verbose("Cache was setup");
                        _internalCache = new MemoryCache("CacheService");
                    }
                }
            }
        }

        public void Clear()
        {
            //https://social.msdn.microsoft.com/Forums/vstudio/en-US/0295b899-c550-48c1-bd5d-841c45ec3c57/memorycache-clear-all?forum=csharpgeneral
            List<string> cacheKeys = _internalCache.Select(kvp => kvp.Key).ToList();
            foreach (string cacheKey in cacheKeys)
            {
                _internalCache.Remove(cacheKey);
            }

            this._logger.Verbose("Cache was cleared");
        }
    }
}
