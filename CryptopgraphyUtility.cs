using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Seismic.Data.CommonLib
{
    public class CryptographyUtility
    {
        public static byte[] PBKDF2Transform(string password, string encryptionKey, bool isToEncrypt, byte[] sourceData)
        {
            // Our symmetric encryption algorithm
            using (AesManaged aes = new AesManaged())
            {
                // Setting our parameters
                aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
                aes.KeySize = aes.LegalKeySizes[0].MaxSize;

                // We're using the PBKDF2 standard for password-based key generation
                using (Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(encryptionKey)))
                {
                    aes.Key = rfc.GetBytes(aes.KeySize / 8);
                    aes.IV = rfc.GetBytes(aes.BlockSize / 8);
                    //aes.Padding = PaddingMode.PKCS7;
                    // Output stream, can be also a FileStream
                    using (MemoryStream decryptStream = new MemoryStream())
                    {
                        // Now, decryption
                        using (ICryptoTransform decryptTrans = isToEncrypt ? aes.CreateEncryptor() : aes.CreateDecryptor())
                        {
                            using (CryptoStream decryptor = new CryptoStream(decryptStream, decryptTrans, CryptoStreamMode.Write))
                            {
                                decryptor.Write(sourceData, 0, sourceData.Length);
                                decryptor.Flush();
                                decryptor.Close();
                            }
                        }

                        return decryptStream.ToArray();
                    }
                }
            }
        }
    }
}
