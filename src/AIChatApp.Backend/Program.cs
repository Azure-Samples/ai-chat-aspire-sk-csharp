using AIChatApp.Model;
using AIChatApp.Services;
using Aspire.Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add the Azure OpenAI client configured in AppHost
builder.AddAzureOpenAIClient("openai");

// Add Semantic Kernel. This will use the OpenAI client configured in the above line
builder.Services.AddKernel();

var chatDeploymentName = builder.Configuration["AI_ChatDeploymentName"] ?? "chat";
builder.Services.AddKernel()
    .AddAzureOpenAIChatCompletion(chatDeploymentName);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<ChatService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Configure APIs for chat related features
// Uncomment for a non-streaming response
//app.MapPost("/api/chat", (ChatRequest request, ChatHandler chatHandler) => (chatHandler.); 
//  .WithName("Chat")
//  .WithOpenApi();
app.MapPost("/api/chat/stream", (ChatRequest request, ChatService chatHandler) => chatHandler.Stream(request))
    .WithName("StreamingChat")
    .WithOpenApi();

app.Run();
