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

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static byte[] Base64Decode(string base64EncodedData)
        {
            return System.Convert.FromBase64String(base64EncodedData);
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
                if (string.IsNullOrEmpty(_aesKeys.Value.Key)
                    || string.IsNullOrEmpty(_aesKeys.Value.IV)
                    || string.IsNullOrEmpty(input))
                    return input;

                string result;
                switch (cryptoFunction)
                {
                    case CryptoFunction.Encrypt:
                        result = EncryptInput(input);
                        break;
                    case CryptoFunction.Decrypt:
                        result = DecryptInput(input);
                        break;
                    default:
                        result = null;
                        break;
                }

                return result;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);

                return input;
            }
        }

        private string EncryptInput(string input)
        {
            var key = Base64Decode(_aesKeys.Value.Key);
            var iv = Base64Decode(_aesKeys.Value.IV);

            using (var aes = Aes.Create())
            {
                using (var encryptor = aes.CreateEncryptor(key, iv))
                {
                    using (var msEncrypt = new System.IO.MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (var swEncrypt = new System.IO.StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(input);
                        }

                        var decryptedContent = msEncrypt.ToArray();
                        var result = new byte[iv.Length + decryptedContent.Length];

                        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                        Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

                        return Convert.ToBase64String(result);
                    }
                }
            }
        }

        private string DecryptInput(string input)
        {
            var fullCipher = Convert.FromBase64String(input);
            var iv = Base64Decode(_aesKeys.Value.IV);
            var cipher = new byte[fullCipher.Length - iv.Length];

            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, fullCipher.Length - iv.Length);
            var key = Base64Decode(_aesKeys.Value.Key);

            using (var aes = Aes.Create())
            {
                using (var decryptor = aes.CreateDecryptor(key, iv))
                {
                    string result;
                    using (var msDecrypt = new System.IO.MemoryStream(cipher))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new System.IO.StreamReader(csDecrypt))
                            {
                                result = srDecrypt.ReadToEnd();
                            }
                        }
                    }

                    return result;
                }
            }
        }

    }
}
