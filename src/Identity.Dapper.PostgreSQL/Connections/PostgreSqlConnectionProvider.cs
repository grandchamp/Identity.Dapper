using Identity.Dapper.Connections;
using Identity.Dapper.Cryptography;
using Identity.Dapper.Models;
using Microsoft.Extensions.Options;
using Npgsql;
using System;
using System.Data.Common;

namespace Identity.Dapper.PostgreSQL.Connections
{
    public class PostgreSqlConnectionProvider : IConnectionProvider
    {
        private readonly IOptions<ConnectionProviderOptions> _connectionProviderOptions;
        private readonly EncryptionHelper _encryptionHelper;
        public PostgreSqlConnectionProvider(IOptions<ConnectionProviderOptions> connProvOpts, EncryptionHelper encHelper)
        {
            _connectionProviderOptions = connProvOpts;
            _encryptionHelper = encHelper;
        }

        public DbConnection Create()
        {
            if (_connectionProviderOptions.Value == null)
                throw new ArgumentNullException("There's no DapperIdentity configuration section registered. Please, register the section in appsettings.json or user secrets.");

            if (string.IsNullOrEmpty(_connectionProviderOptions.Value?.ConnectionString))
                throw new ArgumentNullException("There's no DapperIdentity:ConnectionString configured. Please, register the value.");

            var connectionString = _connectionProviderOptions.Value.ConnectionString;
            var username = _connectionProviderOptions.Value?.Username;
            var password = _connectionProviderOptions.Value?.Password;

            // if both a username and password were provided, update the connection string with them
            // otherwise, leave the connection string that was configured alone
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString)
                {
                    Password = _encryptionHelper.TryDecryptAES256(password),
                    Username = username
                };
                connectionString = connectionStringBuilder.ToString();
            }

            return new NpgsqlConnection(connectionString);
        }
    }
}
