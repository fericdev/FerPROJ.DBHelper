using FerPROJ.Design.Class;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FerPROJ.DBHelper.DBExtensions {
    public static class DBTransactionExtensions {
        public static async Task RemoveRangeAndCommitAsync<TEntity>(this DbContext context, ICollection<TEntity> entity) where TEntity : class {
            context.Set<TEntity>().RemoveRange(entity);

            await context.SaveChangesAsync();
        }
        public static async Task RemoveRangeAsync<TEntity>(this DbContext context, ICollection<TEntity> entity) where TEntity : class {
            context.Set<TEntity>().RemoveRange(entity);
            await Task.CompletedTask;
        }
        public static async Task RemoveAndCommitAsync<TEntity>(this DbContext context, TEntity entity) where TEntity : class {
            context.Set<TEntity>().Remove(entity);

            await context.SaveChangesAsync();
        }
        public static async Task RemoveAsync<TEntity>(this DbContext context, TEntity entity) where TEntity : class {
            context.Set<TEntity>().Remove(entity);

            await Task.CompletedTask;
        }
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
                context.Set<TRelatedEntity>().RemoveRange(toRemove);
            }

            // Set foreign key for each related entity
            foreach (var relatedItem in relatedItems) {
                setForeignKeyForRelatedEntity(relatedItem, foreignKeyValue);
                context.Set<TRelatedEntity>().AddOrUpdate(relatedItem);
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
        }
        public static async Task UpdateRangeAsync<TEntity>(
             this DbContext context,
             List<TEntity> entity)
             where TEntity : class {
            foreach (var item in entity) {
                context.Set<TEntity>().AddOrUpdate(item);
            }
            await Task.CompletedTask;
        }
        public static async Task UpdateAndCommitAsync<TEntity>(
             this DbContext context,
             TEntity entity)
             where TEntity : class {

            context.Set<TEntity>().AddOrUpdate(entity);

            await context.SaveChangesAsync();
        }
        public static async Task UpdateRangeAndCommitAsync<TEntity>(
             this DbContext context,
             List<TEntity> entity)
             where TEntity : class {

            foreach (var item in entity) {
                context.Set<TEntity>().AddOrUpdate(item);
            }
            await context.SaveChangesAsync();
        }
        public static async Task SaveAsync<TEntity>(
             this DbContext context,
             TEntity entity)
             where TEntity : class {

            context.Set<TEntity>().Add(entity);

            await Task.CompletedTask;
        }
        public static async Task SaveRangeAsync<TEntity>(
             this DbContext context,
             List<TEntity> entity)
             where TEntity : class {

            context.Set<TEntity>().AddRange(entity);

            await Task.CompletedTask;
        }
        public static async Task SaveAndCommitAsync<TEntity>(
             this DbContext context,
             TEntity entity)
             where TEntity : class {

            context.Set<TEntity>().Add(entity);

            await context.SaveChangesAsync();
        }
        public static async Task SaveRangeAndCommitAsync<TEntity>(
             this DbContext context,
             List<TEntity> entity)
             where TEntity : class {

            context.Set<TEntity>().AddRange(entity);

            await context.SaveChangesAsync();
        }
        // DTO
        public static async Task SaveDTOAsync<TSource, TEntity>(this DbContext context, TSource myDTO) where TSource : CValidator where TEntity : class {

            myDTO.DateCreated = DateTime.Now;
            myDTO.CreatedBy = CStaticVariable.USERNAME;
            myDTO.Status = CStaticVariable.ACTIVE_STATUS;

            var tbl = new CMapping<TSource, TEntity>().GetMappingResult(myDTO);

            context.Set<TEntity>().Add(tbl);

            await Task.CompletedTask;
        }
        public static async Task SaveDTOAndCommitAsync<TSource, TEntity>(this DbContext context, TSource myDTO) where TSource : CValidator where TEntity : class {
            
            myDTO.DateCreated = DateTime.Now;
            myDTO.CreatedBy = CStaticVariable.USERNAME;
            myDTO.Status = CStaticVariable.ACTIVE_STATUS;

            var tbl = new CMapping<TSource, TEntity>().GetMappingResult(myDTO);

            context.Set<TEntity>().Add(tbl);

            await context.SaveChangesAsync();
        }
        public static async Task SaveRangeDTOAsync<TSource, TEntity>(this DbContext context, List<TSource> myDTO) where TSource : CValidator where TEntity : class {

            foreach(var item in myDTO) {
                item.DateCreated = DateTime.Now;
                item.CreatedBy = CStaticVariable.USERNAME;
                item.Status = CStaticVariable.ACTIVE_STATUS;
            }

            var tbl = new CMappingList<TSource, TEntity>().GetMappingResultList(myDTO);

            context.Set<TEntity>().AddRange(tbl);

            await Task.CompletedTask;
        }
        public static async Task SaveRangeDTOAndCommitAsync<TSource, TEntity>(this DbContext context, List<TSource> myDTO) where TSource : CValidator where TEntity : class {

            foreach (var item in myDTO) {
                item.DateCreated = DateTime.Now;
                item.CreatedBy = CStaticVariable.USERNAME;
                item.Status = CStaticVariable.ACTIVE_STATUS;
            }

            var tbl = new CMappingList<TSource, TEntity>().GetMappingResultList(myDTO);

            context.Set<TEntity>().AddRange(tbl);

            await context.SaveChangesAsync();
        }
        public static async Task UpdateDTOAsync<TSource, TEntity>(this DbContext context, TSource myDTO) where TSource : CValidator where TEntity : class {
            
            myDTO.DateModified = DateTime.Now;
            myDTO.ModifiedBy = CStaticVariable.USERNAME;

            var tbl = new CMapping<TSource, TEntity>().GetMappingResult(myDTO);

            context.Set<TEntity>().AddOrUpdate(tbl);

            await Task.CompletedTask;
        }
        public static async Task UpdateDTOAndCommitAsync<TSource, TEntity>(this DbContext context, TSource myDTO) where TSource : CValidator where TEntity : class {

            myDTO.DateModified = DateTime.Now;
            myDTO.ModifiedBy = CStaticVariable.USERNAME;

            var tbl = new CMapping<TSource, TEntity>().GetMappingResult(myDTO);

            context.Set<TEntity>().AddOrUpdate(tbl);

            await context.SaveChangesAsync();
        }
        public static async Task UpdateRangeDTOAsync<TSource, TEntity>(this DbContext context, List<TSource> myDTO) where TSource : CValidator where TEntity : class {

            foreach(var item in myDTO) {
                item.DateModified = DateTime.Now;
                item.ModifiedBy = CStaticVariable.USERNAME;
            }

            var tbl = new CMappingList<TSource, TEntity>().GetMappingResultList(myDTO);

            foreach (var item in tbl) {
                context.Set<TEntity>().AddOrUpdate(item);
            }
            await Task.CompletedTask;
        }
        public static async Task UpdateRangeDTOAndCommitAsync<TSource, TEntity>(this DbContext context, List<TSource> myDTO) where TSource : CValidator where TEntity : class {

            foreach (var item in myDTO) {
                item.DateModified = DateTime.Now;
                item.ModifiedBy = CStaticVariable.USERNAME;
            }

            var tbl = new CMappingList<TSource, TEntity>().GetMappingResultList(myDTO);

            foreach (var item in tbl) {
                context.Set<TEntity>().AddOrUpdate(item);
            }
            await context.SaveChangesAsync();
        }
        public static async Task SetStatusInActiveAsync<TEntity>(this DbContext context, string id) where TEntity : class {
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

            statusProperty.SetValue(entity, CStaticVariable.IN_ACTIVE_STATUS);

            // Mark entity as modified and save changes
            context.Entry(entity).State = EntityState.Modified;
            await context.SaveChangesAsync();
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

            // Mark entity as modified and save changes
            context.Entry(entity).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }
    }
}
