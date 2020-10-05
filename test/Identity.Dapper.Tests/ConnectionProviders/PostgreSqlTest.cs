using Identity.Dapper.Cryptography;
using Identity.Dapper.Models;
using Identity.Dapper.PostgreSQL.Connections;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Identity.Dapper.Tests.ConnectionProviders
{
    public class PostgreSqlConnectionProviderTest
    {
        private readonly string _key = "E546C8DF278CD5931069B522E695D4F2";  // 32 bytes key for AES256

        private readonly Mock<ILogger<EncryptionHelper>> _mockLogger;
        private readonly Mock<IOptions<AESKeys>> _mockKeys;
        private readonly EncryptionHelper _encryptionHelper;

        public PostgreSqlConnectionProviderTest()
        {
            _mockLogger = new Mock<ILogger<EncryptionHelper>>();
            _mockKeys = new Mock<IOptions<AESKeys>>();

            var aesKeys = new AESKeys
            {
                Key = EncryptionHelper.Base64Encode(_key),
            };
            _mockKeys.Setup(x => x.Value).Returns(aesKeys);
            _encryptionHelper = new EncryptionHelper(_mockKeys.Object, _mockLogger.Object);
        }

        [Fact]
        public void WithUnencryptedCredentials()
        {
            var connectionString = "Host=myServerName;Port=5432;Database=myDataBase;Username=xxxx;Password=xxxx";
            var options = new ConnectionProviderOptions
            {
                ConnectionString = connectionString,
                Username = "testUsername",
                Password = "testPassword"
            };
            var mock = new Mock<IOptions<ConnectionProviderOptions>>();
            mock.Setup(x => x.Value).Returns(options);
            var connectionProvider = new PostgreSqlConnectionProvider(mock.Object, _encryptionHelper);

            var connection = connectionProvider.Create();
            var expected = "Host=myServerName;Port=5432;Database=myDataBase;Username=testUsername;Password=testPassword";

            // connection string should have username/password substituded in
            Assert.Equal(connection.ConnectionString, expected);
        }

        [Fact]
        public void WithEncryptedCredentials()
        {
            var connectionString = "Host=myServerName;Port=5432;Database=myDataBase;Username=xxxx;Password=xxxx";
            var options = new ConnectionProviderOptions
            {
                ConnectionString = connectionString,
                Username = "testUsername",
                Password = "U29tZVJlYWxseUNvb2xJVqkPdV+l1d6/7cks09hR9PY="
            };
            var mock = new Mock<IOptions<ConnectionProviderOptions>>();
            mock.Setup(x => x.Value).Returns(options);
            var connectionProvider = new PostgreSqlConnectionProvider(mock.Object, _encryptionHelper);

            var connection = connectionProvider.Create();
            var expected = "Host=myServerName;Port=5432;Database=myDataBase;Username=testUsername;Password=testPassword";

            // connection string should have username/password substituded in
            Assert.Equal(connection.ConnectionString, expected);
        }

        [Fact]
        public void WithoutCredentials()
        {
            var connectionString = "Host=myServerName;Port=5432;Database=myDataBase;Username=xxxx;Password=xxxx";
            var options = new ConnectionProviderOptions
            {
                ConnectionString = connectionString,
                Username = "",
                Password = ""
            };
            var mock = new Mock<IOptions<ConnectionProviderOptions>>();
            mock.Setup(x => x.Value).Returns(options);
            var connectionProvider = new PostgreSqlConnectionProvider(mock.Object, _encryptionHelper);

            var connection = connectionProvider.Create();

            // connection string should be unchanged
            Assert.Equal(connection.ConnectionString, connectionString);
        }

    }
}
