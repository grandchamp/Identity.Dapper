using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Identity.Dapper
{
    public static class ReflectionExtensions
    {
        public static IEnumerable<string> GetPublicPropertiesNames(this Type type, Func<PropertyInfo, bool> filterBy = null)
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                 .Where(x => x.CanWrite && x.CanRead)
                                 .AsEnumerable();

            if (filterBy != null)
                properties = properties.Where(filterBy);

            return properties.Select(x => x.Name);
        }
    }
}
