using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Web;
using AIChatApp.Model;

namespace AIChatApp.Shared;

public class BackendClient(HttpClient http)
{
    public async IAsyncEnumerable<string> ChatAsync(ChatRequest request)//, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/chat/stream")
        {
            Content = JsonContent.Create(request),
        };
        var response = await http.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead);//, cancellationToken);
        var stream = await response.Content.ReadAsStreamAsync();//cancellationToken);

        await foreach (var item in JsonSerializer.DeserializeAsyncEnumerable<string>(stream))//, cancellationToken: cancellationToken))
        {
            if (item is not null)
            {
                yield return item;
            }
        }
    }
}
