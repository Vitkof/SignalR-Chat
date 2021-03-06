using System;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;

namespace KeyGenerator
{
    public class Program
    {
        public static void Main() { }
        

        public static void Run(string root)
        {
            using (RSA rsa = RSA.Create())
            {
                byte[] privateKey = rsa.ExportRSAPrivateKey();
                byte[] publicKey = rsa.ExportRSAPublicKey();

                var json = JsonSerializer.Serialize(new
                {
                    Private = Convert.ToBase64String(privateKey),
                    Public = Convert.ToBase64String(publicKey)
                }, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(root + "rsaKeys.json", json);
            }
        }
    }
}
