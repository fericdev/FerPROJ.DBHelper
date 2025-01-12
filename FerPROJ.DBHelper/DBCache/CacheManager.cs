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

        public async static Task SaveToCacheAsync<TModel>(TModel value) where TModel : BaseDTO {
            string key = nameof(TModel);
            await Task.Run(() => _cache.Set(key, value, DateTimeOffset.MaxValue));
        }
        public async static Task<TModel> GetCacheAsync<TModel>(Func<TModel, bool> whereCondition) where TModel : BaseDTO {            
            var values = await GetAllCacheAsync<TModel>();
            return values.FirstOrDefault(whereCondition);
        }

        public async static Task SaveAllToCacheAsync<TModel>(List<TModel> values) where TModel : BaseDTO {
            var tasks = values.Select(value => SaveToCacheAsync(value));
            await Task.WhenAll(tasks);
        }

        public async static Task<List<TModel>> GetAllCacheAsync<TModel>() where TModel : BaseDTO {
            string key = nameof(TModel);
            return await Task.Run(() => _cache.Get(key) as List<TModel>);
        }

    }
}
