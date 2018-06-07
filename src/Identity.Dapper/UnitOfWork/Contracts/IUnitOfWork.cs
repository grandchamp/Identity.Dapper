using System;
using System.Data.Common;

namespace Identity.Dapper.UnitOfWork.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        DbTransaction Transaction { get; }
        DbConnection Connection { get; }

        DbConnection CreateOrGetConnection();
        void DiscardChanges();
        void CommitChanges();
    }
}
