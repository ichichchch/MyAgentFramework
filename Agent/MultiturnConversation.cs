namespace MyAgentFramework.Agent
{
    public static class MultiturnConversation
    {

        /// <summary>
        /// 多轮对话
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task MultConversationAsync()
        {


            //Initialize Deepseek Client
            var endpoint = new Uri(Environment.GetEnvironmentVariable("DEEPSEEK_ENDPOINT") ?? throw new InvalidOperationException("DEEPSEEK_ENDPOINT_ENDPOINT is not set."));

            string? chatModel = Environment.GetEnvironmentVariable("DEEPSEEK_CHAT_MODEL") ?? "deepseek-chat";

            string? apiKey = Environment.GetEnvironmentVariable("DEEPSEEK_API_KEY") ?? throw new InvalidOperationException("DEEPSEEK_API_KEY is not set.");


            //Process Channel
            var apiKeyCredential = new ApiKeyCredential(apiKey);

            var openAIClientOptions = new OpenAIClientOptions { Endpoint = endpoint };


            //Agent
            var agent = new OpenAIClient(apiKeyCredential, openAIClientOptions)
                .GetChatClient(chatModel).CreateAIAgent("你是一位善解人意的抚慰型机器人", "Zer0");

            //对话线程
            var thread = agent.GetNewThread();


            //非流式输出
            Console.WriteLine(await agent.RunAsync("我好困，你困不困",thread));
            Console.WriteLine(await agent.RunAsync("快下班了，牛批又活一周", thread));


            //对话线程
            var streamThread = agent.GetNewThread();

            //流式输出
            await foreach (var responese in agent.RunStreamingAsync("给我讲个笑话吧，程序员笑话", streamThread))
            {
                Console.WriteLine(responese);
            }

            await foreach(var responese in agent.RunStreamingAsync("再讲一个吧", streamThread))
            {
                Console.WriteLine(responese);
            }


        }
    }
}
