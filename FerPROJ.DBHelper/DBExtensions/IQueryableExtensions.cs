using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace FerPROJ.DBHelper.DBExtensions {
    public static class IQueryableExtensions {
        public static async Task RemoveAndCommitAsync<TEntity>(this DbContext context, List<TEntity> entity) where TEntity : class {
            context.Set<TEntity>().RemoveRange(entity);

            await context.SaveChangesAsync();
        }
        public static async Task RemoveAsync<TEntity>(this DbContext context, List<TEntity> entity) where TEntity : class {
            context.Set<TEntity>().RemoveRange(entity);
            await Task.CompletedTask;
        }
        public static async Task RemoveAndCommitAsync<TEntity>(this DbContext context, TEntity entity) where TEntity : class{
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
        public static async Task UpdateAsync<TEntity>(
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
        public static async Task UpdateAndCommitAsync<TEntity>(
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
        public static async Task SaveAsync<TEntity>(
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
        public static async Task SaveAndCommitAsync<TEntity>(
             this DbContext context,
             List<TEntity> entity)
             where TEntity : class {

            context.Set<TEntity>().AddRange(entity);

            await context.SaveChangesAsync();
        }
        //
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
        //
        public static IQueryable<T> SearchDateRange<T>(this IQueryable<T> queryable, DateTime? dateFrom, DateTime? dateTo) {
            if (dateFrom == null && dateTo == null) {
                return queryable; // Return the original query if both dateFrom and dateTo are null.
            }

            // Get all DateTime properties of the type
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                      .Where(p => p.PropertyType == typeof(DateTime));

            if (!properties.Any()) {
                return queryable; // Return the original query if there are no DateTime properties.
            }

            // Build the predicate expression
            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
            Expression predicateBody = null;

            foreach (var property in properties) {
                // Create the expression: x.PropertyName >= dateFrom && x.PropertyName <= dateTo
                var propertyExpression = Expression.Property(parameter, property);

                Expression condition = null;

                if (dateFrom != null) {
                    var fromExpression = Expression.GreaterThanOrEqual(propertyExpression, Expression.Constant(dateFrom));
                    condition = fromExpression;
                }

                if (dateTo != null) {
                    var toExpression = Expression.LessThanOrEqual(propertyExpression, Expression.Constant(dateTo));
                    condition = condition == null ? toExpression : Expression.AndAlso(condition, toExpression);
                }

                // Combine with 'Or' logic for each property
                predicateBody = predicateBody == null ? condition : Expression.OrElse(predicateBody, condition);
            }

            // Create the lambda expression: x => x.Property1 >= dateFrom && x.Property1 <= dateTo || x.Property2 ...
            var predicate = Expression.Lambda<Func<T, bool>>(predicateBody, parameter);

            // Apply the predicate to the queryable
            return queryable.Where(predicate);
        }
        public static IQueryable<T> SearchText<T>(this IQueryable<T> queryable, string searchText) {
            if (string.IsNullOrEmpty(searchText)) {
                return queryable; // Return original query if searchText is empty.
            }

            // Get the properties of the type
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                      .Where(p => p.PropertyType == typeof(string)); // Only string properties

            // Build the predicate expression
            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
            Expression predicateBody = null;

            foreach (var property in properties) {
                // Create expression: x.PropertyName != null && x.PropertyName.Contains(searchText)
                var propertyExpression = Expression.Property(parameter, property);

                // Check if the property is not null
                var notNullExpression = Expression.NotEqual(propertyExpression, Expression.Constant(null));

                // Use Contains method
                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                var searchExpression = Expression.Constant(searchText, typeof(string));
                var containsExpression = Expression.Call(propertyExpression, containsMethod, searchExpression);

                // Combine not-null and contains expressions
                var combinedExpression = Expression.AndAlso(notNullExpression, containsExpression);

                // Combine each property expression with the 'Or' logic
                predicateBody = predicateBody == null ? combinedExpression : Expression.Or(predicateBody, combinedExpression);
            }

            if (predicateBody == null) {
                return queryable; // No string properties to search
            }

            // Create the lambda expression: x => x.Property1.Contains(searchText) || x.Property2.Contains(searchText) ...
            var predicate = Expression.Lambda<Func<T, bool>>(predicateBody, parameter);

            // Apply the predicate to the queryable
            return queryable.Where(predicate);
        }
    }
}
