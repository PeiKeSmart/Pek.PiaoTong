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
            var des = new TripleDESCryptoServiceProvider
            {
                Key = Encoding.UTF8.GetBytes(aStrKey),
                Mode = mode
            };
            if (mode == CipherMode.CBC)
            {
                des.IV = Encoding.UTF8.GetBytes(iv);
            }
            var desEncrypt = des.CreateEncryptor();
            byte[] buffer = Encoding.UTF8.GetBytes(aStrString);
            return Convert.ToBase64String(desEncrypt.TransformFinalBlock(buffer, 0, buffer.Length));
        }
        catch (Exception e)
        {
            return string.Empty;
        }
    }
    public static string Decrypt3Des(string aStrString, string aStrKey, CipherMode mode = CipherMode.ECB, string iv = "12345678")
    {
        try
        {
            var des = new TripleDESCryptoServiceProvider
            {
                Key = Encoding.UTF8.GetBytes(aStrKey),
                Mode = mode,
                Padding = PaddingMode.PKCS7
            };
            if (mode == CipherMode.CBC)
            {
                des.IV = Encoding.UTF8.GetBytes(iv);
            }
            var desDecrypt = des.CreateDecryptor();
            var result = "";
            byte[] buffer = Convert.FromBase64String(aStrString);
            result = Encoding.UTF8.GetString(desDecrypt.TransformFinalBlock(buffer, 0, buffer.Length));
            return result;
        }
        catch (Exception e)
        {
            return string.Empty;
        }
    }


}