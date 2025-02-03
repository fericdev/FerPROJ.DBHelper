using FerPROJ.DBHelper.Base;
using FerPROJ.DBHelper.DBExtensions;
using FerPROJ.Design.BaseDTO;
using FerPROJ.Design.Class;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace FerPROJ.DBHelper.DBCache {
    public static class CacheManager {

        private static MemoryCache _cache = MemoryCache.Default;

        #region Save
        public async static Task SaveToCacheAsync<TEntity>(this DbContext dbContext, TEntity value) where TEntity : class {
            if (value == null) {
                return;
            }

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
                // Remove the existing value after identifying it
                if (existingValue != null) {
                    existingList = existingList.Where(x => !primaryKey.GetValue(x).Equals(primaryValue)).ToList();
                }
            }

            // Now you can safely add the new value
            existingList.Add(value);

            // Save the updated list to the cache
            _cache.Set(key, existingList, DateTimeOffset.MaxValue);

            Console.WriteLine($"Cache Cleared and Saved: {key} TIME: {DateTime.Now.TimeOfDay} Count: {existingList.Count}");
        }
        public async static Task SaveAllToCacheAsync<TEntity>(this DbContext dbContext, List<TEntity> values) where TEntity : class {

            if (values.Count <= 0) {
                return;
            }

            string key = typeof(TEntity).Name;

            // Get the current cached list of TEntity
            var existingList = await GetAllListCacheAsync<TEntity>();

            // If there's no existing list, create a new one
            if (existingList == null) {
                existingList = new List<TEntity>();
            }
            else {

                var primaryKey = dbContext.GetPrimaryKeyOfDbContext<TEntity>();

                if (primaryKey == null) {
                    return;
                }

                // Prepare a list to store items to remove
                var itemsToRemove = new List<TEntity>();

                // Identify the items to remove without modifying the collection during iteration
                foreach (var value in values) {

                    var primaryValue = primaryKey.GetValue(value);

                    var existingValue = existingList.FirstOrDefault(x => primaryKey.GetValue(x).Equals(primaryValue));

                    if (existingValue != null) {

                        itemsToRemove.Add(existingValue);

                    }
                }

                // Remove identified items after the iteration
                foreach (var item in itemsToRemove) {

                    existingList.Remove(item);

                }
            }

            // Add the new values to the existing list
            existingList.AddRange(values);

            // Save the updated list to the cache
            _cache.Set(key, existingList, DateTimeOffset.MaxValue);

            Console.WriteLine($"Cache Cleared and Saved: {key} TIME: {DateTime.Now.TimeOfDay} Count: {existingList.Count}");
        }

        public async static Task SaveAllToCacheAsync<TEntity>(this DbContext dbContext, IEnumerable<TEntity> values) where TEntity : class {

            if (values == null || !values.Any()) {

                return;

            }

            await dbContext.SaveAllToCacheAsync(values.ToList());
        }

        public async static Task SaveAllToCacheAsync<TEntity>(this DbContext dbContext, ICollection<TEntity> values) where TEntity : class {

            if (values == null || !values.Any()) {

                return;

            }

            await dbContext.SaveAllToCacheAsync(values.ToList());
        }

        #endregion

        #region Clear and Save
        public async static Task ClearAndSaveAllToCacheAsync<TEntity>(this DbContext dbContext, List<TEntity> values) where TEntity : class {
            //
            if (values.Count <= 0) {
                return;
            }
            //
            string key = typeof(TEntity).Name;

            // Clear the cache synchronously (fast operation)
            _cache.Remove(key);

            // Save the updated list to the cache synchronously
            _cache.Set(key, values, DateTimeOffset.MaxValue);

            Console.WriteLine($"Cache Cleared and Saved: {key} TIME: {DateTime.Now.TimeOfDay} Count: {values.Count}");

            await Task.CompletedTask;
        }

        public async static Task ClearAndSaveAllToCacheAsync<TEntity>(this DbContext dbContext, IEnumerable<TEntity> values) where TEntity : class {
            await dbContext.ClearAndSaveAllToCacheAsync(values.ToList());
        }

        public async static Task ClearAndSaveAllToCacheAsync<TEntity>(this DbContext dbContext, ICollection<TEntity> values) where TEntity : class {
            await dbContext.ClearAndSaveAllToCacheAsync(values.ToList());
        }

        #endregion

        #region Remove
        public async static Task RemoveFromCacheAsync<TEntity>(this DbContext dbContext, TEntity value) where TEntity : class {
            string key = typeof(TEntity).Name;

            // Get the current cached list of TEntity
            var existingList = await GetAllListCacheAsync<TEntity>();

            // If there's no existing list, there's nothing to remove
            if (existingList == null) {
                return;
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

            // Save the updated list to the cache
            await Task.Run(() => _cache.Set(key, existingList, DateTimeOffset.MaxValue));
        }

        public async static Task RemoveAllFromCacheAsync<TEntity>(this DbContext dbContext, List<TEntity> values) where TEntity : class {
            string key = typeof(TEntity).Name;

            // Get the current cached list of TEntity
            var existingList = await GetAllListCacheAsync<TEntity>();

            // If there's no existing list, return
            if (existingList == null) {
                return;
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

            // Save the updated list to the cache
            await Task.Run(() => _cache.Set(key, existingList, DateTimeOffset.MaxValue));
        }


        public async static Task RemoveAllFromCacheAsync<TEntity>(this DbContext dbContext, IEnumerable<TEntity> values) where TEntity : class {
            await dbContext.RemoveAllFromCacheAsync(values.ToList());
        }


        public async static Task RemoveAllFromCacheAsync<TEntity>(this DbContext dbContext, ICollection<TEntity> values) where TEntity : class {
            await dbContext.RemoveAllFromCacheAsync(values.ToList());
        }

        #endregion

        #region Get 
        public async static Task<List<TEntity>> GetAllListCacheAsync<TEntity>() where TEntity : class {
            string key = typeof(TEntity).Name;
            return await Task.FromResult(_cache.Get(key) as List<TEntity>);
        }
        public async static Task<IEnumerable<TEntity>> GetAllEnumerableCacheAsync<TEntity>() where TEntity : class {
            var result = await GetAllListCacheAsync<TEntity>();
            return result?.AsEnumerable();
        }
        public async static Task<IQueryable<TEntity>> GetAllQueryableCacheAsync<TEntity>() where TEntity : class {
            var result = await GetAllListCacheAsync<TEntity>();
            return result?.AsQueryable();
        }
        #endregion

        #region Load all Cached From DB
        public static List<Func<Task>> GetCacheLoadTasks(Type dbContextType) {
            var baseGenericType = typeof(BaseDBEntityAsync<,,,>);

            // List of assemblies to search, including current and referenced assemblies
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic)  // Ignore dynamic assemblies
                .ToList();

            // Get the derived types (that implement BaseDBEntityAsync)
            var derivedTypes = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => !t.IsAbstract && IsSubclassOfRawGeneric(baseGenericType, t))
                .ToList();

            var tasks = new List<Func<Task>>();

            foreach (var type in derivedTypes) {
                var method = type.GetMethod("LoadCachedAsync");
                if (method != null) {
                    // Create a new instance of DbContext dynamically inside each task
                    tasks.Add(async () => {
                        using (var freshDbContext = (DbContext)Activator.CreateInstance(dbContextType)) {
                            var instance = Activator.CreateInstance(type, freshDbContext);
                            await (Task)method.Invoke(instance, null);
                        }
                    });
                }
            }

            return tasks;
        }

        // Helper to check generic inheritance
        private static bool IsSubclassOfRawGeneric(Type generic, Type toCheck) {
            while (toCheck != null && toCheck != typeof(object)) {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur) {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }
        #endregion

    }
}
