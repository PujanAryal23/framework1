using System;
using System.Security.Cryptography;
using System.Text;

namespace EncryptionBusiness.Core
{
    /// <summary>
    /// This algorithm use the following services :
    /// 1. MD5CryptoServiceProvider - generates an array of TDESKey with byte type by computing hash key.
    /// 2. TripleDESCryptoServiceProvider - a cryptoserviceprovider generates key, mode and padding for encryption and decryption of a message.
    /// 3. ICryptoTransform - creates encryptor/decryptor and transform into final block generating expected result
    /// </summary>
    public class EncryptionAlgorithm
    {
        #region PRIVATE FIELDS
        /// <summary>
        /// Default encryption key
        /// </summary>
        private const string HASH_KEY = "encryption_key";

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Encrypt given data
        /// </summary>
        /// <param name="message"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string EncryptData(string message, string key = HASH_KEY)
        {
            return Encrypt(key, message);
        }

        /// <summary>
        /// Decrypt data
        /// </summary>
        /// <param name="message"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string DecryptString(string message, string key = HASH_KEY)
        {
            return Decrypt(key, message);
        }

        #endregion

        #region PRIVATE MEHODSS
        
        /// <summary>
        /// Encryption 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private static string Encrypt(string key, string message)
        {
            byte[] results;
            var UTF8 = new UTF8Encoding();

            var hashProvider = new MD5CryptoServiceProvider();

            byte[] TDESKey = hashProvider.ComputeHash(UTF8.GetBytes(key));
            var TDESAlgorithm = new TripleDESCryptoServiceProvider();
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;
            byte[] dataToEncrypt = UTF8.GetBytes(message);
            try
            {
                ICryptoTransform encryptor = TDESAlgorithm.CreateEncryptor();
                results = encryptor.TransformFinalBlock(dataToEncrypt, 0, dataToEncrypt.Length);
            }
            finally
            {
                TDESAlgorithm.Clear();
                hashProvider.Clear();
            }
            return Convert.ToBase64String(results);
        }

        /// <summary>
        /// Decryption
        /// </summary>
        /// <param name="key"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private static string Decrypt(string key, string message)
        {

            byte[] results;
            var UTF8 = new UTF8Encoding();

            var hashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = hashProvider.ComputeHash(UTF8.GetBytes(key));
            var TDESAlgorithm = new TripleDESCryptoServiceProvider();
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;
            byte[] dataToDecrypt = Convert.FromBase64String(message);
            try
            {
                ICryptoTransform decryptor = TDESAlgorithm.CreateDecryptor();
                results = decryptor.TransformFinalBlock(dataToDecrypt, 0, dataToDecrypt.Length);
            }
            finally
            {
                TDESAlgorithm.Clear();
                hashProvider.Clear();
            }
            return UTF8.GetString(results);
        }

        #endregion

    }


}
