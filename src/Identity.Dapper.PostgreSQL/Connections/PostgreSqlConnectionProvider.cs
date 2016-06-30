using Identity.Dapper.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Common;
using Microsoft.Extensions.Options;
using Identity.Dapper.Cryptography;
using Identity.Dapper.Models;
using Npgsql;

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

            if (string.IsNullOrEmpty(_connectionProviderOptions.Value?.Password))
                throw new ArgumentNullException("There's no DapperIdentity:Password configured. Please, register the value.");

            if (string.IsNullOrEmpty(_connectionProviderOptions.Value?.Username))
                throw new ArgumentNullException("There's no DapperIdentity:Username configured. Please, register the value.");

            var pSqlConnectionBuilder = new NpgsqlConnectionStringBuilder(_connectionProviderOptions.Value.ConnectionString);
            pSqlConnectionBuilder.Password = _encryptionHelper.TryDecryptAES256(_connectionProviderOptions.Value.Password);
            pSqlConnectionBuilder.Username = _connectionProviderOptions.Value.Username;

            return new NpgsqlConnection(pSqlConnectionBuilder.ToString());
        }
    }
}
