namespace Identity.Dapper.Queries.Contracts
{
    public interface ISelectQuery : IQuery
    {
        string GetQuery();
        string GetQuery<TEntity>(TEntity entity);
    }
}
