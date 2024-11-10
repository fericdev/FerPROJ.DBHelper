using FerPROJ.Design.Class;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FerPROJ.DBHelper.DBExtensions {
    public static class IEnumerableExtentions {
        public static IEnumerable<T> SearchDateRange<T>(this IEnumerable<T> queryable, DateTime? dateFrom, DateTime? dateTo, string dateProperty = "") {
            if (dateFrom == null && dateTo == null) {
                return queryable; // Return the original collection if both dateFrom and dateTo are null.
            }

            // Get the specified DateTime property if dateProperty is provided
            PropertyInfo property = null;
            if (!string.IsNullOrEmpty(dateProperty)) {
                property = typeof(T).GetProperty(dateProperty, BindingFlags.Public | BindingFlags.Instance);
                if (property == null) {
                    throw new ArgumentException($"Property '{dateProperty}' is not found or is not of type DateTime.");
                }
            }
            else {
                // Get all DateTime properties of the type
                var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                          .Where(p => p.PropertyType == typeof(DateTime))
                                          .ToList();

                if (!properties.Any()) {
                    return queryable; // Return the original collection if there are no DateTime properties.
                }
            }

            // Filter the collection
            return queryable.Where(item =>
            {
                var propertiesToCheck = property != null ? new List<PropertyInfo> { property } :
                                                           typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                                    .Where(p => p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?))
                                                                    .ToList();

                foreach (var prop in propertiesToCheck) {
                    var propertyValue = (DateTime?)prop.GetValue(item);

                    // Check if the property is within the date range
                    if (propertyValue.HasValue) {
                        bool isAfterStart = !dateFrom.HasValue || propertyValue >= dateFrom.Value;
                        bool isBeforeEnd = !dateTo.HasValue || propertyValue < dateTo.Value.AddDays(1);

                        if (isAfterStart && isBeforeEnd) {
                            return true; // Property is within range
                        }
                    }
                }

                return false; // No properties matched the date range
            });
        }
        public static IEnumerable<T> SearchText<T>(this IEnumerable<T> queryable, string searchText) {
            if (string.IsNullOrEmpty(searchText)) {
                return queryable; // Return original query if searchText is empty.
            }

            // Get the properties of the type
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                      .Where(p => p.PropertyType == typeof(string)); // Only string properties

            if (!properties.Any()) {
                return queryable; // Return original query if no string properties are found.
            }

            // Build the predicate expression
            Func<T, bool> predicate = x => {
                // Loop through all string properties and check if any of them contains the search text
                foreach (var property in properties) {
                    var propertyValue = property.GetValue(x) as string;
                    if (propertyValue != null && propertyValue.SearchFor(searchText)) {
                        return true; // Found a match, no need to check other properties
                    }
                }

                return false; // No match found for any property
            };

            // Apply the predicate to filter the collection
            return queryable.Where(predicate);
        }
        //
        public static async Task<IEnumerable<TEntity>> GetAllAsync<TEntity>(this DbContext context, string propertyName, object propertyValue) where TEntity : class {
            // Get the DbSet for TEntity
            var dbSet = context.Set<TEntity>();

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
                    var lambda = Expression.Lambda<Func<TEntity, bool>>(equality, parameter);

                    // Use the lambda expression in the query
                    var query = dbSet.Where(lambda);
                    return await query.ToListAsync();
                }
            }

            // If no property filter is provided or the property does not exist, return all entities
            return await dbSet.ToListAsync();
        }
        public static async Task<IEnumerable<TEntity>> GetAllAsync<TEntity>(this DbContext context, string status = null) where TEntity : class {
            // Get the DbSet for TEntity
            var dbSet = context.Set<TEntity>();

            if (!string.IsNullOrEmpty(status)) {
                // Check if the Status property exists
                var statusProperty = typeof(TEntity).GetProperty("Status");

                if (statusProperty != null) {
                    // Check if the Status property is of the correct type (e.g., string or int, depending on your design)
                    if (statusProperty.PropertyType == typeof(string)) {
                        // If Status is a string, filter by active status
                        var query = dbSet.Where(e => (string)statusProperty.GetValue(e) == status);
                        return await query.ToListAsync();
                    }
                }
            }

            // If no Status property exists, return all entities
            return await dbSet.ToListAsync();
        }
        public static async Task<IEnumerable<TEntity>> GetAllAsync<TEntity>(this DbContext context, Expression<Func<TEntity, bool>> whereCondition) where TEntity : class {
            // Get the DbSet for TEntity
            var dbSet = context.Set<TEntity>();

            // Apply the where condition if provided
            var query = dbSet.AsQueryable();

            query = query.Where(whereCondition);

            // If no Status property exists, return all entities
            return await query.ToListAsync();
        }
        public static IEnumerable<T> ActiveOnly<T>(this IEnumerable<T> queryable) {
            // Get the Status property if it exists
            var statusProperty = typeof(T).GetProperty("Status");

            // If Status property exists, filter based on active status
            if (statusProperty != null) {
                // Define the predicate to filter active entities
                Func<T, bool> predicate = x => {
                    var statusValue = statusProperty.GetValue(x);

                    // Check if Status property is of type string and matches active status
                    if (statusValue is string statusString && statusString == CStaticVariable.ACTIVE_STATUS) {
                        return true;
                    }

                    return false;
                };

                // Apply the predicate to filter the collection
                return queryable.Where(predicate);
            }

            // If no Status property, return the original collection unfiltered
            return queryable;
        }
        public static IEnumerable<T> InActiveOnly<T>(this IEnumerable<T> queryable) {
            // Get the Status property if it exists
            var statusProperty = typeof(T).GetProperty("Status");

            // If Status property exists, filter based on active status
            if (statusProperty != null) {
                // Define the predicate to filter active entities
                Func<T, bool> predicate = x => {
                    var statusValue = statusProperty.GetValue(x);

                    // Check if Status property is of type string and matches active status
                    if (statusValue is string statusString && statusString == CStaticVariable.IN_ACTIVE_STATUS) {
                        return true;
                    }

                    return false;
                };

                // Apply the predicate to filter the collection
                return queryable.Where(predicate);
            }

            // If no Status property, return the original collection unfiltered
            return queryable;
        }
    }
}
