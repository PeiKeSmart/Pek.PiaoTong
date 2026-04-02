using System.Text;

namespace Pek.PiaoTong;

public class PostJson
{
    /**
     * Json的请求头 post请求地址
     */
    public static string Post4Json(string url, string buildRequest)
    {
        string result = "";
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "POST";
        request.Timeout = 5000;
        request.ContentType = "application/json";
        byte[] byte4builde = Encoding.UTF8.GetBytes(buildRequest);
        request.ContentLength = byte4builde.Length;
        using (Stream reqStream = request.GetRequestStream())
        {
            reqStream.Write(byte4builde, 0, byte4builde.Length);
            reqStream.Close();
        }

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        Stream stream = response.GetResponseStream();
        //获得响应内容
        using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
        {
            result = reader.ReadToEnd();
        }
        return result;
    }
}