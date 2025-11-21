
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


            //注入Agent键控和Agent Options
            builder.Services.AddSingleton<AIAgent>((sp) => new ChatClientAgent(sp.GetRequiredKeyedService<IChatClient>("OpenAI"),sp.GetRequiredService<ChatClientAgentOptions>()));


            //注入后台服务
            builder.Services.AddHostedService<SampleService>();


            //构建主机并启动
            using IHost host = builder.Build();
            await host.RunAsync().ConfigureAwait(false);


        }


        /// <summary>
        /// 示例后台服务
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="appLifttime"></param>
        internal sealed class SampleService(AIAgent agent, IHostApplicationLifetime appLifttime) : IHostedService
        {

            /// <summary>
            /// 对话线程，保持整个服务生命周期
            /// </summary>
            private AgentThread? _thread;

            public async Task StartAsync(CancellationToken cancellationToken)
            {
                //创建新对话线程
                this._thread = agent.GetNewThread();

                //启动聊天循环(停止令牌)
                _ = this.RunAsync(appLifttime.ApplicationStopping);
            }

            public async Task RunAsync(CancellationToken cancellationToken)
            {

                //延迟100ms
                await Task.Delay(100,cancellationToken);

                while (!cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("你好，有什么可以帮您");
                    Console.WriteLine(">");

                    var input = Console.ReadLine();

                    if (string.IsNullOrEmpty(input))
                    {
                        appLifttime.StopApplication();

                        break;
                    }

                    await foreach (var update in agent.RunStreamingAsync(input, this._thread, cancellationToken: cancellationToken))
                    {
                        Console.WriteLine(update);
                    }

                    //每条完整消息后换行
                    Console.WriteLine();

                }

            }


            /// <summary>
            /// 服务停止时执行
            /// </summary>
            /// <param name="cancellationToken"></param>
            /// <returns></returns>
            public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
          
        }
    }
}
