using System;
using System.Collections.Generic;

namespace Identity.Dapper.Queries.Contracts
{
    public interface IQueryList
    {
        Dictionary<Type, IQuery> RetrieveQueryList();
    }
}
