using System.Data.Common;

namespace Identity.Dapper.Connections
{
    public interface IConnectionProvider
    {
        DbConnection Create();
    }
}