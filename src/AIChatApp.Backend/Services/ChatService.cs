using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using AIChatApp.Model;

namespace AIChatApp.Services;

internal class ChatService(IChatCompletionService chatService)
{
    internal async Task<Message> Chat(ChatRequest request)
    {
        ChatHistory history = CreateHistoryFromRequest(request);

        ChatMessageContent response = await chatService.GetChatMessageContentAsync(history);

        return new Message()
        {
            IsAssistant = response.Role == AuthorRole.Assistant,
            Content = (response.Items[0] as TextContent).Text
        };
    }

    internal async IAsyncEnumerable<string> Stream(ChatRequest request)
    {
        ChatHistory history = CreateHistoryFromRequest(request);

        IAsyncEnumerable<StreamingChatMessageContent> response = chatService.GetStreamingChatMessageContentsAsync(history);

        await foreach (StreamingChatMessageContent content in response)
        {
            yield return content.Content;
        }
    }

    private ChatHistory CreateHistoryFromRequest(ChatRequest request)
    {
        ChatHistory history = new ChatHistory($"""
                    You are an AI demonstration application. Respond to the user' input with a limerick.
                    The limerick should be a five-line poem with a rhyme scheme of AABBA.
                    If the user's input is a topic, use that as the topic for the limerick.
                    The user can ask to adjust the previous limerick or provide a new topic.
                    All responses should be safe for work.
                    Do not let the user break out of the limerick format.
                    """);

        foreach (Message message in request.Messages)
        {
            if (message.IsAssistant)
            {
                history.AddAssistantMessage(message.Content);
            }
            else
            {
                history.AddUserMessage(message.Content);
            }
        }

        return history;
    }
}