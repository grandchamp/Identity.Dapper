using Identity.Dapper.Cryptography;
using Identity.Dapper.Models;
using Identity.Dapper.SqlServer.Connections;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Identity.Dapper.Tests.ConnectionProviders
{
    public class SqlServerConnectionProviderTest
    {
        private readonly string _key = "E546C8DF278CD5931069B522E695D4F2";  // 32 bytes key for AES256

        private readonly Mock<ILogger<EncryptionHelper>> _mockLogger;
        private readonly Mock<IOptions<AESKeys>> _mockKeys;
        private readonly EncryptionHelper _encryptionHelper;

        public SqlServerConnectionProviderTest()
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
            var connectionString = "Data Source=myServerName;Initial Catalog=myDataBase;User ID=xxxx;Password=xxxx";
            var options = new ConnectionProviderOptions
            {
                ConnectionString = connectionString,
                Username = "testUsername",
                Password = "testPassword"
            };
            var mock = new Mock<IOptions<ConnectionProviderOptions>>();
            mock.Setup(x => x.Value).Returns(options);
            var connectionProvider = new SqlServerConnectionProvider(mock.Object, _encryptionHelper);

            var connection = connectionProvider.Create();
            var expected = "Data Source=myServerName;Initial Catalog=myDataBase;User ID=testUsername;Password=testPassword";

            // connection string should have username/password substituded in
            Assert.Equal(connection.ConnectionString, expected);
        }

        [Fact]
        public void WithEncryptedCredentials()
        {
            var connectionString = "Data Source=myServerName;Initial Catalog=myDataBase;User ID=xxxx;Password=xxxx";
            var options = new ConnectionProviderOptions
            {
                ConnectionString = connectionString,
                Username = "testUsername",
                Password = "U29tZVJlYWxseUNvb2xJVqkPdV+l1d6/7cks09hR9PY="
            };
            var mock = new Mock<IOptions<ConnectionProviderOptions>>();
            mock.Setup(x => x.Value).Returns(options);
            var connectionProvider = new SqlServerConnectionProvider(mock.Object, _encryptionHelper);

            var connection = connectionProvider.Create();
            var expected = "Data Source=myServerName;Initial Catalog=myDataBase;User ID=testUsername;Password=testPassword";

            // connection string should have username/password substituded in
            Assert.Equal(connection.ConnectionString, expected);
        }

        [Fact]
        public void WithoutCredentials()
        {
            var connectionString = "Data Source=myServerName;Initial Catalog=myDataBase;User ID=xxxx;Password=xxxx";
            var options = new ConnectionProviderOptions
            {
                ConnectionString = connectionString,
                Username = "",
                Password = ""
            };
            var mock = new Mock<IOptions<ConnectionProviderOptions>>();
            mock.Setup(x => x.Value).Returns(options);
            var connectionProvider = new SqlServerConnectionProvider(mock.Object, _encryptionHelper);

            var connection = connectionProvider.Create();

            // connection string should be unchanged
            Assert.Equal(connection.ConnectionString, connectionString);
        }

    }
}
