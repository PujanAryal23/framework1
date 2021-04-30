using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CredentialsEncryptor
{
    public class EncryptionBusinessJMeter
    {
       #region PUBLIC METHODS

        /// <summary>
        /// Encrypt given data
        /// </summary>
        /// <param name="message"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string EncryptData(string message, string key)
        {
            return Encrypt(key, message);
        }

        /// <summary>
        /// Decrypt data
        /// </summary>
        /// <param name="message"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string DecryptString(string message, string key)
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
            var TDESAlgorithm = new TripleDESCryptoServiceProvider();
            TDESAlgorithm.Key = CreateKey(key);
            TDESAlgorithm.IV = CreateIv(key);
            byte[] dataToEncrypt = Encoding.ASCII.GetBytes(message);
            try
            {
                ICryptoTransform encryptor = TDESAlgorithm.CreateEncryptor();
                results = encryptor.TransformFinalBlock(dataToEncrypt, 0, dataToEncrypt.Length);
            }
            finally
            {
                TDESAlgorithm.Clear();
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
            var TDESAlgorithm = new TripleDESCryptoServiceProvider();
            TDESAlgorithm.Key = CreateKey(key);
            TDESAlgorithm.IV = CreateIv(key);
            byte[] dataToDecrypt = Convert.FromBase64String(message);
            try
            {
                ICryptoTransform decryptor = TDESAlgorithm.CreateDecryptor();
                results = decryptor.TransformFinalBlock(dataToDecrypt, 0, dataToDecrypt.Length);
            }
            finally
            {
                TDESAlgorithm.Clear();
            }
            return Encoding.ASCII.GetString(results);
        }

        private static byte[] CreateKey(string key)
        {
            var keyToUse = new byte[24];
            var UTF8 = new UTF8Encoding();
            var hashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = hashProvider.ComputeHash(UTF8.GetBytes(key));
            Array.Copy(TDESKey, 0, keyToUse, 0, 16);
            Array.Copy(TDESKey, 0, keyToUse, 16, 8);
            return keyToUse;
        }

        private static byte[] CreateIv(string iv)
        {
            var ivToUse = new byte[8];
            var UTF8 = new UTF8Encoding();
            var hashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = hashProvider.ComputeHash(UTF8.GetBytes(iv));
            Array.Copy(TDESKey, 0, ivToUse, 0, 8);
            return ivToUse;
        }

        #endregion

    }
}
