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

            var pSqlConnectionBuilder = new NpgsqlConnectionStringBuilder(_connectionProviderOptions.Value.ConnectionString)
            {
                Password = string.IsNullOrEmpty(_connectionProviderOptions.Value?.Password)
                                                    ? string.Empty
                                                    : _encryptionHelper.TryDecryptAES256(_connectionProviderOptions.Value.Password),
                Username = string.IsNullOrEmpty(_connectionProviderOptions.Value?.Username)
                                                    ? string.Empty
                                                    : _connectionProviderOptions.Value.Username
            };

            return new NpgsqlConnection(pSqlConnectionBuilder.ToString());
        }
    }
}
