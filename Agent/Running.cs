namespace MyAgentFramework.Agent
{
    public static class Running
    {
        public static async Task RunningAsync()
        {


            //Initialize Deepseek Client
            var endpoint = new Uri(Environment.GetEnvironmentVariable("DEEPSEEK_ENDPOINT") ?? throw new InvalidOperationException("DEEPSEEK_ENDPOINT_ENDPOINT is not set."));

            string? chatModel = Environment.GetEnvironmentVariable("DEEPSEEK_CHATMODEL") ?? "deepseek-chat";

            string? apiKey = Environment.GetEnvironmentVariable("DEEPSEEK_APIKEY") ?? throw new InvalidOperationException("DEEPSEEK_API_KEY is not set.");


            //Process Channel
            var apiKeyCredential = new ApiKeyCredential(apiKey);

            var openAIClientOptions = new OpenAIClientOptions { Endpoint = endpoint };


            //Agent
            var agent  = new OpenAIClient(apiKeyCredential,openAIClientOptions)
                .GetChatClient(chatModel).CreateAIAgent("你是一位超算力人工智能，你将帮助我完成各种任务","Zer0");


            //非流式输出
            Console.WriteLine(await agent.RunAsync("你好请自我介绍"));


            //流式输出
            await foreach(var response in agent.RunStreamingAsync("你好，你时常感到头晕，为什么？"))
            {
                Console.WriteLine(response);
            }


        }
    }
}
