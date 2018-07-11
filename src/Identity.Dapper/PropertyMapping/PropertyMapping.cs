using Identity.Dapper.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Identity.Dapper
{
    public abstract class IPropertyMapper<TSource> where TSource : DapperIdentityUser
    {
        public abstract IPropertyMapper<TSource> MapFrom(Expression<Func<TSource, object>> prop);
        public abstract Dictionary<string, IProperty> GetAllMappings();
        internal abstract IPropertyMapper<TSource> ToInternal(string claimType);
    }

    public interface IProperty
    {
        object Value { get; }
    }

    public class Property : IProperty
    {
        public Property(object value)
        {
            Value = value;
        }

        public object Value { get; private set; }
    }

    public class PropertyMapper<TSource> : IPropertyMapper<TSource>
        where TSource : DapperIdentityUser
    {
        private Dictionary<string, IProperty> _mappings;
        private IProperty _currentProperty;
        public PropertyMapper() => _mappings = new Dictionary<string, IProperty>();

        public override Dictionary<string, IProperty> GetAllMappings() => _mappings;

        public override IPropertyMapper<TSource> MapFrom(Expression<Func<TSource, object>> prop)
        {
            if (_currentProperty != null)
                throw new InvalidOperationException("You've called MapFrom without bind with a To.");

            var a = PropertyMapperExtensions.FindProperty(prop);
            var obj =string.Empty;

            var b = a.GetValue(obj);

            return this;
        }

        internal override IPropertyMapper<TSource> ToInternal(string claimType)
        {
            if (_currentProperty == null)
                throw new InvalidOperationException("You've called To without prior MapFrom.");

            _mappings.Add(claimType, _currentProperty);
            _currentProperty = null;

            return this;
        }


    }

    public static class PropertyMapperExtensions
    {
        public static IPropertyMapper<TSource> To<TSource>(this IPropertyMapper<TSource> source, string claimType)
            where TSource : DapperIdentityUser
        {
            source.ToInternal(claimType);

            return source;
        }

        public static object GetValue(this MemberInfo memberInfo, object forObject)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)memberInfo).GetValue(forObject);
                case MemberTypes.Property:
                    return ((PropertyInfo)memberInfo).GetValue(forObject);
                default:
                    throw new NotImplementedException();
            }
        }

        public static MemberInfo FindProperty(LambdaExpression lambdaExpression)
        {
            Expression expressionToCheck = lambdaExpression;

            var done = false;

            while (!done)
            {
                switch (expressionToCheck.NodeType)
                {
                    case ExpressionType.Convert:
                        expressionToCheck = ((UnaryExpression)expressionToCheck).Operand;
                        break;
                    case ExpressionType.Lambda:
                        expressionToCheck = ((LambdaExpression)expressionToCheck).Body;
                        break;
                    case ExpressionType.MemberAccess:
                        var memberExpression = ((MemberExpression)expressionToCheck);

                        if (memberExpression.Expression.NodeType != ExpressionType.Parameter &&
                            memberExpression.Expression.NodeType != ExpressionType.Convert)
                        {
                            throw new ArgumentException(
                                $"Expression '{lambdaExpression}' must resolve to top-level member and not any child object's properties. You can use ForPath, a custom resolver on the child type or the AfterMap option instead.",
                                nameof(lambdaExpression));
                        }

                        var member = memberExpression.Member;

                        return member;
                    default:
                        done = true;
                        break;
                }
            }

            throw new Exception("Custom configuration for members is only supported for top-level individual members on a type.");
        }
    }

}
