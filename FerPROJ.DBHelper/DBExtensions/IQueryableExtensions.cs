using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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

        #region Search By Date
        public static IQueryable<T> SearchDateRange<T>(this IQueryable<T> queryable, DateTime? dateFrom, DateTime? dateTo, string dateProperty = "") {
            if (!dateFrom.HasValue && !dateTo.HasValue) {
                return queryable; // Return original collection if both dates are null
            }

            ParameterExpression param = Expression.Parameter(typeof(T), "x");

            Expression combinedExpression = null;

            // If specific property is provided
            if (!string.IsNullOrEmpty(dateProperty)) {
                combinedExpression = BuildDateExpression<T>(param, dateFrom, dateTo, dateProperty);
            }
            else {
                // Search all DateTime properties
                var dateProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                              .Where(p => p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?))
                                              .ToList();

                foreach (var prop in dateProperties) {
                    var expr = BuildDateExpression<T>(param, dateFrom, dateTo, prop.Name);

                    // Combine multiple expressions using OR
                    combinedExpression = combinedExpression == null
                        ? expr
                        : Expression.OrElse(combinedExpression, expr);
                }
            }

            // If no DateTime properties found, return original query
            if (combinedExpression == null)
                return queryable;

            var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, param);
            return queryable.Where(lambda);
        }

        private static Expression BuildDateExpression<T>(ParameterExpression param, DateTime? dateFrom, DateTime? dateTo, string propertyName) {
            var property = Expression.Property(param, propertyName);
            Expression greaterThanOrEqual = null;
            Expression lessThanOrEqual = null;

            // Handle non-nullable DateTime properties
            if (property.Type == typeof(DateTime)) {
                if (dateFrom.HasValue) {
                    greaterThanOrEqual = Expression.GreaterThanOrEqual(property, Expression.Constant(dateFrom.Value));
                }

                if (dateTo.HasValue) {
                    lessThanOrEqual = Expression.LessThanOrEqual(property, Expression.Constant(dateTo.Value));
                }
            }
            // Handle nullable DateTime? properties
            else if (property.Type == typeof(DateTime?)) {
                var hasValue = Expression.Property(property, "HasValue");
                var value = Expression.Property(property, "Value");

                if (dateFrom.HasValue) {
                    var fromCondition = Expression.GreaterThanOrEqual(value, Expression.Constant(dateFrom.Value));
                    greaterThanOrEqual = Expression.AndAlso(hasValue, fromCondition);
                }

                if (dateTo.HasValue) {
                    var toCondition = Expression.LessThanOrEqual(value, Expression.Constant(dateTo.Value));
                    lessThanOrEqual = Expression.AndAlso(hasValue, toCondition);
                }
            }

            // Combine conditions based on provided date range
            if (greaterThanOrEqual != null && lessThanOrEqual != null) {
                return Expression.AndAlso(greaterThanOrEqual, lessThanOrEqual);
            }
            else if (greaterThanOrEqual != null) {
                return greaterThanOrEqual;
            }
            else {
                return lessThanOrEqual;
            }
        }
        #endregion

    }
}
