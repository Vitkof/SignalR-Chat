using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using KeyGenerator;

namespace Server.Auth
{
    internal class AuthOptions
    {
        internal string Issuer { get; set; } = "MyIssuer";
        internal string Audience { get; set; } = "SuperServer";
        internal int Lifetime { get; set; } = 1;
        internal SigningCredentials SigningCredentials { get; set; } = new SigningCredentials(GetPrivate(), SecurityAlgorithms.RsaSha256);

        private static string PrivateKeyStr = "MIIEpAIBAAKCAQEAu\u002B\u002BW2ITzTlSfnwqw3VMBzwIBErZNPCV0OdxPc60/9f\u002B2kRqZPiAOhb6B7GclbRCDoh0/8gIsUY5y0tWj0VJM00fom10Uvax/QFJ0KxA1gSnWdFnGAtNS\u002B7h27O60bQKTSK12ANtOLRELOkz1BkfRlVFrAPdQw/oJPByNGEyCXj4fD5A11fBLi32UQkt1usVKs/1bVoHE2RvnSPaYoF4e2jNt9K0cn\u002BoIbuTn0O7vBLurrvYqB1mbNU4j3faeIaZr7mdpCbwx2dgOzXxYBAQblSMGeAm29Z9NYZCd\u002B\u002BOzfHGhyUreIhiuu0aBO6e3CXXzAyCe\u002BAunhuwwCUYiAIL/VQIDAQABAoIBAQCRjiCglWfztOrjvN36rL1r3LuECJmNCd0Yqx8GEprFJkX54EXrrdxRjZkGxWRhMjchKdJK15AHonIgBMMZ7cn\u002BoWTwX4ke1ijAYpwCdk1aOlMUTitkKNPOjbHeE4q\u002BGw6DbYVFaJUFpnuxrcTgFmOmaCad2u48urzIR7ynoyodXl4M5YS8jqj2iSjsPCs1DHgvVrrhu5XtOrfse8I1FNBwKuA\u002Bi1hLCT3yv4hAMjlkauj6sv4EvPSg05h1mRbmXhJuPOI2u\u002B4Sq5xHw2dBS2a35ftzN9Ov0VQOdUJBaCVUoY39x9wSh1u0NVej0MOwdVMK6uuSIna\u002BtFpt6S1nCWgJAoGBAMXBX0jOuwMR31sbFRs8xPyttWcj04rhllLfFuWZhKPfzcBlEP6ZGfSq3HiepN1oP\u002BUijc1JE/MVYef3joycAyfRF5aT46LYISnV5SDyJlQgNnRN37aH/y10LO994ksOLBMSC64gCxr7RVaLq66VJFA8OEIKvqJanksqW1BswftfAoGBAPNJ1e6kRvAbMTmZnU4WyrS2RAdmxJTERB5t1F44brYIc7JqcOZnj3G/Q65K4RWkkmI\u002BmJhWzdNO/4EAJqlLgteYnBFtAkjoltAMuYa2vxWGMI0TI4/B4LPtD1mbJ7cGfcoxTR5X9R1j8dCiXWqLiMOmSUb2ScxJP6fKy6cuGjXLAoGAaXs8pOxISlnlJoZqmq2ucQ0C/rHYa3LqqOeAIhXh7zs4V1BRYUwu0Re2I8yTKdoqgsEMnBxHvNiB8aZVbeDluf5Is7PMrxbTkaoa3pjrIuJPyXekRYuC7C7UrSYbZ2d20PFGM6m54rl2gbRcbfH1faqxY2Cx0I2/c15zeJGaYscCgYEA4D\u002BWqblSic8GuIkXrfrkLV4zpn7bhCTEhSfHMHHj2nY/7pyeJEIgvkAGKolqn4BqTZRVI1Yfsj6G1GlHOZUYrZFFiepoyLtbB7oy6DFaQITIC8EwbjnO3QoNtBKHsPSSzeRTqTx5\u002BvU814pRgPvMvEpyEK7Dp0F9IzSSMy8TibECgYAjfOifKzRi36k64E/8qPGbVqhH8RW5aCbZmc9lowh7Q/U5C\u002Bu4nYdOIfBFPI0nNWB5O8vlOoWjPd7Aok9QUQslIw0NEvaJXifyBsJJpH8t\u002Bm0S1aRjNTBlZzhBSEBNPT/JK27bWqc3tC1bi2hfNi/m4gK8JYLna7DGNv3\u002BsXCmhw==";

        private static string PublicKeyStr = "MIIBCgKCAQEAu\u002B\u002BW2ITzTlSfnwqw3VMBzwIBErZNPCV0OdxPc60/9f\u002B2kRqZPiAOhb6B7GclbRCDoh0/8gIsUY5y0tWj0VJM00fom10Uvax/QFJ0KxA1gSnWdFnGAtNS\u002B7h27O60bQKTSK12ANtOLRELOkz1BkfRlVFrAPdQw/oJPByNGEyCXj4fD5A11fBLi32UQkt1usVKs/1bVoHE2RvnSPaYoF4e2jNt9K0cn\u002BoIbuTn0O7vBLurrvYqB1mbNU4j3faeIaZr7mdpCbwx2dgOzXxYBAQblSMGeAm29Z9NYZCd\u002B\u002BOzfHGhyUreIhiuu0aBO6e3CXXzAyCe\u002BAunhuwwCUYiAIL/VQIDAQAB";

        internal SecurityKey PrivateKey = GetPrivate();
        internal SecurityKey PublicKey = GetPublic();


        private static SecurityKey GetPrivate()
        {
            RSA rsa = RSA.Create();
            rsa.ImportRSAPrivateKey(Convert.FromBase64String(PrivateKeyStr), out _);
            return new RsaSecurityKey(rsa);
        }

        private static SecurityKey GetPublic() 
        {
            RSA rsa = RSA.Create();
            rsa.ImportRSAPublicKey(Convert.FromBase64String(PublicKeyStr), out _);
            return new RsaSecurityKey(rsa);
        }

        
        internal static SecurityKey GetPrivate(string path, string file)
        {
            try
            {
                using var stream = new StreamReader(new FileStream(Path.Combine(path, file), FileMode.Open));
                var content = stream.ReadToEnd();
                PrivateKeyStr = JsonConvert.DeserializeObject<Keys>(content).Private;
                return GetPrivate();
            }
            catch
            {
                KeyGenerator.Program.Run(path);
                using var stream = new StreamReader(new FileStream(Path.Combine(path, file), FileMode.Open));
                var content = stream.ReadToEnd();
                PrivateKeyStr = JsonConvert.DeserializeObject<Keys>(content).Private;
                return GetPrivate();
            }
        }

        internal static SecurityKey GetPublic(string path, string file)
        {
            try
            {
                using var stream = new StreamReader(new FileStream(Path.Combine(path, file), FileMode.Open));
                var content = stream.ReadToEnd();
                PublicKeyStr = JsonConvert.DeserializeObject<Keys>(content).Public;
                return GetPublic();
            }
            catch
            {
                KeyGenerator.Program.Run(path);
                using var stream = new StreamReader(new FileStream(Path.Combine(path, file), FileMode.Open));
                var content = stream.ReadToEnd();
                PublicKeyStr = JsonConvert.DeserializeObject<Keys>(content).Public;
                return GetPublic();
            }
        }
        

        private class Keys
        {
            public string Private;
            public string Public;
        }
    }
}
