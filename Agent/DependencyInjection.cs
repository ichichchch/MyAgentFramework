namespace MyAgentFramework.Agent
{

    /// <summary>
    /// 依赖注入
    /// </summary>
    public static class DependencyInjection
    {
        public static async Task DIFunction()
        {


            //Initialize Deepseek Client
            var endpoint = new Uri(Environment.GetEnvironmentVariable("DEEPSEEK_ENDPOINT") ?? throw new InvalidOperationException("DEEPSEEK_ENDPOINT_ENDPOINT is not set."));

            string? chatModel = Environment.GetEnvironmentVariable("DEEPSEEK_CHAT_MODEL") ?? "deepseek-chat";

            string? apiKey = Environment.GetEnvironmentVariable("DEEPSEEK_API_KEY") ?? throw new InvalidOperationException("DEEPSEEK_API_KEY is not set.");


            //Process Channel
            var apiKeyCredential = new ApiKeyCredential(apiKey);

            var openAIClientOptions = new OpenAIClientOptions { Endpoint = endpoint};


            //Host Builder
            var builder = Host.CreateApplicationBuilder();

            //单例注入Options
            builder.Services.AddSingleton(new ChatClientAgentOptions {Instructions = "你是一名很温柔的心理医生",Name="Zer0" });


            //注入键控服务
            builder.Services.AddKeyedChatClient("OpenAI", (sp) => new OpenAIClient(apiKeyCredential,openAIClientOptions).GetChatClient(chatModel).AsIChatClient());





            



        }
    }
}
