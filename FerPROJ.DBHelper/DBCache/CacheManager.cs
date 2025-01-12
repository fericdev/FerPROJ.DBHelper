using FerPROJ.Design.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace FerPROJ.DBHelper.DBCache {
    public class CacheManager {

        private static MemoryCache _cache = MemoryCache.Default;

        public async static Task SaveToCacheAsync<TEntity>(TEntity value) where TEntity : class {
            string key = nameof(TEntity);
            await Task.Run(() => _cache.Set(key, value, DateTimeOffset.MaxValue));
        }
        public async static Task<TEntity> GetCacheAsync<TEntity>(Func<TEntity, bool> whereCondition) where TEntity : class {            
            var values = await GetAllListCacheAsync<TEntity>();
            return values.FirstOrDefault(whereCondition);
        }

        public async static Task SaveAllToCacheAsync<TEntity>(List<TEntity> values) where TEntity : class {
            var tasks = values.Select(value => SaveToCacheAsync(value));
            await Task.WhenAll(tasks);
        }
        public async static Task SaveAllToCacheAsync<TEntity>(IEnumerable<TEntity> values) where TEntity : class {
            var tasks = values.Select(value => SaveToCacheAsync(value));
            await Task.WhenAll(tasks);
        }

        public async static Task<List<TEntity>> GetAllListCacheAsync<TEntity>() where TEntity : class {
            string key = nameof(TEntity);
            return await Task.Run(() => _cache.Get(key) as List<TEntity>);
        }
        public async static Task<IEnumerable<TEntity>> GetAllEnumerableCacheAsync<TEntity>() where TEntity : class {
            string key = nameof(TEntity);
            return await Task.Run(() => _cache.Get(key) as IEnumerable<TEntity>);
        }

    }
}
