using FerPROJ.DBHelper.DBCache;
using FerPROJ.Design.Class;
using FerPROJ.Design.BaseDTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FerPROJ.DBHelper.DBExtensions {
    public static class DBTransactionExtensions {

        #region properties
        public static bool AllowDuplicate {  get; set; } 
        public static string PropertyToCheck { get; set; }
        #endregion

        #region Remove
        public static async Task RemoveRangeAndCommitAsync<TEntity>(this DbContext context, ICollection<TEntity> entity) where TEntity : class {
            if (entity.Count() <= 0) {
                return;
            }
            context.Set<TEntity>().RemoveRange(entity);
            await context.SaveChangesAsync();
            await context.RemoveAllFromCacheAsync(entity);
        }
        public static async Task RemoveRangeAndCommitAsync<TEntity>(this DbContext context, IEnumerable<TEntity> entity) where TEntity : class {
            if (entity.Count() <= 0) {
                return;
            }
            context.Set<TEntity>().RemoveRange(entity);
            await context.SaveChangesAsync();
            await context.RemoveAllFromCacheAsync(entity);
        }
        public static async Task RemoveRangeAsync<TEntity>(this DbContext context, ICollection<TEntity> entity) where TEntity : class {
            if (entity.Count() <= 0) {
                return;
            }
            context.Set<TEntity>().RemoveRange(entity);
            await context.RemoveFromCacheAsync(entity);
        }
        public static async Task RemoveRangeAsync<TEntity>(this DbContext context, IEnumerable<TEntity> entity) where TEntity : class {
            if (entity.Count() <= 0) {
                return;
            }
            context.Set<TEntity>().RemoveRange(entity);
            await context.RemoveAllFromCacheAsync(entity);
        }
        public static async Task RemoveAndCommitAsync<TEntity>(this DbContext context, TEntity entity) where TEntity : class {
            
            context.Set<TEntity>().Remove(entity);
            await context.SaveChangesAsync();
            await context.RemoveFromCacheAsync(entity);
        }
        public static async Task RemoveAsync<TEntity>(this DbContext context, TEntity entity) where TEntity : class {
           
            context.Set<TEntity>().Remove(entity);
            await context.RemoveFromCacheAsync(entity);

        }
        #endregion

        #region Update
        public static async Task UpdateAndCommitAsync<TEntity, TRelatedEntity>(
            this DbContext context,
            TEntity entity,
            ICollection<TRelatedEntity> relatedItems,
            Func<TEntity, ICollection<TRelatedEntity>> getRelatedEntities,
            Action<TRelatedEntity, string> setForeignKeyForRelatedEntity,
            string foreignKeyValue)
            where TEntity : class
            where TRelatedEntity : class {
            // Start by ensuring the main entity is tracked by the context
            context.Set<TEntity>().AddOrUpdate(entity);

            // Retrieve the related entities for comparison
            var existingRelatedEntities = getRelatedEntities(entity);

            // Remove any related entities that are not in the incoming list
            var toRemove = existingRelatedEntities
                .Where(existing => !relatedItems.Any(item => item.Equals(existing)))
                .ToList();

            if (toRemove.Any()) {
                await context.RemoveRangeAsync(toRemove);
            }

            // Set foreign key for each related entity
            foreach (var relatedItem in relatedItems) {
                setForeignKeyForRelatedEntity(relatedItem, foreignKeyValue);
                await context.UpdateAsync(relatedItem);
            }

            // Commit all changes
            await context.SaveChangesAsync();
        }
        public static async Task UpdateAsync<TEntity>(
             this DbContext context,
             TEntity entity)
             where TEntity : class {

            context.Set<TEntity>().AddOrUpdate(entity);

            await Task.CompletedTask;

            await context.SaveToCacheAsync(entity);
        }
        public static async Task UpdateRangeWithForeignKeyAsync<TEntity>(
            this DbContext context,
            List<TEntity> entities,
            string foreignKey,
            object foreignKeyValue)
            where TEntity : class {
            // Get the property info for the specified foreign key
            var foreignKeyProperty = typeof(TEntity).GetProperty(foreignKey, BindingFlags.Public | BindingFlags.Instance);

            if (foreignKeyProperty == null) {
                throw new ArgumentException($"The property '{foreignKey}' does not exist on type '{typeof(TEntity).Name}'.");
            }

            // Ensure the foreign key property can be set and its type is compatible with the provided value
            if (!foreignKeyProperty.CanWrite) {
                throw new InvalidOperationException($"The property '{foreignKey}' is read-only.");
            }

            // Retreive all to be deleted
            var entitiesToBeDeleted = await context.GetAllAsync<TEntity>(foreignKey, foreignKeyValue);
            await context.RemoveRangeAsync(entitiesToBeDeleted);

            // Iterate through each entity in the list
            foreach (var entity in entities) {
                // Set the foreign key property value
                foreignKeyProperty.SetValue(entity, foreignKeyValue);

                // Add or update the entity
                await context.SaveAsync(entity);
            }

        }
        public static async Task UpdateRangeAndCommitWithForeignKeyAsync<TEntity>(
            this DbContext context,
            List<TEntity> entities,
            string foreignKey,
            object foreignKeyValue)
            where TEntity : class {
            // Get the property info for the specified foreign key
            var foreignKeyProperty = typeof(TEntity).GetProperty(foreignKey, BindingFlags.Public | BindingFlags.Instance);

            if (foreignKeyProperty == null) {
                throw new ArgumentException($"The property '{foreignKey}' does not exist on type '{typeof(TEntity).Name}'.");
            }

            // Ensure the foreign key property can be set and its type is compatible with the provided value
            if (!foreignKeyProperty.CanWrite) {
                throw new InvalidOperationException($"The property '{foreignKey}' is read-only.");
            }

            // Retreive all to be deleted
            var entitiesToBeDeleted = await context.GetAllAsync<TEntity>(foreignKey, foreignKeyValue);
            await context.RemoveRangeAsync(entitiesToBeDeleted);

            // Iterate through each entity in the list
            foreach (var entity in entities) {
                // Set the foreign key property value
                foreignKeyProperty.SetValue(entity, foreignKeyValue);

                // Add or update the entity
                await context.SaveAsync(entity);
            }

            //
            await context.SaveChangesAsync();

        }
        public static async Task UpdateRangeAsync<TEntity>(
             this DbContext context,
             List<TEntity> entity)
             where TEntity : class {
            foreach (var item in entity) {
                await context.UpdateAsync(item);
            }
        }
        public static async Task UpdateAndCommitAsync<TEntity>(
             this DbContext context,
             TEntity entity)
             where TEntity : class {

            await context.UpdateAsync(entity);

            await context.SaveChangesAsync();
        }
        public static async Task UpdateRangeAndCommitAsync<TEntity>(
             this DbContext context,
             List<TEntity> entity)
             where TEntity : class {

            foreach (var item in entity) {
                await context.UpdateAsync(item);
            }
            await context.SaveChangesAsync();
        }
        #endregion

        #region Save
        public static async Task SaveAsync<TEntity>(
             this DbContext context,
             TEntity entity)
             where TEntity : class {

            context.Set<TEntity>().Add(entity);
            await Task.CompletedTask;
            await context.SaveToCacheAsync(entity);
        }
        public static async Task SaveRangeAsync<TEntity>(
             this DbContext context,
             ICollection<TEntity> entity)
             where TEntity : class {

            context.Set<TEntity>().AddRange(entity);
            await context.SaveAllToCacheAsync(entity);

        }
        public static async Task SaveAndCommitAsync<TEntity>(
             this DbContext context,
             TEntity entity)
             where TEntity : class {

            context.Set<TEntity>().Add(entity);
            await context.SaveToCacheAsync(entity);

            await context.SaveChangesAsync();
        }
        public static async Task SaveRangeAndCommitAsync<TEntity>(
             this DbContext context,
             List<TEntity> entity)
             where TEntity : class {

            context.Set<TEntity>().AddRange(entity);
            await context.SaveAllToCacheAsync(entity);

            await context.SaveChangesAsync();
        }
        // DTO
        public static async Task SaveDTOAsync<TSource, TEntity>(this DbContext context, TSource myDTO) where TSource : BaseDTO where TEntity : class {

            myDTO.DateCreated = DateTime.Now;
            myDTO.CreatedBy = CStaticVariable.USERNAME;
            myDTO.Status = CStaticVariable.ACTIVE_STATUS;

            var tbl = new CMapping<TSource, TEntity>().GetMappingResult(myDTO);

            await context.SaveAsync(tbl);

        }
        public static async Task SaveDTOAndCommitAsync<TSource, TEntity>(this DbContext context, TSource myDTO) where TSource : BaseDTO where TEntity : class {

            myDTO.DateCreated = DateTime.Now;
            myDTO.CreatedBy = CStaticVariable.USERNAME;
            myDTO.Status = CStaticVariable.ACTIVE_STATUS;

            var tbl = new CMapping<TSource, TEntity>().GetMappingResult(myDTO);

            if (!AllowDuplicate && !string.IsNullOrWhiteSpace(PropertyToCheck)) {
                // Get the property info for PropertyToCheck
                var propertyInfo = typeof(TEntity).GetProperty(PropertyToCheck);

                // Check if the property exists on TEntity
                if (propertyInfo == null) {
                    throw new ArgumentException($"Property '{PropertyToCheck}' does not exist on entity type '{typeof(TEntity).Name}'.");
                }

                // Build a dynamic expression to represent the equality comparison in LINQ
                // Example: e => e.PropertyToCheck == propertyValue

                // Parameter "e" in the expression
                var parameter = Expression.Parameter(typeof(TEntity), "e");

                // Access "e.PropertyToCheck"
                var propertyAccess = Expression.Property(parameter, propertyInfo);

                // Get the value of PropertyToCheck from tbl (the mapped entity)
                var propertyValue = propertyInfo.GetValue(tbl);

                // Create the expression "e.PropertyToCheck == propertyValue"
                var constant = Expression.Constant(propertyValue);
                var equality = Expression.Equal(propertyAccess, constant);

                // Build the complete lambda expression: e => e.PropertyToCheck == propertyValue
                var lambda = Expression.Lambda<Func<TEntity, bool>>(equality, parameter);

                // Use the expression in AnyAsync to check if an entity with the same property value exists
                var entityExists = await context.Set<TEntity>().AnyAsync(lambda);

                if (entityExists) {
                    throw new ArgumentException($"{PropertyToCheck} already exists.");
                }
            }

            await context.SaveAndCommitAsync(tbl);

        }
        public static async Task SaveRangeDTOAsync<TSource, TEntity>(this DbContext context, List<TSource> myDTO) where TSource : BaseDTO where TEntity : class {

            foreach (var item in myDTO) {
                item.DateCreated = DateTime.Now;
                item.CreatedBy = CStaticVariable.USERNAME;
                item.Status = CStaticVariable.ACTIVE_STATUS;
            }

            var tbl = new CMappingList<TSource, TEntity>().GetMappingResultList(myDTO);

            await context.SaveRangeAsync(tbl);

        }
        public static async Task SaveRangeDTOAndCommitAsync<TSource, TEntity>(this DbContext context, List<TSource> myDTO) where TSource : BaseDTO where TEntity : class {

            foreach (var item in myDTO) {
                item.DateCreated = DateTime.Now;
                item.CreatedBy = CStaticVariable.USERNAME;
                item.Status = CStaticVariable.ACTIVE_STATUS;
            }

            var tbl = new CMappingList<TSource, TEntity>().GetMappingResultList(myDTO);

            await context.SaveRangeAndCommitAsync(tbl);
        }
        #endregion

        #region Update DTO
        public static async Task UpdateDTOAsync<TSource, TEntity>(this DbContext context, TSource myDTO) where TSource : BaseDTO where TEntity : class {

            myDTO.DateModified = DateTime.Now;
            myDTO.ModifiedBy = CStaticVariable.USERNAME;

            var tbl = new CMapping<TSource, TEntity>().GetMappingResult(myDTO);

            context.Set<TEntity>().AddOrUpdate(tbl);
            await context.SaveToCacheAsync(tbl);
            await Task.CompletedTask;

        }
        public static async Task UpdateDTOAndCommitAsync<TSource, TEntity>(this DbContext context, TSource myDTO) where TSource : BaseDTO where TEntity : class {

            myDTO.DateModified = DateTime.Now;
            myDTO.ModifiedBy = CStaticVariable.USERNAME;

            var tbl = new CMapping<TSource, TEntity>().GetMappingResult(myDTO);

            context.Set<TEntity>().AddOrUpdate(tbl);
            await context.SaveToCacheAsync(tbl);

            await context.SaveChangesAsync();
        }
        public static async Task UpdateRangeDTOAsync<TSource, TEntity>(this DbContext context, List<TSource> myDTO) where TSource : BaseDTO where TEntity : class {

            foreach (var item in myDTO) {
                item.DateModified = DateTime.Now;
                item.ModifiedBy = CStaticVariable.USERNAME;
            }

            var tbl = new CMappingList<TSource, TEntity>().GetMappingResultList(myDTO);

            foreach (var item in tbl) {
                await context.UpdateAsync(item);
                await context.SaveToCacheAsync(item);
            }

        }
        public static async Task UpdateRangeDTOAndCommitAsync<TSource, TEntity>(this DbContext context, List<TSource> myDTO) where TSource : BaseDTO where TEntity : class {

            foreach (var item in myDTO) {
                item.DateModified = DateTime.Now;
                item.ModifiedBy = CStaticVariable.USERNAME;
            }

            var tbl = new CMappingList<TSource, TEntity>().GetMappingResultList(myDTO);

            foreach (var item in tbl) {
                await context.UpdateAndCommitAsync(item);
                await context.SaveToCacheAsync(item);
            }

        }
        #endregion

        #region Update Status
        public static async Task SetStatusInActiveAsync<TEntity>(this DbContext context, string id) where TEntity : class {
         
            // Retrieve the entity by primary key
            var entity = await context.GetByIdAsync<TEntity, string>(id);

            // If the entity was not found, return
            if (entity == null) {
                return;
            }

            // Find the Status property and set it to inactive
            var statusProperty = typeof(TEntity).GetProperty("Status");
            if (statusProperty == null) {
                throw new InvalidOperationException("Status property not found on entity.");
            }

            statusProperty.SetValue(entity, CStaticVariable.IN_ACTIVE_STATUS);

            await context.UpdateAndCommitAsync(entity);
        }
        public static async Task SetStatusActiveAsync<TEntity>(this DbContext context, string id) where TEntity : class {
            // Get the DbSet for TEntity
            var dbSet = context.Set<TEntity>();

            // Locate the primary key property
            var keyProperty = typeof(TEntity).GetProperties()
                .FirstOrDefault(p => p.GetCustomAttributes<KeyAttribute>().Any());

            if (keyProperty == null) {
                throw new InvalidOperationException("Primary key not found for entity.");
            }

            // Convert the id to the appropriate type of the key property
            var keyType = keyProperty.PropertyType;
            var convertedId = Convert.ChangeType(id, keyType);

            // Retrieve the entity by primary key
            var entity = await dbSet.FindAsync(convertedId);

            // If the entity was not found, return
            if (entity == null) {
                return;
            }

            // Find the Status property and set it to inactive
            var statusProperty = typeof(TEntity).GetProperty("Status");
            if (statusProperty == null) {
                throw new InvalidOperationException("Status property not found on entity.");
            }

            statusProperty.SetValue(entity, CStaticVariable.ACTIVE_STATUS);

            await context.UpdateAndCommitAsync(entity);
        }
        #endregion

        #region Get Method
        public static async Task<TEntity> GetByIdAsync<TEntity, TType>(this DbContext context, TType id) where TEntity : class {
            
            // Get Primary Key
            PropertyInfo keyProperty = context.GetPrimaryKeyOfDbContext<TEntity>();

            // Create a parameter expression for the entity type (e.g., "e => e.Id == id")
            var parameter = Expression.Parameter(typeof(TEntity), "e");

            // Ensure type compatibility by converting `id` to the primary key's type
            var idConstant = Expression.Constant(Convert.ChangeType(id, keyProperty.PropertyType), keyProperty.PropertyType);

            // Create the equality expression (e.g., "e.Id == id")
            var predicate = Expression.Lambda<Func<TEntity, bool>>(
                Expression.Equal(
                    Expression.Property(parameter, keyProperty.Name),
                    idConstant
                ),
                parameter
            );

            return await context.GetByPredicateAsync(predicate);

        }
        public static async Task<TEntity> GetByIdAsync<TEntity, TType>(
            this DbContext context,
            TType id,
            string propertyName) where TEntity : class {

            // Find the specified property on TEntity
            var property = typeof(TEntity).GetProperty(propertyName);
            if (property == null) {
                throw new ArgumentException($"Property '{propertyName}' does not exist on entity {typeof(TEntity).Name}");
            }

            // Create a parameter expression for the entity type
            var parameter = Expression.Parameter(typeof(TEntity), "e");

            // Create the equality expression for the specified property
            var predicate = Expression.Lambda<Func<TEntity, bool>>(
                Expression.Equal(
                    Expression.Property(parameter, property),
                    Expression.Constant(id, typeof(TType))
                ),
                parameter
            );

            return await context.GetByPredicateAsync(predicate);

        }
        public static async Task<TEntity> GetByPredicateCachedAsync<TEntity>(this DbContext context, Expression<Func<TEntity, bool>> predicate) where TEntity : class {

            var cachedData = await CacheManager.GetAllQueryableCacheAsync<TEntity>();

            if (cachedData != null && cachedData.Any()) {

                var result = cachedData.FirstOrDefault(predicate);

                if (result != null) {
                    return result;
                }
            }

            return await context.Set<TEntity>().FirstOrDefaultAsync(predicate);

        }
        public static async Task<TEntity> GetByPredicateAsync<TEntity>(this DbContext context, Expression<Func<TEntity, bool>> predicate) where TEntity : class {
            
            return await context.Set<TEntity>().FirstOrDefaultAsync(predicate);

        }
        public static async Task<string> GetGeneratedIDAsync<TEntity>(
            this DbContext context, string prefix = null) where TEntity : class {

            if (prefix == null) {
                prefix = typeof(TEntity).Name.Substring(2, 3).ToUpper();
            }

            // Get the current count and increment by 1
            var count = await context.Set<TEntity>().CountAsync() + 1;

            // Return the new ID with the format "<prefix>-00<count>"
            return $"{prefix}-00{count}";

        }
        #endregion

        #region Get All Method
        //
        public static async Task<IEnumerable<TEntity>> GetAllAsync<TEntity>(this DbContext context, string propertyName, object propertyValue) where TEntity : class {

            Expression<Func<TEntity, bool>> whereCondition = null;

            if (!string.IsNullOrEmpty(propertyName)) {
                // Get the property info for the specified property name
                var property = typeof(TEntity).GetProperty(propertyName);

                if (property != null) {
                    // Create a parameter expression representing the entity (e.g., e => e.PropertyName)
                    var parameter = Expression.Parameter(typeof(TEntity), "e");

                    // Create an expression for accessing the property (e.PropertyName)
                    var propertyAccess = Expression.MakeMemberAccess(parameter, property);

                    // Create a constant expression for the value to compare against (propertyValue)
                    var constant = Expression.Constant(propertyValue);

                    // Create an equality expression (e.PropertyName == propertyValue)
                    var equality = Expression.Equal(propertyAccess, constant);

                    // Create a lambda expression representing the predicate (e => e.PropertyName == propertyValue)
                    whereCondition = Expression.Lambda<Func<TEntity, bool>>(equality, parameter);

                }
            }

            // If no property filter is provided or the property does not exist, return all entities
            return await context.GetAllAsync(whereCondition, false);
        }
        public static async Task<IEnumerable<TEntity>> GetAllWithSearchAsync<TEntity>(this DbContext context, string searchText, DateTime? dateFrom, DateTime? dateTo, bool isCached = true) where TEntity : class {

            var cachedData = await CacheManager.GetAllEnumerableCacheAsync<TEntity>();

            if (cachedData != null && cachedData.Any() && isCached) {

                var result = cachedData.SearchDateRange(dateFrom, dateTo);

                result = cachedData.SearchText(searchText);

                if (result != null && result.Any()) {

                    return result.ToList();

                }
            }

            var query = context.Set<TEntity>().AsEnumerable();

            query = query.SearchDateRange(dateFrom, dateTo);

            return await Task.FromResult(query.SearchText(searchText).ToList());
        }
        public static async Task<IEnumerable<TEntity>> GetAllAsync<TEntity>(this DbContext context, bool isCached = true) where TEntity : class {

            var cachedData = await CacheManager.GetAllQueryableCacheAsync<TEntity>();

            if (cachedData != null && cachedData.Any() && isCached) {

                return cachedData;

            }

            return await context.Set<TEntity>().ToListAsync();

        }
        public static async Task<IEnumerable<TEntity>> GetAllAsync<TEntity>(this DbContext context, Expression<Func<TEntity, bool>> whereCondition, bool isCached = true) where TEntity : class {

            var cachedData = await CacheManager.GetAllQueryableCacheAsync<TEntity>();

            if (cachedData != null && cachedData.Any() && isCached) {

                var result = cachedData.Where(whereCondition);

                if (result != null && result.Any()) {

                    return result.ToList();

                }

            }

            // Get the DbSet for TEntity
            var dbSet = context.Set<TEntity>();

            // Apply the where condition if provided
            var query = dbSet.AsQueryable();

            query = query.Where(whereCondition);

            // If no Status property exists, return all entities
            return await query.ToListAsync();
        }
        #endregion

        #region Get Primary Key
        public static PropertyInfo GetPrimaryKeyOfDbContext<TEntity>(this DbContext context) where TEntity : class {
            // Use ObjectContext to find the primary key property in EF6
            var objectContext = ((IObjectContextAdapter)context).ObjectContext;
            var entityType = objectContext.MetadataWorkspace
                .GetItems<System.Data.Entity.Core.Metadata.Edm.EntityType>(System.Data.Entity.Core.Metadata.Edm.DataSpace.CSpace)
                .FirstOrDefault(e => e.Name == typeof(TEntity).Name);

            if (entityType == null) {
                throw new InvalidOperationException($"Entity type {typeof(TEntity).Name} is not part of the model.");
            }

            // Get the primary key property name
            var keyPropertyName = entityType.KeyMembers.FirstOrDefault()?.Name;
            if (string.IsNullOrEmpty(keyPropertyName)) {
                throw new InvalidOperationException($"No primary key defined on entity {typeof(TEntity).Name}");
            }

            // Find the property in TEntity that matches the primary key name
            var keyProperty = typeof(TEntity).GetProperty(keyPropertyName);
            if (keyProperty == null) {
                throw new InvalidOperationException($"No primary key property named '{keyPropertyName}' found on entity {typeof(TEntity).Name}");
            }

            return keyProperty;
        }
        #endregion

    }
}
