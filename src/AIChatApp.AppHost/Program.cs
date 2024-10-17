using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// Add support for a local configuration file, which doesn't get committed to source control
builder.Configuration.Sources.Insert(0, new JsonConfigurationSource { Path = "appsettings.Local.json", Optional = true });

var chatDeploymentName = "chat";
var connectionString = builder.Configuration.GetConnectionString("openai");
var openai = String.IsNullOrEmpty(connectionString)
    ? builder.AddAzureOpenAI("openai")
         .AddDeployment(new AzureOpenAIDeployment(chatDeploymentName, "gpt-4o", "2024-05-13", "GlobalStandard", 10))
    : builder.AddConnectionString("openai", "OPENAI_CONNECTION_STRING");

builder.AddProject<AIChatApp_Web>("aichatapp-web")
    .WithReference(openai)
    .WithEnvironment("AI_ChatDeploymentName", chatDeploymentName);

builder.Build().Run();
