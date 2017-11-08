namespace Identity.Dapper.Queries.Contracts
{
    public interface IInsertQuery : IQuery
    {
        string GetQuery<TEntity>(TEntity entity);
    }
}
