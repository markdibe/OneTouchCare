using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OneToucheCare.Models
{
    public class Encription
    {
        public string Encrypt(string EncryptionKey, string clearText)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            clearText = cleanUpEncription(clearText);
            return clearText;
        }

        public string Decrypt(string EncryptionKey, string cipherText)
        {

            cipherText = cipherText.Replace(" ", "+");
            cipherText = MakeItDirtyAgain(cipherText);
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }

            return cipherText;
        }

        public static string cleanUpEncription(string encriptedstring)
        {
            string[] dirtyCharacters = { ";", "/", "?", ":", "@", "&", "=", "+", "$", "," };
            string[] cleanCharacters = { "p2n3t4G5l6m", "s1l2a3s4h", "q1e2st3i4o5n", "T22p14nt2s", "a9t", "a2n3nd", "e1q2ua88l", "p22l33u1ws", "d0l1ar5", "c0m8a1a" };

            foreach (string dirtyCharacter in dirtyCharacters)
            {
                encriptedstring = encriptedstring.Replace(dirtyCharacter, cleanCharacters[Array.IndexOf(dirtyCharacters, dirtyCharacter)]);
            }
            return encriptedstring;
        }

        public static string MakeItDirtyAgain(string encriptedString)
        {
            string[] dirtyCharacters = { ";", "/", "?", ":", "@", "&", "=", "+", "$", "," };
            string[] cleanCharacters = { "p2n3t4G5l6m", "s1l2a3s4h", "q1e2st3i4o5n", "T22p14nt2s", "a9t", "a2n3nd", "e1q2ua88l", "p22l33u1ws", "d0l1ar5", "c0m8a1a" };
            foreach (string symbol in cleanCharacters)
            {
                encriptedString = encriptedString.Replace(symbol, dirtyCharacters[Array.IndexOf(cleanCharacters, symbol)]);
            }
            return encriptedString;
        }
    }
}


