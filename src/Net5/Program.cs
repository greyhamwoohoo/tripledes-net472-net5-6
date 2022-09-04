using System;

namespace Net5
{
    class Program
    {
        static void Main(string[] args)
        {
            EncryptAndDecrypt("HELLO");

            var clearTextBigString = "AndOnTheThirdDayHeSaidLetTheEncryptionAndDecryptionWorkAndLoAndBeholdItDid";
            for (int i = 0; i <= clearTextBigString.Length; i++)
            {
                EncryptAndDecrypt(clearTextBigString.Substring(0, i));
            }
        }

        static void EncryptAndDecrypt(string clearText)
        {
            var tripleDESProvider = new TripleDesProvider();

            var cipherTextAsBase64 = tripleDESProvider.Encrypt(clearText);
            var recoveredClearText = tripleDESProvider.Decrypt(cipherTextAsBase64);

            Console.WriteLine($"{clearText} {clearText.Length} {cipherTextAsBase64} {cipherTextAsBase64.Length} {recoveredClearText} {recoveredClearText.Length}");

            if (recoveredClearText != clearText)
            {
                throw new ArgumentOutOfRangeException($"The clearText was not successfully round-tripped. ");
            }
        }
    }
}
