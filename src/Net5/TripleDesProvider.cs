using System;
using System.Security.Cryptography;
using System.Text;

namespace Net5
{
    public class TripleDesProvider
    {
        // Change these :)
        private byte[] TRIPLEDES_KEY = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        private byte[] TRIPLEDES_IV = { 9, 8, 7, 6, 5, 4, 3, 2 };

        public string Encrypt(string clearText)
        {
            var tripleDES = CreateTripleDES(TRIPLEDES_KEY, TRIPLEDES_IV);
            
            // Disable all padding - we will implement PKCS7 padding ourselves
            tripleDES.Padding = PaddingMode.None;

            byte[] clearTextAsBytes = Encoding.UTF8.GetBytes(clearText);

            // We need to 'pad' the bytes ourselves so it looks like PKCS7
            // For more information, see: https://github.com/dotnet/runtime/blob/main/src/libraries/System.Security.Cryptography/src/System/Security/Cryptography/SymmetricPadding.cs
            var blockSizeInBytes = tripleDES.BlockSize / 8;
            byte[] paddedClearTextAsBytes = new byte[((clearTextAsBytes.Length + blockSizeInBytes) / blockSizeInBytes) * blockSizeInBytes];

            for (int i = 0; i < paddedClearTextAsBytes.Length; i++)
            {
                paddedClearTextAsBytes[i] = Convert.ToByte(blockSizeInBytes - (clearTextAsBytes.Length % blockSizeInBytes));
            }

            // After the .CopyTo, the remaining bytes will remain as to the size of the padding (PKCS7)
            clearTextAsBytes.CopyTo(paddedClearTextAsBytes, 0);

            ICryptoTransform encryptor = tripleDES.CreateEncryptor();

            byte[] encryptedBytes = encryptor.TransformFinalBlock(paddedClearTextAsBytes, 0, paddedClearTextAsBytes.Length);
            var cipherTextAsBase64 = Convert.ToBase64String(encryptedBytes);

            return cipherTextAsBase64;
        }

        public string EncryptUsingDefaultPadding(string clearText)
        {
            // This implemented uses exactly the same settings as Net472 but produces different ciphertext for some inputs. 
            // This is included as reference. It is this problem we need to solve. 
            var tripleDES = CreateTripleDES(TRIPLEDES_KEY, TRIPLEDES_IV);
            
            tripleDES.Padding = PaddingMode.PKCS7;

            byte[] clearTextAsBytes = Encoding.UTF8.GetBytes(clearText);

            ICryptoTransform encryptor = tripleDES.CreateEncryptor();

            byte[] encryptedBytes = encryptor.TransformFinalBlock(clearTextAsBytes, 0, clearTextAsBytes.Length);
            var cipherTextAsBase64 = Convert.ToBase64String(encryptedBytes);

            return cipherTextAsBase64;
        }

        public string Decrypt(string cipherTextAsBase64)
        {
            var tripleDES = CreateTripleDES(TRIPLEDES_KEY, TRIPLEDES_IV);

            // We implemented our own specific PKCS7 padding for encryption; so when we decrypt, setting this PaddingMode is what we want. 
            tripleDES.Padding = PaddingMode.PKCS7;

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

            tripleDES.Mode = CipherMode.CFB;

            return tripleDES;
        }
    }
}
