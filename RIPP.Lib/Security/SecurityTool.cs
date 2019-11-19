using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace RIPP.Lib.Security
{
    public class SecurityTool
    {
        #region MD5加密密码字段

        private const int START = 8;
        private const int LENGTH = 16;
        public static string BuildPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return password;
            MD5CryptoServiceProvider md5Provider = new MD5CryptoServiceProvider();
            return BitConverter.ToString(md5Provider.ComputeHash(Encoding.Default.GetBytes(password))).Replace("-", "").ToLower().Substring(START, LENGTH);
        }

        #endregion

        #region Des加密

        static byte[] btKey = null;

        static byte[] btIV = null;
        /// <summary>  
        /// 对字符串进行DES加密  
        /// Encrypts the specified sourcestring.  
        /// </summary>  
        /// <param name="sourcestring">The sourcestring.待加密的字符串</param>  
        /// <returns>加密后的BASE64编码的字符串</returns>  
        public static string DesEncrypt(string sourceString, string key = "HyeyWl30")
        {
           

            if (string.IsNullOrEmpty(sourceString))
                return "";
            string myiv = "Hyey20100430";

            if (btKey == null)
                btKey = Encoding.Default.GetBytes(key);
            if (btIV == null)
                btIV = Encoding.Default.GetBytes(myiv);
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] inData = Encoding.Default.GetBytes(sourceString);
                try
                {
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(btKey, btIV), CryptoStreamMode.Write))
                    {
                        cs.Write(inData, 0, inData.Length);
                        cs.FlushFinalBlock();
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
                catch
                {
                    throw;
                }
            }
        }

        /// <summary>  
        /// Decrypts the specified encrypted string.  
        /// 对DES加密后的字符串进行解密  
        /// </summary>  
        /// <param name="encryptedString">The encrypted string.待解密的字符串</param>  
        /// <returns>解密后的字符串</returns>  
        public static string DesDecrypt(string encryptedString, string key = "HyeyWl30")
        {



            if (string.IsNullOrEmpty(encryptedString))
                return "";
            string myiv = "Hyey20100430";
            if (btKey == null)
                btKey = Encoding.Default.GetBytes(key);
            if (btIV == null)
                btIV = Encoding.Default.GetBytes(myiv);
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            using (MemoryStream ms = new MemoryStream())
            {
                byte[] inData = Convert.FromBase64String(encryptedString);
                try
                {
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(btKey, btIV), CryptoStreamMode.Write))
                    {
                        cs.Write(inData, 0, inData.Length);
                        cs.FlushFinalBlock();
                    }
                    return Encoding.Default.GetString(ms.ToArray());
                }
                catch
                {
                    throw;
                }
            }
        }

        public static string MyEncrypt(string sourceString, string key = "HyeyWl30")
        {
            if (string.IsNullOrEmpty(sourceString))
                return "";
            byte[] byt = System.Text.Encoding.UTF8.GetBytes(sourceString);
            return Convert.ToBase64String(byt);
        }

        public static string MyDecrypt(string encryptedString, string key = "HyeyWl30")
        {

            if (string.IsNullOrEmpty(encryptedString))
                return "";
            byte[] b = Convert.FromBase64String(encryptedString);

            return System.Text.Encoding.UTF8.GetString(b);
        }
        #endregion
    }
}