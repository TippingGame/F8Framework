using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace F8Framework.Core
{
    public static partial class Util
    {
        public class OptimizedAES
        {
            public readonly byte[] key;
            public readonly byte[] iv;
            public OptimizedAES(string key, string iv)
            {
                this.key = Encryption.GenerateBytesKey(key);
                this.iv = Encryption.GenerateBytesKey(iv, 16);
            }
        }
        /// <summary>
        /// 加密工具
        /// </summary>
        public static class Encryption
        {
            static StringBuilder stringBuilderCache = new StringBuilder(1024);
            
            public enum GUIDFormat
            {
                N,
                D,
                B,
                P,
                X
            }

            /// <summary>
            /// 生成一个GUID
            /// <para>"N" 	32 位，例如 "33ee30121c43457eabb7e838a5e052e6"</para>
            /// <para>"D" 	32 位,	由连字符分隔的 32 位，例如 "33ee3012-1c43-457e-abb7-e838a5e052e6"</para>
            /// <para>"B" 	32 位，用连字符分隔的 32 位数字，用大括号括起来，例如 "{33ee3012-1c43-457e-abb7-e838a5e052e6}"</para>
            /// <para>"P" 	32 位，用连字符分隔的 32 位数字，括在括号中，例如 "(33ee3012-1c43-457e-abb7-e838a5e052e6)"</para>
            /// <para>"X" 	32 位，四个十六进制值括在大括号中，其中第四个值是八个十六进制值的子集，这些值也括在大括号中，例如 "{0x33ee3012,0x1c43,0x457e,{0xab,0xb7,0xe8,0x38,0xa5,0xe0,0x52,0xe6}}"</para>
            /// </summary>
            /// <param name="format">格式化类型</param>
            /// <returns>格式化后的GUID</returns>
            public static string GUID(GUIDFormat format = GUIDFormat.N)
            {
                return Guid.NewGuid().ToString(format.ToString());
            }

            /// <summary>
            /// MD5加密，返回16位加密后的大写16进制字符
            /// </summary>
            /// <param name="context">需要加密的字符</param>
            /// <returns>加密后的结果</returns>
            public static string MD5Encrypt16(string context)
            {
                byte[] md5Bytes = Encoding.UTF8.GetBytes(context);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] cryptString = md5.ComputeHash(md5Bytes);
                stringBuilderCache.Clear();
                for (int i = 4; i < 12; i++)
                {
                    stringBuilderCache.Append(cryptString[i].ToString("X2"));
                }

                return stringBuilderCache.ToString();
            }

            /// <summary>
            /// MD5加密，返回32位加密后的大写16进制字符
            /// </summary>
            /// <param name="context">需要加密的字符</param>
            /// <returns>加密后的结果</returns>
            public static string MD5Encrypt32(string context)
            {
                byte[] md5Bytes = Encoding.UTF8.GetBytes(context);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] cryptString = md5.ComputeHash(md5Bytes);
                stringBuilderCache.Clear();
                int length = cryptString.Length;
                for (int i = 0; i < length; i++)
                {
                    //X大写的16进制，x小写
                    stringBuilderCache.Append(cryptString[i].ToString("X2"));
                }

                return stringBuilderCache.ToString();
            }

            /// <summary>
            /// MD5加密 
            /// </summary>
            /// <param name="context">需要加密的字符</param>
            /// <returns>加密后的结果</returns>
            public static string MD5Encrypt(string context)
            {
                byte[] md5Bytes = Encoding.UTF8.GetBytes(context);
                MD5 md5 = MD5.Create();
                byte[] cryptBytes = md5.ComputeHash(md5Bytes);
                int length = cryptBytes.Length;
                for (int i = 0; i < length; i++)
                {
                    //X大写的16进制，x小写
                    stringBuilderCache.Append(cryptBytes[i].ToString("X2"));
                }

                return stringBuilderCache.ToString();
            }

            /// <summary>
            /// 生成密钥
            /// </summary>
            /// <param name="key">原始密钥信息</param>
            /// <param name="dstLen">密钥位数，如8，16，24，32</param>
            /// <returns>加密后的值</returns>
            public static byte[] GenerateBytesKey(string key, int dstLen = 32)
            {
                if (string.IsNullOrEmpty(key))
                    return Array.Empty<byte>();

                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] inputBytes = Encoding.UTF8.GetBytes(key);
                    byte[] hashBytes = sha256.ComputeHash(inputBytes);
                    byte[] dstBytes = new byte[dstLen];
                    Array.Copy(hashBytes, 0, dstBytes, 0, dstLen);
                    return dstBytes;
                }
            }

            /// <summary>
            /// 加密算法HMACSHA1 base64
            /// </summary>
            /// <param name="context">被加密的数据</param>
            /// <param name="key">加密密码</param>
            /// <returns>加密后的字段</returns>
            public static string HmacSHA1ToBase64(string context, string key)
            {
                string encrpytedResult = string.Empty;
                using (HMACSHA1 mac = new HMACSHA1(Encoding.UTF8.GetBytes(key)))
                {
                    byte[] hashMsg = mac.ComputeHash(Encoding.UTF8.GetBytes(context));
                    encrpytedResult = Convert.ToBase64String(hashMsg);
                }

                return encrpytedResult;
            }

            /// <summary>
            /// 加密算法HMACSHA1
            /// </summary>
            /// <param name="context">被加密的数据</param>
            /// <param name="key">加密密码</param>
            /// <returns>加密后的字段</returns>
            public static string HmacSHA1(string context, string key)
            {
                using (HMACSHA1 mac = new HMACSHA1(Encoding.UTF8.GetBytes(key)))
                {
                    byte[] hash = mac.ComputeHash(Encoding.UTF8.GetBytes(context));
                    return BitConverter.ToString(hash).Replace("-", "");
                }
            }

            /// <summary>
            /// 加密算法HMACSHA1，输出16位字符串
            /// </summary>
            /// <param name="context">被加密的数据</param>
            /// <param name="key">加密密码</param>
            /// <returns>加密后的字段</returns>
            public static string HmacSHA1ToHex(string context, string key)
            {
                string encrpytedResult = string.Empty;
                using (HMACSHA1 mac = new HMACSHA1(Encoding.UTF8.GetBytes(key)))
                {
                    byte[] hashBytes = mac.ComputeHash(Encoding.UTF8.GetBytes(context));
                    int length = hashBytes.Length;
                    stringBuilderCache.Clear();
                    for (int i = 0; i < length; i++)
                    {
                        stringBuilderCache.Append(hashBytes[i].ToString("X2"));
                    }

                    encrpytedResult = stringBuilderCache.ToString();
                }

                return encrpytedResult;
            }

            /// <summary>
            /// 加密算法HMACSHA256
            /// </summary>
            /// <param name="context">被加密的数据</param>
            /// <param name="key">加密密钥</param>
            /// <returns>加密后的字段</returns>
            public static string HmacSHA256(string context, string key)
            {
                using (HMACSHA256 mac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
                {
                    byte[] hash = mac.ComputeHash(Encoding.UTF8.GetBytes(context));
                    return BitConverter.ToString(hash).Replace("-", "");
                }
            }

            /// <summary>
            /// 加密算法HMACSHA256 base64
            /// </summary>
            /// <param name="context">被加密的数据</param>
            /// <param name="key">加密密钥</param>
            /// <returns>加密后的字段</returns>
            public static string HmacSHA256ToBase64(string context, string key)
            {
                var keyBytes = Encoding.UTF8.GetBytes(key);
                var textBytes = Encoding.UTF8.GetBytes(context);
                using (HMACSHA256 mac = new HMACSHA256(keyBytes))
                {
                    byte[] hash = mac.ComputeHash(textBytes);
                    return Convert.ToBase64String(hash);
                }
            }

            /// <summary>
            /// 加密算法HMACSHA256，输出16位字符串
            /// </summary>
            /// <param name="context">被加密的数据</param>
            /// <param name="key">加密密码</param>
            /// <returns>加密后的字段</returns>
            public static string HmacSHA256ToHex(string context, string key)
            {
                string encrpytedResult = string.Empty;
                using (HMACSHA256 mac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
                {
                    byte[] hashBytes = mac.ComputeHash(Encoding.UTF8.GetBytes(context));
                    int length = hashBytes.Length;
                    stringBuilderCache.Clear();
                    for (int i = 0; i < length; i++)
                    {
                        stringBuilderCache.Append(hashBytes[i].ToString("X2"));
                    }

                    encrpytedResult = stringBuilderCache.ToString();
                }

                return encrpytedResult;
            }

            /// <summary>
            /// 加密字节数组
            /// </summary>
            public static byte[] AES_Encrypt(byte[] plainText, OptimizedAES optimizedAES)
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = optimizedAES.key;
                    aes.IV = optimizedAES.iv;

                    using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                    using (var ms = new MemoryStream())
                    {
                        // 加密数据
                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            cs.Write(plainText, 0, plainText.Length);
                        }

                        return ms.ToArray();
                    }
                }
            }

            /// <summary>
            /// 解密字节数组
            /// </summary>
            public static byte[] AES_Decrypt(byte[] cipherText, OptimizedAES optimizedAES)
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = optimizedAES.key;
                    aes.IV = optimizedAES.iv;

                    using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    using (var ms = new MemoryStream(cipherText))
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    using (var resultStream = new MemoryStream())
                    {
                        cs.CopyTo(resultStream);
                        return resultStream.ToArray();
                    }
                }
            }

            /// <summary>
            /// 加密字符串（UTF-8 编码）
            /// </summary>
            public static string AES_Encrypt(string plainText, OptimizedAES optimizedAES)
            {
                if (plainText.IsNullOrEmpty())
                {
                    return plainText;
                }
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                byte[] encryptedBytes = AES_Encrypt(plainBytes, optimizedAES);
                return Convert.ToBase64String(encryptedBytes);
            }

            /// <summary>
            /// 解密字符串（UTF-8 编码）
            /// </summary>
            public static string AES_Decrypt(string cipherText, OptimizedAES optimizedAES)
            {
                if (cipherText.IsNullOrEmpty())
                {
                    return cipherText;
                }
                byte[] encryptedBytes = Convert.FromBase64String(cipherText);
                byte[] decryptedBytes = AES_Decrypt(encryptedBytes, optimizedAES);
                return Encoding.UTF8.GetString(decryptedBytes);
            }
            
            // 生成 RSA 密钥对
            public static (string publicKey, string privateKey) GenerateRsaKeyPair()
            {
                using (RSA rsa = RSA.Create())
                {
                    string publicKey = rsa.ToXmlString(false);
                    string privateKey = rsa.ToXmlString(true);
                    return (publicKey, privateKey);
                }
            }
            
            /// <summary>
            /// 使用 RSA 加密字符串，返回 Base64 编码的加密结果
            /// </summary>
            /// <param name="plainText">要加密的字符串</param>
            /// <param name="publicKeyXml">RSA 公钥（XML 格式）</param>
            /// <param name="padding">填充模式（默认 Pkcs1）</param>
            /// <returns>Base64 编码的加密结果</returns>
            public static string RSA_Encrypt(string plainText, string publicKeyXml, RSAEncryptionPadding padding = null)
            {
                byte[] dataToEncrypt = Encoding.UTF8.GetBytes(plainText);
                byte[] encryptedData = RSA_Encrypt(dataToEncrypt, publicKeyXml, padding);
                return Convert.ToBase64String(encryptedData);
            }

            /// <summary>
            /// 使用 RSA 加密字节数组，返回加密后的字节数组
            /// </summary>
            /// <param name="dataToEncrypt">要加密的字节数组</param>
            /// <param name="publicKeyXml">RSA 公钥（XML 格式）</param>
            /// <param name="padding">填充模式（默认 Pkcs1）</param>
            /// <returns>加密后的字节数组</returns>
            public static byte[] RSA_Encrypt(byte[] dataToEncrypt, string publicKeyXml, RSAEncryptionPadding padding = null)
            {
                using (RSA rsa = RSA.Create())
                {
                    rsa.FromXmlString(publicKeyXml);
                    padding = padding ?? RSAEncryptionPadding.Pkcs1; // 默认使用 Pkcs1 填充
                    return rsa.Encrypt(dataToEncrypt, padding);
                }
            }

            /// <summary>
            /// 使用 RSA 解密 Base64 编码的字符串，返回解密后的字符串
            /// </summary>
            /// <param name="cipherText">Base64 编码的加密字符串</param>
            /// <param name="privateKeyXml">RSA 私钥（XML 格式）</param>
            /// <param name="padding">填充模式（默认 Pkcs1）</param>
            /// <returns>解密后的字符串</returns>
            public static string RSA_Decrypt(string cipherText, string privateKeyXml, RSAEncryptionPadding padding = null)
            {
                byte[] encryptedData = Convert.FromBase64String(cipherText);
                byte[] decryptedData = RSA_Decrypt(encryptedData, privateKeyXml, padding);
                return Encoding.UTF8.GetString(decryptedData);
            }

            /// <summary>
            /// 使用 RSA 解密字节数组，返回解密后的字节数组
            /// </summary>
            /// <param name="cipherText">要解密的字节数组</param>
            /// <param name="privateKeyXml">RSA 私钥（XML 格式）</param>
            /// <param name="padding">填充模式（默认 Pkcs1）</param>
            /// <returns>解密后的字节数组</returns>
            public static byte[] RSA_Decrypt(byte[] cipherText, string privateKeyXml, RSAEncryptionPadding padding = null)
            {
                using (RSA rsa = RSA.Create())
                {
                    rsa.FromXmlString(privateKeyXml);
                    padding = padding ?? RSAEncryptionPadding.Pkcs1; // 默认使用 Pkcs1 填充
                    return rsa.Decrypt(cipherText, padding);
                }
            }

            /// <summary>
            /// 生成验证码
            /// </summary>
            /// <param name="length">指定验证码的长度</param>
            /// <returns>验证码字符串</returns>
            public static string CreateValidateCode(int length)
            {
                string ch = "abcdefghjkmnpqrstuvwxyzABCDEFGHJKMNPQRSTUVWXYZ1234567890@#$%&?";
                byte[] bytes = new byte[4];
                using (var cpt = new RNGCryptoServiceProvider())
                {
                    cpt.GetBytes(bytes);
                    var r = new Random(BitConverter.ToInt32(bytes, 0));
                    stringBuilderCache.Clear();
                    for (int i = 0; i < length; i++)
                    {
                        stringBuilderCache.Append(ch[r.Next(ch.Length)]);
                    }

                    return stringBuilderCache.ToString();
                }
            }

            /// <summary>
            /// 异或加密
            /// </summary>
            /// <param name="context">需要加密的内容</param>
            /// <param name="key">密钥</param>
            /// <returns>加密后的内容</returns>
            public static byte[] XOR_Encrypt(byte[] context, byte[] key)
            {
                byte[] outputBytes = new byte[context.Length];
                var cntLength = outputBytes.Length;
                var keyLength = key.Length;
                for (int i = 0; i < cntLength; i++)
                {
                    outputBytes[i] = (byte)(context[i] ^ key[i % keyLength]);
                }

                return outputBytes;
            }

            //X | Y | Result
            //==============
            //0 | 0 | 0
            //1 | 0 | 1
            //0 | 1 | 1
            //1 | 1 | 0

            /// <summary>
            /// 异或解密
            /// </summary>
            /// <param name="context">需要解密的内容</param>
            /// <param name="key">密钥</param>
            /// <returns>解密后的内容</returns>
            public static byte[] XOR_Decrypt(byte[] context, byte[] key)
            {
                byte[] outputBytes = new byte[context.Length];
                var cntLength = outputBytes.Length;
                var keyLength = key.Length;
                for (int i = 0; i < cntLength; i++)
                {
                    outputBytes[i] = (byte)(context[i] ^ key[i % keyLength]);
                }

                return outputBytes;
            }

            /// <summary>
            /// Generate MD5
            /// </summary>
            /// <param name="context">bytes</param>
            /// <returns>hash</returns>
            public static string GenerateMD5(byte[] context)
            {
#if NET_STANDARD_2_0
                using (var hash = MD5.Create())
#elif NET_4_6
                using (var hash = MD5Cng.Create())
#endif
                {
                    byte[] data = hash.ComputeHash(context);
                    var sBuilder = new StringBuilder();
                    for (int i = 0; i < data.Length; i++)
                    {
                        sBuilder.Append(data[i].ToString("x2"));
                    }

                    return sBuilder.ToString();
                }
            }
        }
    }
}