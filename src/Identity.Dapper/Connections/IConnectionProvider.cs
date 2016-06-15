using Identity.Dapper.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.Dapper.Connections
{
    public interface IConnectionProvider
    {
        DbConnection Create();
    }
}
