using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SacaDev.Util.Encryption
{
    public static class EncryptionHelpers
    {
        // This size of the IV (in bytes) must = (keysize / 8).  Default keysize is 256, so the IV must be
        // 32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.
        private const string initVector = "pemgail9uzpgzl88";
        // This constant is used to determine the keysize of the encryption algorithm
        private const int keysize = 256;

        /// <summary>
		/// Encrypts the given string using rijndaelmanaged with the given key
		/// </summary>
		/// <param name="stringToEncrypt"></param>
		/// <param name="EncryptionKey"></param>
		/// <returns></returns>
        public static string EncryptWithKey(this string stringToEncrypt, string EncryptionKey)
        {
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(stringToEncrypt);

            PasswordDeriveBytes password = new PasswordDeriveBytes(EncryptionKey, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
			using(MemoryStream memoryStream = new MemoryStream()) {
				using(CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write)) {
					cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
					cryptoStream.FlushFinalBlock();
					byte[] cipherTextBytes = memoryStream.ToArray();
					return Convert.ToBase64String(cipherTextBytes);//encode the encrypted bytes using base64.
				}
			}
        }
        /// <summary>
		/// Decrypt the given encrypted rijndaelManaged string with the given key
		/// </summary>
		/// <param name="stringToDecrypt"></param>
		/// <param name="passPhrase"></param>
		/// <returns></returns>
        public static string DecryptWithKey(this string stringToDecrypt, string passPhrase)
        {
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);

            byte[] cipherTextBytes = Convert.FromBase64String(stringToDecrypt);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
        }
    } 
}