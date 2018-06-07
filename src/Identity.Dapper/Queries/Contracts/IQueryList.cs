using System;
using System.Collections.Concurrent;

namespace Identity.Dapper.Queries.Contracts
{
    public interface IQueryList
    {
        ConcurrentDictionary<Type, IQuery> RetrieveQueryList();
    }
}
