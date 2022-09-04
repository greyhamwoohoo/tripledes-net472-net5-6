# tripledes-net472-net5-6
Implement TripleDES Encryption on .Net5 so that it produces the same output as .Net472

NOTE: Everything shown in this implementation for .Net 5 applies to .Net 6

# Background
The TripleDES Implementation is different between Net472 and NET5 when using CFB and PKCS7 Padding: using exactly the same code, key and IV on both platforms to encrypt the word 'HELLO' will produce different ciphertext:

| Platform | ClearText | Output |
| -- | -- | -- |
| Net472 | Hello | UrTy2SxcTbY= |
| Net5   | Hello | UrTy2Sxe     |

This is a known issue and is discussed at length [here](https://github.com/dotnet/runtime/issues/43234).

# Explanation
The specific issue in this case is to do with the Padding implementation in .Net5 when CFB mode is used. 

To get .Net5 to encrypt using TripleDES the same as Net472, we need to implement the PKCS7 padding ourselves in .Net5. Decryption works fine. 

# Implementation
Two implementations are included here:

Net472 includes the 'reference implementation' that encrypts as expected - PKCS7 padding is implemented as nature intended when using CFB mode and it just works. 

Net5 has two implementations:

1. EncryptUsingDefaultPadding will encrypt using the default implementation; it will produce different ciphertexts than Net472 for most inputs. This is the problem we want to solve. 
2. Encrypt will encrypt using the custom padding implementation. This is the solution we seek. 

# Round-tripping
Capture the outputs from both applications and compare the files; if they have parity, confidence is high (!) that the clearText has produced the same cipherText on both platforms:

```bash
 .\src\Net472\bin\Debug\Net472.exe  > output\net472.txt
 .\src\Net5\bin\Debug\net5.0\Net5.exe > output\net5.txt
```

# Caveats
This makes sense to me as a solution; but I am no expert on encryption. Use at your own risk. 
