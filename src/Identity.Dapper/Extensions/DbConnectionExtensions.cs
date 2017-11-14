using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace Identity.Dapper
{
    public static class DbConnectionExtensions
    {
        public static Task WaitForConnectionOpenAsync(this DbConnection conn, string connString)
        {
            var tcs = new TaskCompletionSource<bool>();

            Task.Run(async () =>
            {
                try
                {
                    while (conn.State != System.Data.ConnectionState.Open)
                    {
                        if (conn.State != System.Data.ConnectionState.Connecting || conn.State != System.Data.ConnectionState.Executing || conn.State != System.Data.ConnectionState.Fetching)
                        {
                            conn.ConnectionString = connString;

                            await conn.OpenAsync();
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }

                tcs.SetResult(true);
            });

            return tcs.Task;
        }
    }
}
