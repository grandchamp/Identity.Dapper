using Identity.Dapper.Queries.Contracts;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Identity.Dapper.Queries
{
    public class QueryList : IQueryList
    {
        private readonly ConcurrentDictionary<Type, IQuery> _dictionary;

        private readonly IServiceProvider _serviceProvider;
        public QueryList(IServiceProvider serviceProvider)
        {
            _dictionary = new ConcurrentDictionary<Type, IQuery>();
            _serviceProvider = serviceProvider;
        }

        public ConcurrentDictionary<Type, IQuery> RetrieveQueryList()
        {
            if (_dictionary.Count == 0)
            {
                var platform = Environment.OSVersion.Platform.ToString();
                var runtimeAssemblyNames = DependencyContext.Default.GetRuntimeAssemblyNames(platform);

                var exportedTypes = runtimeAssemblyNames.Select(Assembly.Load)
                                                        .Where(x => x.FullName.StartsWith("Identity."))
                                                        .SelectMany(x => x.ExportedTypes);

                foreach (var type in exportedTypes)
                {
                    var getConstructorParameters = new Func<Type, List<object>>(x =>
                    {
                        var constructorParameters = type.GetTypeInfo()
                                                        .DeclaredConstructors
                                                        .FirstOrDefault(y => y.IsPublic)
                                                        .GetParameters();

                        var parameterList = new List<object>();
                        foreach (var parameter in constructorParameters)
                            parameterList.Add(_serviceProvider.GetService(parameter.ParameterType));

                        return parameterList;
                    });

                    if (typeof(IInsertQuery).IsAssignableFrom(type) && !type.IsAbstract)
                        _dictionary.TryAdd(type, Activator.CreateInstance(type, getConstructorParameters(type).ToArray()) as IInsertQuery);
                    else if (typeof(IDeleteQuery).IsAssignableFrom(type) && !type.IsAbstract)
                        _dictionary.TryAdd(type, Activator.CreateInstance(type, getConstructorParameters(type).ToArray()) as IDeleteQuery);
                    else if (typeof(ISelectQuery).IsAssignableFrom(type) && !type.IsAbstract)
                        _dictionary.TryAdd(type, Activator.CreateInstance(type, getConstructorParameters(type).ToArray()) as ISelectQuery);
                    else if (typeof(IUpdateQuery).IsAssignableFrom(type) && !type.IsAbstract)
                        _dictionary.TryAdd(type, Activator.CreateInstance(type, getConstructorParameters(type).ToArray()) as IUpdateQuery);
                }
            }

            return _dictionary;
        }
    }
}
