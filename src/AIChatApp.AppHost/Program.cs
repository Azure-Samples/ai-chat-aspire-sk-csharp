using Microsoft.Extensions.Configuration.Json;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// Add support for a local configuration file, which doesn't get committed to source control
builder.Configuration.Sources.Insert(0, new JsonConfigurationSource { Path = "appsettings.Local.json", Optional = true });

var chatDeploymentName = "chat";
var openai = builder.ExecutionContext.IsPublishMode 
    ? builder.AddAzureOpenAI("openai")
         .AddDeployment(new AzureOpenAIDeployment(chatDeploymentName, "gpt-4o", "2024-05-13", "GlobalStandard", 10))
    : builder.AddConnectionString("openai");

builder.AddProject<AIChatApp_Backend>("aichatapp-backend")
    .WithReference(openai)
    .WithEnvironment("AI_ChatDeploymentName", chatDeploymentName);

builder.AddProject<AIChatApp_Web>("aichatapp-web");

builder.Build().Run();
