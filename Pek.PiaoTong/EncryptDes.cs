using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Pek.PiaoTong;

public class EncryptDes
{

    /**
     * aStrString   加密内容
     * aStrKey      加密秘钥
     */
    public static String Encrypt3Des(String aStrString, String aStrKey, CipherMode mode = CipherMode.ECB, String iv = "12345678")
    {
        try
        {
            using var des = TripleDES.Create();
            des.Key = Encoding.UTF8.GetBytes(aStrKey);
            des.Mode = mode;
            des.Padding = PaddingMode.PKCS7;

            if (mode == CipherMode.CBC)
            {
                des.IV = Encoding.UTF8.GetBytes(iv);
            }

            using var desEncrypt = des.CreateEncryptor();
            Byte[] buffer = Encoding.UTF8.GetBytes(aStrString);
            return Convert.ToBase64String(desEncrypt.TransformFinalBlock(buffer, 0, buffer.Length));
        }
        catch (CryptographicException)
        {
            return string.Empty;
        }
    }

    public static string Decrypt3Des(string aStrString, string aStrKey, CipherMode mode = CipherMode.ECB, string iv = "12345678")
    {
        try
        {
            using var des = TripleDES.Create();
            des.Key = Encoding.UTF8.GetBytes(aStrKey);
            des.Mode = mode;
            des.Padding = PaddingMode.PKCS7;

            if (mode == CipherMode.CBC)
            {
                des.IV = Encoding.UTF8.GetBytes(iv);
            }

            using var desDecrypt = des.CreateDecryptor();
            Byte[] buffer = Convert.FromBase64String(aStrString);
            return Encoding.UTF8.GetString(desDecrypt.TransformFinalBlock(buffer, 0, buffer.Length));
        }
        catch (Exception) when (String.IsNullOrEmpty(aStrString) || String.IsNullOrEmpty(aStrKey))
        {
            return string.Empty;
        }
        catch (CryptographicException)
        {
            return string.Empty;
        }
        catch (FormatException)
        {
            return string.Empty;
        }
    }
}