using FerPROJ.DBHelper.DBCrud;
using FerPROJ.DBHelper.DBExtensions;
using FerPROJ.Design.BaseModels;
using FerPROJ.Design.Class;
using FerPROJ.Design.Forms;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace FerPROJ.DBHelper.DBCache {
    public static class CacheManager {

        private static MemoryCache _cache = MemoryCache.Default;
        private static readonly ConcurrentDictionary<string, (DateTime LastUpdate, TimeSpan LastDuration)> _updateTracker = new ConcurrentDictionary<string, (DateTime LastUpdate, TimeSpan LastDuration)>();

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
        public async static Task SaveAllToCacheAsync<TEntity>(
            this DbContext dbContext,
            List<TEntity> values) where TEntity : class {
            if (values.Count == 0)
                return;

            string key = typeof(TEntity).Name;

            var existingList = await GetAllListCacheAsync<TEntity>() ?? new List<TEntity>();

            PropertyInfo primaryKey;

            if (dbContext == null)
                primaryKey = typeof(TEntity).GetProperty("Id");
            else
                primaryKey = dbContext.GetPrimaryKeyOfDbContext<TEntity>();

            if (primaryKey == null)
                return;

            // Build lookup of new keys
            var newKeySet = new HashSet<object>(
                values.Select(v => primaryKey.GetValue(v))
            );

            // Remove items no longer present
            existingList.RemoveAll(x =>
            {
                var keyValue = primaryKey.GetValue(x);
                return !newKeySet.Contains(keyValue);
            });

            // Build dictionary for fast lookup
            var existingDict = existingList.ToDictionary(
                x => primaryKey.GetValue(x),
                x => x
            );

            foreach (var value in values) {
                var pk = primaryKey.GetValue(value);

                if (existingDict.ContainsKey(pk)) {
                    // Replace existing
                    var index = existingList.IndexOf(existingDict[pk]);
                    existingList[index] = value;
                }
                else {
                    // Add new
                    existingList.Add(value);
                }
            }

            _cache.Set(key, existingList, DateTimeOffset.MaxValue);

            Console.WriteLine($"Cache Synced: {key} TIME: {DateTime.Now.TimeOfDay} Count: {existingList.Count}");
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
            _cache.Set(key, existingList, DateTimeOffset.MaxValue);

            await Task.CompletedTask;
        }

        public async static Task RemoveAllFromCacheAsync<TEntity>(this DbContext dbContext, List<TEntity> values) where TEntity : class {

            if (values.Count <= 0) {
                return;
            }

            string key = typeof(TEntity).Name;

            // Get the current cached list of TEntity
            var existingList = await GetAllListCacheAsync<TEntity>();

            // If there's no existing list, return
            if (existingList == null) {
                return;
            }
            else {
                foreach (var value in values) {
                    existingList.Remove(value);
                }
            }

            // Save the updated list to the cache
            _cache.Set(key, existingList, DateTimeOffset.MaxValue);

            await Task.CompletedTask;
        }


        public async static Task RemoveAllFromCacheAsync<TEntity>(this DbContext dbContext, IEnumerable<TEntity> values) where TEntity : class {
            await dbContext.RemoveAllFromCacheAsync(values.ToList());
        }


        public async static Task RemoveAllFromCacheAsync<TEntity>(this DbContext dbContext, ICollection<TEntity> values) where TEntity : class {
            await dbContext.RemoveAllFromCacheAsync(values.ToList());
        }

        #endregion

        #region Remove by list ids
        public async static Task RemoveAllByIdsFromCacheAsync<TEntity>(this DbContext dbContext, List<object> values) where TEntity : class {

            if (values.Count <= 0) {
                return;
            }

            string key = typeof(TEntity).Name;

            // Get the current cached list of TEntity
            var existingList = await GetAllListCacheAsync<TEntity>();

            // If there's no existing list, return
            if (existingList == null) {
                return;
            }

            var primaryKey = dbContext.GetPrimaryKeyOfDbContext<TEntity>();

            if (primaryKey == null) {
                return;
            }

            // Prepare a list to store items to remove
            var itemsToRemove = new List<TEntity>();

            // Loop through each object value in the values list
            foreach (var value in values) {

                var existingValue = existingList.FirstOrDefault(x => primaryKey.GetValue(x).Equals(value));

                if (existingValue != null) {

                    itemsToRemove.Add(existingValue);

                }

            }

            // Remove identified items after the iteration
            foreach (var item in itemsToRemove) {

                existingList.Remove(item);

            }

            // Save the updated list to the cache
            _cache.Set(key, existingList, DateTimeOffset.MaxValue);

            Console.WriteLine($"Updated: {key} TIME: {DateTime.Now.TimeOfDay} Count: {existingList.Count}");
        }

        // Overload for IEnumerable
        public async static Task RemoveAllFromCacheAsync<TEntity>(this DbContext dbContext, IEnumerable<object> values) where TEntity : class {
            await dbContext.RemoveAllByIdsFromCacheAsync<TEntity>(values.ToList());
        }

        // Overload for ICollection
        public async static Task RemoveAllFromCacheAsync<TEntity>(this DbContext dbContext, ICollection<object> values) where TEntity : class {
            await dbContext.RemoveAllByIdsFromCacheAsync<TEntity>(values.ToList());
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

        #region Get or Create
        public async static Task<List<TEntity>> GetOrCreateListCacheAsync<TEntity>(IEnumerable<Func<Task<TEntity>>> values) where TEntity : class {
            
            var key = typeof(TEntity).Name;

            if (ShouldUpdate(key)) {

                await FrmSplasherLoading.ShowSplashAsync();

                var factories = values.ToList();

                var sw = Stopwatch.StartNew();

                var tasks = factories.Select(f => f());

                var results = await Task.WhenAll(tasks);

                FrmSplasherLoading.CloseSplash();

                sw.Stop();

                await SaveAllToCacheAsync(null, results);

                RecordUpdate(key, sw.Elapsed);

                return results.ToList();
            }

            return await GetAllListCacheAsync<TEntity>();
        }
        private static bool ShouldUpdate(string key) {
            var now = DateTime.Now;

            if (!_updateTracker.TryGetValue(key, out var info)) {
                // First time: allow update
                _updateTracker[key] = (now, TimeSpan.FromSeconds(5));
                return true;
            }

            if (info.LastUpdate + info.LastDuration <= now) {
                return true; // enough time passed
            }

            return false; // still in cooldown
        }
        private static void RecordUpdate(string key, TimeSpan duration) {
            // Add a small buffer to the duration to prevent immediate re-updates
            duration = duration.Add(TimeSpan.FromSeconds(5));

            // Update the tracker with the current time and the duration of the update
            _updateTracker[key] = (DateTime.Now, duration);
        }
        #endregion

        #region Load all Cached From DB
        public static List<Func<Task>> GetCacheLoadTasks(params string[] assembliesToLoad) {
            var baseGenericType = typeof(BaseRepository<,,,>);

            var allAssemblies = assembliesToLoad.ToList();
            allAssemblies.Add("FerPROJ.DBHelper");

            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            foreach (var file in allAssemblies) {
                // Support both file names (e.g., "LMS.Repository.dll") and short names ("LMS.Repository")
                var fileName = file.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) ? file : file + ".dll";
                var path = Path.Combine(baseDir, fileName);

                if (File.Exists(path)) {
                    var asmName = Path.GetFileNameWithoutExtension(fileName);
                    if (!AppDomain.CurrentDomain.GetAssemblies()
                        .Any(a => a.GetName().Name.Equals(asmName, StringComparison.OrdinalIgnoreCase))) {
                        Assembly.LoadFrom(path);
                    }
                }
                else {
                    Console.WriteLine($"⚠️ Assembly not found: {path}");
                }
            }

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
                        using (var freshDbContext = (DbContext)Activator.CreateInstance(CAppConstants.DB_CONTEXT_TYPE)) {
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
