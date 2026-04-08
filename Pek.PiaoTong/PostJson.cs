using System.Net;
using System.Text;

namespace Pek.PiaoTong;

public class PostJson
{
    private static readonly HttpClient _httpClient = new HttpClient();

    /**
     * Json的请求头 post请求地址
     */
    public static string Post4Json(string url, string buildRequest)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(buildRequest, Encoding.UTF8, "application/json")
        };

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        using var response = _httpClient.Send(request, cts.Token);
        response.EnsureSuccessStatusCode();
        return response.Content.ReadAsStringAsync(cts.Token).GetAwaiter().GetResult();
    }
}