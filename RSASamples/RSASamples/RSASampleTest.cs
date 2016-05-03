using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RSASamples
{
    [TestClass]
    public class RSASampleTest
    {
        [TestMethod]
        public void Test_GenerateKeys()
        {
            var rsaKeys = this.GenerateRSAKeys();
        }

        [TestMethod]
        public void Test_EncryptString()
        {
            var rsaKeys = this.GenerateRSAKeys();

            var encryptString = this.EncryptString(rsaKeys.Item1, "Hello World");
        }

        [TestMethod]
        public void Test_DecryptString()
        {
            var rsaKeys = this.GenerateRSAKeys();

            var encryptString = this.EncryptString(rsaKeys.Item1, "Hello World");

            var decryptString = this.DecryptString(rsaKeys.Item2, encryptString);

            Assert.AreEqual("Hello World", decryptString);
        }

        [TestMethod]
        public void Test_EncryptFile()
        {
            var rsaKeys = this.GenerateRSAKeys();

            var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            this.EncryptFile(
                rsaKeys.Item1,
                Path.Combine(currentDirectory, "TestFiles", "TextFile1.txt"),
                Path.Combine(currentDirectory, "TestFiles", "EncryptedTextFile1.txt"));
        }

        [TestMethod]
        public void Test_DecryptFile()
        {
            var rsaKeys = this.GenerateRSAKeys();

            var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            this.EncryptFile(
                rsaKeys.Item1,
                Path.Combine(currentDirectory, "TestFiles", "TextFile1.txt"),
                Path.Combine(currentDirectory, "TestFiles", "EncryptedTextFile1.txt"));

            this.DecryptFile(
                rsaKeys.Item2,
                Path.Combine(currentDirectory, "TestFiles", "EncryptedTextFile1.txt"),
                Path.Combine(currentDirectory, "TestFiles", "DecryptedTextFile1.txt"));
        }

        private Tuple<string, string> GenerateRSAKeys()
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            var publicKey = rsa.ToXmlString(false);
            var privateKey = rsa.ToXmlString(true);

            return Tuple.Create<string, string>(publicKey, privateKey);
        }

        private string EncryptString(string publicKey, string rawContent)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(publicKey);

            var encryptString = Convert.ToBase64String(rsa.Encrypt(Encoding.UTF8.GetBytes(rawContent), false));

            return encryptString;
        }

        private string DecryptString(string privateKey, string encryptedContent)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(privateKey);

            var decryptString = Encoding.UTF8.GetString(rsa.Decrypt(Convert.FromBase64String(encryptedContent), false));

            return decryptString;
        }

        private void EncryptFile(string publicKey, string rawFilePath, string encryptedFilePath)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(publicKey);

            using (FileStream testDataStream = File.OpenRead(rawFilePath))
            using (FileStream encrytpStream = File.OpenWrite(encryptedFilePath))
            {
                var testDataByteArray = new byte[testDataStream.Length];
                testDataStream.Read(testDataByteArray, 0, testDataByteArray.Length);

                var encryptDataByteArray = rsa.Encrypt(testDataByteArray, false);

                encrytpStream.Write(encryptDataByteArray, 0, encryptDataByteArray.Length);
            }
        }

        private void DecryptFile(string privateKey, string encryptedFilePath, string decryptedFilePath)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(privateKey);

            using (FileStream encrytpStream = File.OpenRead(encryptedFilePath))
            using (FileStream decrytpStream = File.OpenWrite(decryptedFilePath))
            {
                var encryptDataByteArray = new byte[encrytpStream.Length];
                encrytpStream.Read(encryptDataByteArray, 0, encryptDataByteArray.Length);

                var decryptDataByteArray = rsa.Decrypt(encryptDataByteArray, false);

                decrytpStream.Write(decryptDataByteArray, 0, decryptDataByteArray.Length);
            }
        }
    }
}
