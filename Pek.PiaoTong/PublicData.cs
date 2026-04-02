using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pek.PiaoTong;

public class PublicData
{
    /**
     * 公共报文组装
     */
    public static string publicparam(string content, string platformCode,
        string platformAlias, string privateKey, string password)
    {
        StringBuilder sign = new StringBuilder();
        string contentstr = EncryptDes.Encrypt3Des(content, password);
        sign.Append("content=" + contentstr + "&");
        sign.Append("format=JSON&");
        sign.Append("platformCode=" + platformCode + "&");
        string time = DateTime.Now.ToString("yyyyMMddHHmmss");
        string serianlNo = platformAlias + time + GenerateCheckCode(8);
        sign.Append("serialNo=" + serianlNo + "&");
        sign.Append("signType=RSA&");
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        sign.Append("timestamp=" + timestamp + "&");
        sign.Append("version=1.0");
        string rsasign = RSA.sign(sign.ToString(), privateKey);
        Hashtable publictable = new Hashtable();
        publictable.Add("sign", rsasign);
        publictable.Add("format", "JSON");
        publictable.Add("platformCode", platformCode);
        publictable.Add("serialNo", serianlNo);
        publictable.Add("signType", "RSA");
        publictable.Add("timestamp", timestamp);
        publictable.Add("version", "1.0");
        publictable.Add("content", contentstr);
        return ToJson.Table2Json(publictable);
    }

    public static string disposeResponse(string str, string ptpublickey, string deskey)
    {
        // 使用 JsonDocument 解析 JSON，避免破坏 content 字段内容
        using (JsonDocument doc = JsonDocument.Parse(str))
        {
            JsonElement root = doc.RootElement;

            // 提取各个字段
            string sign = root.GetProperty("sign").GetString() ?? "";
            string serialNo = root.GetProperty("serialNo").GetString() ?? "";
            string encryptedContent = root.GetProperty("content").GetString() ?? "";

            // 构建验签字符串（按字段名排序，排除 sign 和 serialNo）
            StringBuilder signContent = new StringBuilder();
            List<string> keys = new List<string>();

            foreach (JsonProperty property in root.EnumerateObject())
            {
                if (property.Name != "sign" && property.Name != "serialNo")
                {
                    keys.Add(property.Name);
                }
            }
            keys.Sort();

            foreach (string key in keys)
            {
                signContent.Append($"{key}={root.GetProperty(key).GetString()}&");
            }
            signContent.Append($"serialNo={serialNo}");

            // 验签
            bool res = RSA.verify(signContent.ToString(), sign, ptpublickey, "UTF-8");
            Console.WriteLine($"验签结果: {res}");

            if (res)
            {
                // 解密 content
                string decryptedContent = EncryptDes.Decrypt3Des(encryptedContent, deskey);

                // 使用 JsonSerializer 构建返回结果，保证正确的 JSON 格式
                var options = new System.Text.Json.JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = false
                };

                var resultDict = new Dictionary<string, object>();
                foreach (JsonProperty property in root.EnumerateObject())
                {
                    if (property.Name == "content")
                    {
                        // 尝试将 content 解析为 JSON 对象，如果失败则作为字符串处理
                        try
                        {
                            using (JsonDocument contentDoc = JsonDocument.Parse(decryptedContent))
                            {
                                resultDict[property.Name] = contentDoc.RootElement.Clone();
                            }
                        }
                        catch
                        {
                            // 如果不是有效的 JSON，就作为字符串保存
                            resultDict[property.Name] = decryptedContent;
                        }
                    }
                    else
                    {
                        resultDict[property.Name] = property.Value.GetString() ?? "";
                    }
                }

                return System.Text.Json.JsonSerializer.Serialize(resultDict, options);
            }
            else
            {
                return "验签失败";
            }
        }
    }
    /**
     * 随机数生成
     */
    private static string GenerateCheckCode(int codeCount)
    {
        int rep = 0;
        string str = string.Empty;
        long num2 = DateTime.Now.Ticks + rep;
        rep++;
        Random random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> rep)));
        for (int i = 0; i < codeCount; i++)
        {
            char ch;
            int num = random.Next();
            if ((num % 2) == 0)
            {
                ch = (char)(0x30 + ((ushort)(num % 10)));
            }
            else
            {
                ch = (char)(0x41 + ((ushort)(num % 0x1a)));
            }
            str = str + ch.ToString();
        }
        return str;
    }
}