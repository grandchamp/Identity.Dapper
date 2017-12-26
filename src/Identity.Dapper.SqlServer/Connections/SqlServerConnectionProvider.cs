using Identity.Dapper.Connections;
using Identity.Dapper.Cryptography;
using Identity.Dapper.Models;
using Microsoft.Extensions.Options;
using System;
using System.Data.Common;
using System.Data.SqlClient;

namespace Identity.Dapper.SqlServer.Connections
{
    public class SqlServerConnectionProvider : IConnectionProvider
    {
        private readonly IOptions<ConnectionProviderOptions> _connectionProviderOptions;
        private readonly EncryptionHelper _encryptionHelper;
        public SqlServerConnectionProvider(IOptions<ConnectionProviderOptions> connProvOpts, EncryptionHelper encHelper)
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

            var sqlConnectionBuilder = new SqlConnectionStringBuilder(_connectionProviderOptions.Value.ConnectionString)
            {
                Password = string.IsNullOrEmpty(_connectionProviderOptions.Value?.Password)
                                                   ? string.Empty
                                                   : _encryptionHelper.TryDecryptAES256(_connectionProviderOptions.Value.Password),
                UserID = string.IsNullOrEmpty(_connectionProviderOptions.Value?.Username)
                                                   ? string.Empty
                                                   : _connectionProviderOptions.Value.Username
            };

            return new SqlConnection(sqlConnectionBuilder.ToString());
        }
    }
}
