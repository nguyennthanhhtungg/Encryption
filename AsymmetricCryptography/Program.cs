using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AsymmetricCryptography
{
    class Program
    {
        static void Main(string[] args)
        {
            GenKey_SaveInContainer("MyKeyContainer");

            RSACryptoServiceProvider rsa = GetKeyFromContainer("MyKeyContainer");

            string publicKey = rsa.ToXmlString(false);

            string privateKey = rsa.ToXmlString(true);


            EncryptText(publicKey, "Hello AsymmetricCryptography!", "encryptedData.txt");

            Console.WriteLine($"Decrypted message: {DecryptData(privateKey, "encryptedData.txt")}");

            DeleteKeyFromContainer("MyKeyContainer");
            Console.ReadKey();
        }

        static void GenKey_SaveInContainer(string containerName)
        {
            CspParameters parameters = new CspParameters
            {
                KeyContainerName = containerName
            };

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(parameters);

            Console.WriteLine($"Key added to container: \n  {rsa.ToXmlString(true)}");
        }

        static RSACryptoServiceProvider GetKeyFromContainer(string containerName)
        {
            CspParameters parameters = new CspParameters
            {
                KeyContainerName = containerName
            };

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(parameters);

            Console.WriteLine($"Key retrieved from container : \n {rsa.ToXmlString(true)}");

            return rsa;
        }

        static void DeleteKeyFromContainer(string containerName)
        {
            CspParameters parameters = new CspParameters
            {
                KeyContainerName = containerName
            };

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(parameters)
            {
                PersistKeyInCsp = false
            };

            rsa.Clear();
            Console.WriteLine("Key deleted.");
        }

        static void EncryptText(string publicKey, string text, string fileName)
        {
            UnicodeEncoding byteConverter = new UnicodeEncoding();
            byte[] dataToEncrypt = byteConverter.GetBytes(text);

            byte[] encryptedData;
            using(RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKey);

                encryptedData = rsa.Encrypt(dataToEncrypt, false);
            }

            File.WriteAllBytes(fileName, encryptedData);
            Console.WriteLine("Data has been encrypted");
        }

        static string DecryptData(string privateKey, string fileName)
        {
            byte[] dataToDecrypt = File.ReadAllBytes(fileName);

            byte[] decryptedData;
            using(RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(privateKey);
                decryptedData = rsa.Decrypt(dataToDecrypt, false);
            }

            UnicodeEncoding byteConverter = new UnicodeEncoding();
            return byteConverter.GetString(decryptedData);
        }
    }
}
