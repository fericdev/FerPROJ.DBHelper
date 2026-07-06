using FerPROJ.DBHelper.Entity;
using FerPROJ.Design.Class;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Linq.Expressions;
using System.Text;

namespace FerPROJ.DBHelper.DBExtensions {
    public static class ExpressionExtensions {
        public static string ToQuery<T>(this Expression<Func<T, bool>> expression) {

            var visitor = new QueryBuilderVisitor();

            if (typeof(BaseEntity).IsAssignableFrom(typeof(T))) {
                // x =>
                var parameter = expression.Parameters[0];
                // x.Status
                var statusProperty = Expression.Property(parameter, nameof(BaseEntity.Status));

                // Status.ACTIVE
                var activeValue = Expression.Constant(CAppConstants.ACTIVE_STATUS);

                // x.Status == Status.ACTIVE
                var activeExpression = Expression.Equal(statusProperty, activeValue);

                // (original) && x.Status == Status.ACTIVE
                var body = Expression.AndAlso(expression.Body, activeExpression);

                visitor.Visit(body);

                return visitor.GetQuery();
            }

            visitor.Visit(expression.Body);

            return visitor.GetQuery();
        }
    }

    public class QueryBuilderVisitor : ExpressionVisitor {

        private readonly StringBuilder _query = new StringBuilder();

        public string GetQuery() => _query.ToString();

        protected override Expression VisitBinary(BinaryExpression node) {
            switch (node.NodeType) {
                case ExpressionType.AndAlso:
                    Visit(node.Left);
                    Visit(node.Right);
                    break;

                case ExpressionType.Equal:
                    AppendComparison(node, "=");
                    break;

                case ExpressionType.NotEqual:
                    AppendComparison(node, "!=");
                    break;

                case ExpressionType.GreaterThan:
                    AppendComparison(node, ">");
                    break;

                case ExpressionType.GreaterThanOrEqual:
                    AppendComparison(node, ">=");
                    break;

                case ExpressionType.LessThan:
                    AppendComparison(node, "<");
                    break;

                case ExpressionType.LessThanOrEqual:
                    AppendComparison(node, "<=");
                    break;
            }

            return node;
        }

        protected override Expression VisitUnary(UnaryExpression node) {
            // Handles: !x.Flag OR !x.Equals(y)
            if (node.NodeType == ExpressionType.Not) {
                // Case: !x.Equals(y)
                if (node.Operand is MethodCallExpression call &&
                    call.Method.Name == "Equals") {
                    var member = call.Object as MemberExpression;
                    var value = GetValue(call.Arguments[0]);

                    _query.Append($"&{GetMemberName(member)}!={value}");
                    return node;
                }

                // Case: !boolFlag
                if (node.Operand is MemberExpression memberExpr) {
                    _query.Append($"&{GetMemberName(memberExpr)}=false");
                    return node;
                }
            }

            return base.VisitUnary(node);
        }

        protected override Expression VisitMember(MemberExpression node) {
            // Handles: c => c.Undeclared
            if (node.Type == typeof(bool)) {
                _query.Append($"&{GetMemberName(node)}=true");
                return node;
            }

            return base.VisitMember(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node) {
            // Handles: x.Equals(value)
            if (node.Method.Name == "Equals") {
                if (node.Object is MemberExpression member) {
                    var value = GetValue(node.Arguments[0]);
                    _query.Append($"&{GetMemberName(member)}={value}");
                    return node;
                }

                // static string.Equals(a, b)
                if (node.Arguments.Count == 2) {
                    var memberExpr = node.Arguments[0] as MemberExpression;
                    var value = GetValue(node.Arguments[1]);

                    _query.Append($"&{GetMemberName(memberExpr)}={value}");
                    return node;
                }
            }

            // Handle IsEquals extension
            if (node.Method.Name == "IsEquals" && node.Arguments.Count == 2) {

                // c.Type
                var stringExpr = Visit(node.Arguments[0]); 

                // ledgerAccountType
                var enumExpr = node.Arguments[1];          

                // Extract enum value (must be constant or captured variable)
                object enumValue = null;

                if (enumExpr is MemberExpression memberExpr) {
                    var objectMember = Expression.Convert(memberExpr, typeof(object));
                    var getterLambda = Expression.Lambda<Func<object>>(objectMember);
                    enumValue = getterLambda.Compile().Invoke();
                }

                else if (enumExpr is ConstantExpression constExpr) {
                    enumValue = constExpr.Value;
                }

                if (!enumValue.IsNullOrEmpty()) {

                    var memberEnumExpr = node.Arguments[0] as MemberExpression;

                    // Convert enum to string OUTSIDE expression
                    var enumString = enumValue.ToString();

                    _query.Append($"&{GetMemberName(memberEnumExpr)}={enumString}");

                    return node;
                }
            }

            return base.VisitMethodCall(node);
        }

        private void AppendComparison(BinaryExpression node, string op) {
            var leftMember = ExtractMember(node.Left);
            var rightValue = GetValue(node.Right);

            if (leftMember == null) {
                leftMember = ExtractMember(node.Right);
                rightValue = GetValue(node.Left);
            }

            if (leftMember == null)
                return;

            _query.Append($"&{leftMember.Member.Name}{op}{rightValue}");
        }

        private MemberExpression ExtractMember(Expression expression) {
            if (expression == null) return null;

            if (expression is MemberExpression member)
                return member;

            if (expression is UnaryExpression unary)
                return ExtractMember(unary.Operand);

            if (expression is LambdaExpression lambda)
                return ExtractMember(lambda.Body);

            if (expression is MethodCallExpression call && call.Object != null)
                return ExtractMember(call.Object);

            return null;
        }

        private string GetMemberName(MemberExpression member) {
            return member?.Member?.Name ?? string.Empty;
        }

        private object GetValue(Expression expression) {
            if (expression is ConstantExpression constant)
                return NormalizeValue(constant.Value);

            var lambda = Expression.Lambda(expression);
            var compiled = lambda.Compile();
            return NormalizeValue(compiled.DynamicInvoke());
        }

        private object NormalizeValue(object value) {
            if (value is bool b)
                return b.ToString().ToLowerInvariant();

            return value;
        }
    }
}