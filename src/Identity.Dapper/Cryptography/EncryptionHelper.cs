using Identity.Dapper.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Security.Cryptography;

namespace Identity.Dapper.Cryptography
{
    public class EncryptionHelper
    {
        private enum CryptoFunction
        {
            Encrypt, Decrypt
        }

        private readonly IOptions<AESKeys> _aesKeys;
        private readonly ILogger<EncryptionHelper> _log;
        public EncryptionHelper(IOptions<AESKeys> aesKeys, ILogger<EncryptionHelper> log)
        {
            _aesKeys = aesKeys;
            _log = log;
        }

        /// <summary>
        /// Tries to decrypt a AES256 encrypted string.
        /// If the string is not encrypted or if it can't be decrypted, return the original string.
        /// </summary>
        /// <param name="encrypted"></param>
        /// <returns></returns>
        public string TryDecryptAES256(string encrypted) => AES256(CryptoFunction.Decrypt, encrypted);

        public string EncryptAES256(string toEncrypt) => AES256(CryptoFunction.Encrypt, toEncrypt);

        private string AES256(CryptoFunction cryptoFunction, string input)
        {
            try
            {
                if (string.IsNullOrEmpty(_aesKeys.Value.Key) || string.IsNullOrEmpty(_aesKeys.Value.IV))
                    return input;

                using (var aes = Aes.Create())
                {
                    ICryptoTransform cryptoTransform;
                    switch (cryptoFunction)
                    {
                        case CryptoFunction.Encrypt:
                            cryptoTransform = aes.CreateEncryptor(Convert.FromBase64String(_aesKeys.Value.Key), Convert.FromBase64String(_aesKeys.Value.IV));
                            break;
                        case CryptoFunction.Decrypt:
                            cryptoTransform = aes.CreateDecryptor(Convert.FromBase64String(_aesKeys.Value.Key), Convert.FromBase64String(_aesKeys.Value.IV));
                            break;
                        default:
                            cryptoTransform = null;
                            break;
                    }

                    using (cryptoTransform)
                    {
                        var inputBytes = System.Text.Encoding.UTF8.GetBytes(input);

                        var transformedBytes = cryptoTransform.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
                        return System.Text.Encoding.UTF8.GetString(transformedBytes);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);

                return input;
            }
        }
    }
}
