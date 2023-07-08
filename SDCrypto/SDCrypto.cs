using System.Security.Cryptography;

namespace SDCrypto
{
    public class SDCrypto
    {

        private int _iterations;
        public SDCrypto()
        {
            this._iterations = 65556;
        }
        public int Iterations => _iterations;

        public string EncryptDataAES(string plainText, string keyString)
        {
            Random rnd = new Random();
            Byte[] salt = new Byte[20];


            rnd.NextBytes(salt);
            var derivedPassword = new Rfc2898DeriveBytes(keyString, salt, _iterations);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            byte[] keyBytes = derivedPassword.GetBytes(symmetricKey.KeySize / 8);

            byte[] toEncrypt = System.Text.Encoding.UTF8.GetBytes(plainText);

            byte[] encryptedData;
            byte[] iv = new byte[16];
            using (Aes aes = Aes.Create())
            {
                aes.Mode = CipherMode.CBC;
                aes.Key = keyBytes;
                aes.IV = iv;
                aes.Padding = PaddingMode.PKCS7;
                ICryptoTransform encryptor = aes.CreateEncryptor();
                using (MemoryStream mStream = new MemoryStream())
                {
                    using (CryptoStream cStream = new CryptoStream(mStream, encryptor, CryptoStreamMode.Write))
                    {
                        cStream.Write(toEncrypt, 0, toEncrypt.Length);
                        cStream.FlushFinalBlock();
                        encryptedData = mStream.ToArray();
                    }
                }
            }

            byte[] saltcipher = new byte[salt.Length + encryptedData.Length];


            Array.Copy(salt, 0, saltcipher, 0, salt.Length);
            Array.Copy(encryptedData, 0, saltcipher, salt.Length, encryptedData.Length);


            return Base16Encode(saltcipher);
        }

        public string DecryptDataAES(string cipherText, string keyString)
        {
            byte[] decryptedBytes = null;
            byte[] salt = new byte[20];
            byte[] encryptedBuffer = Base16Decode(cipherText);
            Array.Copy(encryptedBuffer, salt, salt.Length);

            byte[] cipherbytes = new byte[encryptedBuffer.Length - salt.Length];
            Array.Copy(encryptedBuffer, salt.Length, cipherbytes, 0, cipherbytes.Length);


            var derivedPassword = new Rfc2898DeriveBytes(keyString, salt, _iterations);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            byte[] keyBytes = derivedPassword.GetBytes(symmetricKey.KeySize / 8);
            byte[] iv = new byte[16];
            using (Aes aes = Aes.Create())
            {
                aes.Mode = CipherMode.CBC;
                aes.Key = keyBytes;
                aes.IV = iv;
                aes.Padding = PaddingMode.PKCS7;
                ICryptoTransform decryptor = aes.CreateDecryptor();
                using (MemoryStream mStream = new MemoryStream())
                {
                    using (var cs = new CryptoStream(mStream, decryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(cipherbytes, 0, cipherbytes.Length);
                        cs.Close();
                    }

                    decryptedBytes = mStream.ToArray();
                }
            }


            string returnString = "";
            foreach (byte b in decryptedBytes)
            {
                returnString += (char)b;
            }

            return returnString;
        }

        private readonly char[] ByteToHex = new char[]
        {
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
        'a', 'b', 'c', 'd', 'e', 'f'
        };

        private string Base16Encode(byte[] input)
        {
            char[] output = new char[input.Length * 2];
            int i = 0;

            foreach (byte c in input)
            {
                int low = c & 0xF;
                int high = (c & 0xF0) >> 4;
                output[i++] = ByteToHex[high];
                output[i++] = ByteToHex[low];
            }

            return new String(output);
        }

        private static byte[] Base16Decode(string inputString)
        {
            byte[] returnBytes = new byte[inputString.Length / 2];

            int j = 0;
            for (int i = 0; i < inputString.Length; i = i + 2)
            {
                int c1 = Convert.ToInt32(inputString[i].ToString(), 16);
                int c2 = Convert.ToInt32(inputString[i + 1].ToString(), 16);

                int bt = c1 << 4 | c2;
                returnBytes[j++] = (byte)bt;
            }

            return returnBytes;
        }


    }

}