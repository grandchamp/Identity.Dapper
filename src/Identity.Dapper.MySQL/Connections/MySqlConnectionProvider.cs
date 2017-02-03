using Identity.Dapper.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Common;
using Identity.Dapper.Models;
using Microsoft.Extensions.Options;
using Identity.Dapper.Cryptography;
using MySql.Data.MySqlClient;

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

            if (string.IsNullOrEmpty(_connectionProviderOptions.Value?.Password))
                throw new ArgumentNullException("There's no DapperIdentity:Password configured. Please, register the value.");

            if (string.IsNullOrEmpty(_connectionProviderOptions.Value?.Username))
                throw new ArgumentNullException("There's no DapperIdentity:Username configured. Please, register the value.");

            var mySqlConnectionBuilder = new MySqlConnectionStringBuilder(_connectionProviderOptions.Value.ConnectionString);
            mySqlConnectionBuilder.Password = _encryptionHelper.TryDecryptAES256(_connectionProviderOptions.Value.Password);
            mySqlConnectionBuilder.UserID = _connectionProviderOptions.Value.Username;

            return new MySqlConnection(mySqlConnectionBuilder.ToString());
        }
    }
}
