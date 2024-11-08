using FerPROJ.Design.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FerPROJ.DBHelper.DBExtensions {
    public static class IEnumerableExtentions {
        public static IEnumerable<T> SearchDateRange<T>(this IEnumerable<T> queryable, DateTime? dateFrom, DateTime? dateTo) {
            if (dateFrom == null && dateTo == null) {
                return queryable; // Return the original query if both dateFrom and dateTo are null.
            }

            // Get all DateTime properties of the type
            var properties = typeof(T).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                                      .Where(p => p.PropertyType == typeof(DateTime))
                                      .ToList();

            if (!properties.Any()) {
                return queryable; // Return the original query if there are no DateTime properties.
            }

            // Filter the collection
            return queryable.Where(item =>
            {
                foreach (var property in properties) {
                    var propertyValue = (DateTime?)property.GetValue(item);

                    bool isInRange = true;

                    // Check if the property is within the date range (if dateFrom is specified)
                    if (dateFrom.HasValue && propertyValue.HasValue && propertyValue < dateFrom.Value) {
                        isInRange = false;
                    }

                    // Check if the property is within the date range (if dateTo is specified)
                    if (dateTo.HasValue && propertyValue.HasValue && propertyValue > dateTo.Value) {
                        isInRange = false;
                    }

                    // If any of the DateTime properties fail to meet the range conditions, exclude this item
                    if (isInRange) {
                        return true;
                    }
                }

                return false; // If no DateTime properties meet the range criteria
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
    }
}
