using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FerPROJ.DBHelper.DBExtensions {
    public static class ExpressionExtensions  {
        public static string ToQuery<T>(this Expression<Func<T, bool>> expression) {
            var visitor = new QueryBuilderVisitor();
            visitor.Visit(expression);
            return visitor.GetQuery();
        }
    }
    public class QueryBuilderVisitor : ExpressionVisitor {
        private readonly StringBuilder _query = new StringBuilder();

        public string GetQuery() => _query.ToString();

        protected override Expression VisitBinary(BinaryExpression node) {
            if (node.NodeType == ExpressionType.AndAlso) {
                Visit(node.Left);
                Visit(node.Right);
            }
            else if (node.NodeType == ExpressionType.Equal) {
                var member = (MemberExpression)node.Left;
                var constant = GetValue(node.Right);

                _query.Append($"&{member.Member.Name}={constant}");
            }

            return node;
        }

        private object GetValue(Expression expression) {
            if (expression is ConstantExpression constant)
                return constant.Value;

            var lambda = Expression.Lambda(expression);
            var compiled = lambda.Compile();
            return compiled.DynamicInvoke();
        }
    }
}
