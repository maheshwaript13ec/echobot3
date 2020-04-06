using EchoBotApp.Bots;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class Startup
{
    // Inject the IHostingEnvironment into constructor
    public Startup(IHostingEnvironment env)
    {
        // Set the root path
        ContentRootPath = env.ContentRootPath;
    }

    // Track the root path so that it can be used to setup the app configuration
    public string ContentRootPath { get; private set; }

    public void ConfigureServices(IServiceCollection services)
    {
        // Set up the service configuration
        var builder = new ConfigurationBuilder()
            .SetBasePath(ContentRootPath)
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables();
        var configuration = builder.Build();
        services.AddSingleton(configuration);

        services.AddSingleton(sp =>
        {
            var qnaService = new QnAMaker(new QnAMakerEndpoint()
            {
                // get these details from qnamaker.ai
                // https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-qna?view=azure-bot-service-4.0&tabs=cs
                KnowledgeBaseId = "e73c13c4-6d4c-4725-9b62-562258da13fc",
                Host = "https://chatbothand.azurewebsites.net/qnamaker",
                EndpointKey = "cd9e83a7-bbe1-4f99-b947-68d0a715a123"
            });

            return qnaService;
        });

        // Add your SimpleBot to your application
        services.AddBot<SimpleBot>(options =>
        {
            options.CredentialProvider = new ConfigurationCredentialProvider(configuration);
        });


    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        app.UseStaticFiles();

        // Tell your application to use Bot Framework
        app.UseBotFramework();
    }
}