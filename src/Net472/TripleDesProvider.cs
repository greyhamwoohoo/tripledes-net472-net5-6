using System;
using System.Security.Cryptography;
using System.Text;

namespace Net472
{
    public class TripleDesProvider
    {
        // Change these :)
        private byte[] TRIPLEDES_KEY = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        private byte[] TRIPLEDES_IV = { 9, 8, 7, 6, 5, 4, 3, 2 };

        public string Encrypt(string clearText)
        {
            var tripleDES = CreateTripleDES(TRIPLEDES_KEY, TRIPLEDES_IV);

            ICryptoTransform encryptor = tripleDES.CreateEncryptor();

            byte[] clearTextAsBytes = Encoding.UTF8.GetBytes(clearText);
            byte[] encryptedBytes = encryptor.TransformFinalBlock(clearTextAsBytes, 0, clearTextAsBytes.Length);
            var cipherTextAsBase64 = Convert.ToBase64String(encryptedBytes);

            return cipherTextAsBase64;
        }

        public string Decrypt(string cipherTextAsBase64)
        {
            var tripleDES = CreateTripleDES(TRIPLEDES_KEY, TRIPLEDES_IV);

            byte[] cipherTextAsBytes = Convert.FromBase64String(cipherTextAsBase64);

            ICryptoTransform decryptor = tripleDES.CreateDecryptor();

            var decryptedText = Encoding.UTF8.GetString(decryptor.TransformFinalBlock(cipherTextAsBytes, 0, cipherTextAsBytes.Length));
            return decryptedText;
        }

        private TripleDES CreateTripleDES(byte[] tripleDesKey, byte[] tripleDesIV)
        {
            var tripleDES = TripleDES.Create();

            tripleDES.KeySize = 128;
            tripleDES.BlockSize = 64;
            tripleDES.FeedbackSize = 8;

            tripleDES.IV = tripleDesIV;
            tripleDES.Key = tripleDesKey;

            tripleDES.Padding = PaddingMode.PKCS7;
            tripleDES.Mode = CipherMode.CFB;

            return tripleDES;
        }
    }
}
