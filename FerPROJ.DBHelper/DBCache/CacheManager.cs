using FerPROJ.DBHelper.DBExtensions;
using FerPROJ.Design.Class;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace FerPROJ.DBHelper.DBCache {
    public static class CacheManager {

        private static MemoryCache _cache = MemoryCache.Default;

        public async static Task SaveToCacheAsync<TEntity>(this DbContext dbContext, TEntity value) where TEntity : class {
            string key = typeof(TEntity).Name;

            // Get the current cached list of TEntity
            var existingList = await GetAllListCacheAsync<TEntity>();

            // If there's an existing list, add the new value to it
            if (existingList == null) {
                existingList = new List<TEntity>();
            }
            else {
                var primaryKey = dbContext.GetPrimaryKeyOfDbContext<TEntity>();
                if (primaryKey == null) {
                    return;
                }

                var primaryValue = primaryKey.GetValue(value);
                var existingValue = existingList.FirstOrDefault(x => primaryKey.GetValue(x).Equals(primaryValue) == true);              
                if (existingValue != null) {
                    existingList.Remove(existingValue);
                }

            }

            existingList.Add(value);

            // Save the updated list to the cache
            await Task.Run(() => _cache.Set(key, existingList, DateTimeOffset.MaxValue));
        }
        public async static Task SaveAllToCacheAsync<TEntity>(this DbContext dbContext, List<TEntity> values) where TEntity : class {
            string key = typeof(TEntity).Name;

            // Get the current cached list of TEntity
            var existingList = await GetAllListCacheAsync<TEntity>();

            // If there's no existing list, create a new one
            if (existingList == null) {
                existingList = new List<TEntity>();
            }
            else {
                // Remove all values from the list that are already in the new list
                foreach (var value in values) {

                    var primaryKey = dbContext.GetPrimaryKeyOfDbContext<TEntity>();
                    if (primaryKey == null) {
                        return;
                    }

                    var primaryValue = primaryKey.GetValue(value);

                    var existingValue = existingList.FirstOrDefault(x => primaryKey.GetValue(x).Equals(primaryValue) == true);
                    if (existingValue != null) {
                        existingList.Remove(existingValue);
                    }
                }
            }

            // Add the new values to the existing list
            existingList.AddRange(values);

            // Save the updated list to the cache
            await Task.Run(() => _cache.Set(key, existingList, DateTimeOffset.MaxValue));
        }

        public async static Task SaveAllToCacheAsync<TEntity>(this DbContext dbContext, IEnumerable<TEntity> values) where TEntity : class {
            await dbContext.SaveAllToCacheAsync(values.ToList());
        }

        public async static Task SaveAllToCacheAsync<TEntity>(this DbContext dbContext, ICollection<TEntity> values) where TEntity : class {
            await dbContext.SaveAllToCacheAsync(values.ToList());
        }
        public async static Task<List<TEntity>> GetAllListCacheAsync<TEntity>() where TEntity : class {
            string key = typeof(TEntity).Name;
            return await Task.Run(() => _cache.Get(key) as List<TEntity>);
        }
        public async static Task<IEnumerable<TEntity>> GetAllEnumerableCacheAsync<TEntity>() where TEntity : class {
            var result = await GetAllListCacheAsync<TEntity>();
            return result?.AsEnumerable();
        }
        public async static Task<IQueryable<TEntity>> GetAllQueryableCacheAsync<TEntity>() where TEntity : class {
            var result = await GetAllListCacheAsync<TEntity>();
            return result?.AsQueryable();
        }

    }
}
