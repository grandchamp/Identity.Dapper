using Identity.Dapper.Connections;
using Identity.Dapper.Cryptography;
using Identity.Dapper.Models;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System;
using System.Data.Common;

namespace Identity.Dapper.MySQL.Connections
{
    public class MySqlConnectionProvider : IConnectionProvider
    {
        private readonly IOptions<ConnectionProviderOptions> _connectionProviderOptions;
        private readonly EncryptionHelper _encryptionHelper;
        public MySqlConnectionProvider(IOptions<ConnectionProviderOptions> connProvOpts, EncryptionHelper encHelper)
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

            var mySqlConnectionBuilder = new MySqlConnectionStringBuilder(_connectionProviderOptions.Value.ConnectionString);

            if (string.IsNullOrEmpty(_connectionProviderOptions.Value?.Password))
                throw new ArgumentNullException("There's no DapperIdentity:Password configured. Please, register the value.");
            else
                mySqlConnectionBuilder.Password = mySqlConnectionBuilder.IntegratedSecurity ? string.Empty : _encryptionHelper.TryDecryptAES256(_connectionProviderOptions.Value.Password);

            if (string.IsNullOrEmpty(_connectionProviderOptions.Value?.Username))
                throw new ArgumentNullException("There's no DapperIdentity:Username configured. Please, register the value.");
            else
                mySqlConnectionBuilder.UserID = mySqlConnectionBuilder.IntegratedSecurity ? string.Empty : _connectionProviderOptions.Value.Username;

            return new MySqlConnection(mySqlConnectionBuilder.ToString());
        }
    }
}
