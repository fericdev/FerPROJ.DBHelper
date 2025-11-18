using FerPROJ.DBHelper.DBCache;
using FerPROJ.Design.Class;
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
using System.Collections;
using FerPROJ.Design.BaseModels;
using FerPROJ.DBHelper.Entity;

namespace FerPROJ.DBHelper.DBExtensions {
    public static class DbContextExtensions {

        #region properties
        public static bool AllowDuplicate { get; set; }
        public static List<string> PropertiesToCheck { get; set; } = new List<string>();
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

            UpdateFieldsOfEntity(entity);

            context.Set<TEntity>().AddOrUpdate(entity);

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
        public static async Task SaveDTOAsync<TSource, TEntity>(this DbContext context, TSource myDTO) where TSource : BaseModel where TEntity : class {

            myDTO.DateCreated = DateTime.Now;
            myDTO.CreatedBy = CAppConstants.USERNAME;
            myDTO.Status = CAppConstants.ACTIVE_STATUS;
            myDTO.CreatedById = CAppConstants.USER_ID;

            var tbl = new CMappingExtension<TSource, TEntity>().GetMappingResult(myDTO);

            await context.SaveAsync(tbl);

        }
        public static async Task SaveDTOAndCommitAsync<TSource, TEntity>(this DbContext context, TSource myDTO) where TSource : BaseModel where TEntity : class {

            myDTO.DateCreated = DateTime.Now;
            myDTO.CreatedBy = CAppConstants.USERNAME;
            myDTO.Status = CAppConstants.ACTIVE_STATUS;
            myDTO.CreatedById = CAppConstants.USER_ID;

            var tbl = new CMappingExtension<TSource, TEntity>().GetMappingResult(myDTO);

            if (!AllowDuplicate && PropertiesToCheck.Any()) {

                var entityType = typeof(TEntity);

                // Validate that all properties exist
                foreach (var prop in PropertiesToCheck) {
                    if (!entityType.GetProperties().Any(c=>c.Name.SearchFor(prop))) {
                        throw new ArgumentException($"Property '{prop}' does not exist on entity type '{entityType.Name}'.");
                    }
                }

                // Parameter "e" in the expression
                var parameter = Expression.Parameter(entityType, "e");

                // Combine all equality checks with AND
                Expression finalExpression = null;

                foreach (var prop in PropertiesToCheck) {
                    var propertyInfo = entityType.GetProperties().FirstOrDefault(c=>c.Name.SearchFor(prop));
                    var propertyAccess = Expression.Property(parameter, propertyInfo);

                    var propertyValue = propertyInfo.GetValue(tbl);
                    var constant = Expression.Constant(propertyValue);

                    var equality = Expression.Equal(propertyAccess, constant);

                    finalExpression = finalExpression == null
                        ? equality
                        : Expression.AndAlso(finalExpression, equality);
                }

                // Build the complete lambda expression: e => e.Prop1 == value1 && e.Prop2 == value2
                var lambda = Expression.Lambda<Func<TEntity, bool>>(finalExpression, parameter);

                // Use the expression to check for duplicates
                var entityExists = await context.HasDataAsync(lambda);

                if (entityExists) {
                    throw new ArgumentException($"{string.Join(", ", PropertiesToCheck)} already exists.");
                }
            }

            await context.SaveAndCommitAsync(tbl);

        }
        public static async Task SaveModelAndCommitAsync<TModel, TModelItem, TEntity, TEntityItem>(this DbContext context, TModel model, List<TModelItem> modelItems) 
            where TModel : BaseModel 
            where TModelItem : BaseModelItem
            where TEntity: BaseEntity
            where TEntityItem : BaseEntityItem {

            // Set common properties
            model.DateCreated = DateTime.Now;
            model.CreatedBy = CAppConstants.USERNAME;
            model.Status = CAppConstants.ACTIVE_STATUS;
            model.CreatedById = CAppConstants.USER_ID;

            // Set common properties for each item
            foreach (var item in modelItems) {
                item.ParentId = model.Id;
            }

            // Map entity from model
            var entity = new CMappingExtension<TModel, TEntity>().GetMappingResult(model);

            // Map entity items from model items
            var entityItems = new CMappingExtensionList<TModelItem, TEntityItem>().GetMappingResultList(modelItems);

            // Save main entity
            await context.SaveAndCommitAsync(entity);

            // Save entity items
            await context.SaveRangeAndCommitAsync(entityItems);

        }
        public static async Task SaveRangeDTOAsync<TSource, TEntity>(this DbContext context, List<TSource> myDTO) where TSource : BaseModel where TEntity : class {

            foreach (var item in myDTO) {
                item.DateCreated = DateTime.Now;
                item.CreatedBy = CAppConstants.USERNAME;
                item.Status = CAppConstants.ACTIVE_STATUS;
                item.CreatedById = CAppConstants.USER_ID;
            }

            var tbl = new CMappingExtensionList<TSource, TEntity>().GetMappingResultList(myDTO);

            await context.SaveRangeAsync(tbl);

        }
        public static async Task SaveRangeDTOAndCommitAsync<TSource, TEntity>(this DbContext context, List<TSource> myDTO) where TSource : BaseModel where TEntity : class {

            foreach (var item in myDTO) {
                item.DateCreated = DateTime.Now;
                item.CreatedBy = CAppConstants.USERNAME;
                item.Status = CAppConstants.ACTIVE_STATUS;
                item.CreatedById = CAppConstants.USER_ID;
            }

            var tbl = new CMappingExtensionList<TSource, TEntity>().GetMappingResultList(myDTO);

            await context.SaveRangeAndCommitAsync(tbl);
        }
        #endregion

        #region Update DTO
        public static async Task UpdateDTOAsync<TSource, TEntity>(this DbContext context, TSource myDTO) where TSource : BaseModel where TEntity : class {

            myDTO.DateModified = DateTime.Now;
            myDTO.ModifiedBy = CAppConstants.USERNAME;
            myDTO.ModifiedById = CAppConstants.USER_ID;

            var tbl = new CMappingExtension<TSource, TEntity>().GetMappingResult(myDTO);

            context.Set<TEntity>().AddOrUpdate(tbl);

            await context.SaveToCacheAsync(tbl);


        }
        public static async Task UpdateDTOAndCommitAsync<TSource, TEntity>(this DbContext context, TSource myDTO) where TSource : BaseModel where TEntity : class {

            myDTO.DateModified = DateTime.Now;
            myDTO.ModifiedBy = CAppConstants.USERNAME;
            myDTO.ModifiedById = CAppConstants.USER_ID;

            var tbl = new CMappingExtension<TSource, TEntity>().GetMappingResult(myDTO);

            context.Set<TEntity>().AddOrUpdate(tbl);

            await context.SaveToCacheAsync(tbl);

            await context.SaveChangesAsync();
        }
        public static async Task UpdateModelAndCommitAsync<TModel, TModelItem, TEntity, TEntityItem>(this DbContext context, TModel model, List<TModelItem> modelItems)
            where TModel : BaseModel
            where TModelItem : BaseModelItem
            where TEntity : BaseEntity
            where TEntityItem : BaseEntityItem {

            // Set common properties
            model.DateModified = DateTime.Now;
            model.ModifiedBy = CAppConstants.USERNAME;
            model.ModifiedById = CAppConstants.USER_ID;

            // Set common properties for each item
            foreach (var item in modelItems) {
                item.ParentId = model.Id;
            }

            // Map entity from model
            var entity = new CMappingExtension<TModel, TEntity>().GetMappingResult(model);

            // Map entity items from model items
            var entityItems = new CMappingExtensionList<TModelItem, TEntityItem>().GetMappingResultList(modelItems);

            // Save main entity
            await context.UpdateAndCommitAsync(entity);

            // Save entity items
            await context.UpdateRangeAndCommitAsync(entityItems);

        }
        public static async Task UpdateRangeDTOAsync<TSource, TEntity>(this DbContext context, List<TSource> myDTO) where TSource : BaseModel where TEntity : class {

            foreach (var item in myDTO) {
                item.DateModified = DateTime.Now;
                item.ModifiedBy = CAppConstants.USERNAME;
                item.ModifiedById = CAppConstants.USER_ID;
            }

            var tbl = new CMappingExtensionList<TSource, TEntity>().GetMappingResultList(myDTO);

            foreach (var item in tbl) {
                await context.UpdateAsync(item);
                await context.SaveToCacheAsync(item);
            }

        }
        public static async Task UpdateRangeDTOAndCommitAsync<TSource, TEntity>(this DbContext context, List<TSource> myDTO) where TSource : BaseModel where TEntity : class {

            foreach (var item in myDTO) {
                item.DateModified = DateTime.Now;
                item.ModifiedBy = CAppConstants.USERNAME;
                item.ModifiedById = CAppConstants.USER_ID;
            }

            var tbl = new CMappingExtensionList<TSource, TEntity>().GetMappingResultList(myDTO);

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

            statusProperty.SetValue(entity, CAppConstants.IN_ACTIVE_STATUS);

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

            statusProperty.SetValue(entity, CAppConstants.ACTIVE_STATUS);

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
        public static async Task<TEntityItem> GetByParentIdAsync<TEntityItem>(this DbContext context, Guid parentId) where TEntityItem : BaseEntityItem {
            return await context.GetByPredicateAsync<TEntityItem>(c=>c.ParentId == parentId);
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
        public static async Task<TEntity> GetCacheByPredicateAsync<TEntity>(this DbContext context, Expression<Func<TEntity, bool>> predicate) where TEntity : class {

            var cachedData = await CacheManager.GetAllQueryableCacheAsync<TEntity>();

            if (cachedData != null && cachedData.Any()) {

                return cachedData.FirstOrDefault(predicate);

            }

            return null;

        }
        public static async Task<TEntity> GetByPredicateAsync<TEntity>(this DbContext context, Expression<Func<TEntity, bool>> predicate) where TEntity : class {

            return await context.Set<TEntity>().FirstOrDefaultAsync(predicate);

        }
        public static async Task<string> GetGeneratedIDAsync<TEntity>(this DbContext context, string prefix, bool withSlash = true) where TEntity : class {
            // Use the first 3 letters of the class name as default prefix if none is provided
            if (string.IsNullOrEmpty(prefix)) {
                prefix = typeof(TEntity).Name.Substring(2, 3).ToUpper();
            }

            // Get the current count and increment by 1
            var count = await context.Set<TEntity>().CountAsync() + 1;

            // Extract the numeric portion from the prefix if it’s meant to be a number
            if (long.TryParse(prefix, out long baseNumber)) {

                // Increment the base number by the count
                var newIDNumber = baseNumber + count;

                // Return the new ID with or without the slash as specified
                return withSlash ? $"{newIDNumber.ToString().Insert(4, "-")}" : $"{newIDNumber}";
            }

            // Return the new ID with the format "<prefix>-00<count>"
            return withSlash ? $"{prefix}-00{count}" : $"{prefix}{count}";
        }
        public static async Task<string> GetGeneratedIDAsync<TEntity>(this DbContext context, string prefix, bool withSlash, Expression<Func<TEntity, bool>> whereCondition) where TEntity : class{
            // Use the first 3 letters of the class name as default prefix if none is provided
            if (string.IsNullOrEmpty(prefix)) {
                prefix = typeof(TEntity).Name.Substring(2, 3).ToUpper();
            }

            // Apply the where condition if provided
            var query = context.Set<TEntity>().AsQueryable();

            if (whereCondition != null) {
                query = query.Where(whereCondition);
            }

            // Get the count with the where condition, then increment by 1
            var count = await query.CountAsync() + 1;

            // Extract the numeric portion from the prefix if it’s meant to be a number
            if (long.TryParse(prefix, out long baseNumber)) {

                // Increment the base number by the count
                var newIDNumber = baseNumber + count;

                // Return the new ID with or without the slash as specified
                return withSlash ? $"{newIDNumber.ToString().Insert(4, "-")}" : $"{newIDNumber}";
            }

            // Return the new ID with the format "<prefix>-00<count>"
            return withSlash ? $"{prefix}-00{count}" : $"{prefix}{count}";
        }
        #endregion

        #region Get All Method
        public static async Task<IEnumerable<TEntityItem>> GetAllItemsByParentIdAsync<TEntityItem>(this DbContext context, Guid parentId) where TEntityItem : BaseEntityItem {
            // Assuming you already have a GetAllAsync(predicate) method
            return await context.GetAllAsync<TEntityItem>(c=>c.ParentId == parentId);
        }
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
            return await context.GetAllAsync(whereCondition);
        }
        public static async Task<IEnumerable<TEntity>> GetAllWithSearchAsync<TEntity>(this DbContext context, string searchText, DateTime? dateFrom, DateTime? dateTo, int dataLimit = 100, bool isCached = true) where TEntity : class {

            var cachedData = await CacheManager.GetAllEnumerableCacheAsync<TEntity>();

            if (cachedData != null && isCached) {

                var result = cachedData.SearchDateRange(dateFrom, dateTo);

                result = cachedData.SearchText(searchText);

                if (result != null) {

                    result = result.Take(dataLimit);

                    return result.ToList();

                }
            }

            var query = context.Set<TEntity>().AsQueryable();

            query = query.SearchDateRange(dateFrom, dateTo);

            query = query.SearchText(searchText);

            query = query.Take(dataLimit);

            return await query.ToListAsync();
        }
        public static async Task<IEnumerable<TEntity>> GetAllAsync<TEntity>(this DbContext context, bool isCached = true) where TEntity : class {

            var cachedData = await CacheManager.GetAllQueryableCacheAsync<TEntity>();

            if (cachedData != null && isCached) {

                return cachedData;

            }

            return await context.Set<TEntity>().ToListAsync();

        }

        public static async Task<IEnumerable<TEntity>> GetAllUnCachedAsync<TEntity>(this DbContext context) where TEntity : class {
            // Get cached data
            var cached = await CacheManager.GetAllQueryableCacheAsync<TEntity>();

            // Get data from the database (query not executed yet)
            var query = context.Set<TEntity>().AsQueryable();

            // If cached data exists
            if (cached != null) {

                var queryData = query.SearchDateRange(DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1)).ToList();
                var cachedData = cached.SearchDateRange(DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1)).ToList();

                // Compare the two lists based on their properties
                var newOrModifiedData = CComparison.CompareLists(queryData, cachedData);

                // Step 1: Get the primary key property
                var pkeyProperty = context.GetPrimaryKeyOfDbContext<TEntity>();

                var pkeyValues = await context.GetPrimaryKeyValuesAsync<TEntity>();

                var cachedIds = cached.Select(item => pkeyProperty.GetValue(item))
                                .ToList();

                var deletedIds = cachedIds.Except(pkeyValues).ToList();

                if(deletedIds.Count > 0) {

                    await context.RemoveAllByIdsFromCacheAsync<TEntity>(deletedIds);

                }
                // If there is new or modified data, return it
                if (newOrModifiedData.Any()) {

                    return newOrModifiedData;

                }
                // If no new or modified data, return cached data
                return null;
            }

            return await query.ToListAsync();
        }

        public static async Task<IEnumerable<TEntity>> GetAllAsync<TEntity>(this DbContext context, Expression<Func<TEntity, bool>> whereCondition, bool isCached = true) where TEntity : class {

            var cachedData = await CacheManager.GetAllQueryableCacheAsync<TEntity>();

            if (cachedData != null && isCached) {

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

        public static async Task<List<object>> GetPrimaryKeyValuesAsync<TEntity>(this DbContext context) where TEntity : class {
            var keyProperty = context.GetPrimaryKeyOfDbContext<TEntity>();
            var keyType = keyProperty.PropertyType;

            // Parameter expression (e =>)
            var parameter = Expression.Parameter(typeof(TEntity), "e");

            // Property access (e.KeyProperty)
            var propertyAccess = Expression.Property(parameter, keyProperty.Name);

            // Create lambda expression (e => e.KeyProperty)
            var lambda = Expression.Lambda(propertyAccess, parameter);

            // Create the IQueryable of primary key values using the correct types
            var query = context.Set<TEntity>().AsQueryable();

            // Use reflection to invoke Select method with the correct generic parameters
            var selectMethod = typeof(Queryable)
                .GetMethods()
                .First(m => m.Name == "Select" && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(TEntity), keyType);

            // Execute Select with the lambda expression
            var primaryKeysQuery = selectMethod.Invoke(null, new object[] { query, lambda });

            // Convert the result to List<object> and execute asynchronously (no need for MakeGenericMethod)
            var toListAsyncMethod = typeof(System.Data.Entity.QueryableExtensions)
                .GetMethods()
                .First(m => m.Name == nameof(System.Data.Entity.QueryableExtensions.ToListAsync) && m.GetParameters().Length == 1);

            // Execute asynchronously
            var task = (Task)toListAsyncMethod.Invoke(null, new object[] { primaryKeysQuery });
            await task.ConfigureAwait(false);

            // Convert results to List<object>
            var result = (IEnumerable)task.GetType().GetProperty("Result").GetValue(task);
            return result.Cast<object>().ToList();
        }

        #endregion

        #region Check extra property 
        private static void UpdateFieldsOfEntity<TEntity>(TEntity entity)
            where TEntity : class {

            var type = typeof(TEntity);

            // Check for DateModified property
            var dateModifiedProperty = type.GetProperty("DateModified");
            if (dateModifiedProperty != null && dateModifiedProperty.CanWrite) {
                dateModifiedProperty.SetValue(entity, DateTime.Now);
            }

            // Check for ModifiedBy property
            var modifiedByProperty = type.GetProperty("ModifiedBy");
            if (modifiedByProperty != null && modifiedByProperty.CanWrite) {
                modifiedByProperty.SetValue(entity, CAppConstants.USERNAME); // Default to "Unknown" if null
            }
        }
        private static void UpdateFieldsOfEntities<TEntity>(ICollection<TEntity> entities)
            where TEntity : class {
            foreach (var entity in entities) {
                UpdateFieldsOfEntity(entity);
            }
        }
        #endregion

        #region Utilities
        public static async Task<bool> HasDataAsync<TEntity>(this DbContext context, Expression<Func<TEntity, bool>> predicate = null) where TEntity : class {
            return predicate != null ? await context.Set<TEntity>().AnyAsync(predicate) : await context.Set<TEntity>().AnyAsync();
        }
        #endregion

    }
}
