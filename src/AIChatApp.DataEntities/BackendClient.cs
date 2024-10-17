using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Web;
using AIChatApp.Model;

namespace AIChatApp.Shared;

public class BackendClient(HttpClient http)
{
    public async IAsyncEnumerable<string> ChatAsync(ChatRequest request)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/chat/stream")
        {
            Content = JsonContent.Create(request),
        };
        var response = await http.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead);
        var stream = await response.Content.ReadAsStreamAsync();

        await foreach (var item in JsonSerializer.DeserializeAsyncEnumerable<string>(stream))
        {
            if (item is not null)
            {
                yield return item;
            }
        }
    }
}
