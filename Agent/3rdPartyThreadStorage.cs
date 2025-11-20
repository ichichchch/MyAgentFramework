namespace MyAgentFramework.Agent
{

    /// <summary>
    /// 用于存储 AI 对话线程的第三方（外部）存储服务的私有引用或实现
    /// </summary>
    public static class ThreePartyThreadStorage
    {
        public  static async Task StorageFunction()
        {

            //Initialize Deepseek Client
            var endpoint = new Uri(Environment.GetEnvironmentVariable("DEEPSEEK_ENDPOINT") ?? throw new InvalidOperationException("DEEPSEEK_ENDPOINT_ENDPOINT is not set."));

            string? chatModel = Environment.GetEnvironmentVariable("DEEPSEEK_CHAT_MODEL") ?? "deepseek-chat";

            string? apiKey = Environment.GetEnvironmentVariable("DEEPSEEK_API_KEY") ?? throw new InvalidOperationException("DEEPSEEK_API_KEY is not set.");


            //Process Channel
            var apiKeyCredential = new ApiKeyCredential(apiKey);

            var openAIClientOptions = new OpenAIClientOptions { Endpoint = endpoint };

            //Vector Store 向量存储
            var vectorStore = new InMemoryVectorStore();

            //Agent Options
            var agentOptions = new ChatClientAgentOptions
            {
                Instructions = "你是一位抚慰型温柔的心理专家，你将辅助我回答各种问题",
                Name = "ZZER!0",
                ChatMessageStoreFactory = fac =>
                {
                    return new VectorChatMessageStore(vectorStore, fac.SerializedState, fac.JsonSerializerOptions);
                }
            };

            //Agent
            var agent = new OpenAIClient(apiKeyCredential, openAIClientOptions)
                .GetChatClient(chatModel).CreateAIAgent(agentOptions);


            //Thread
            var thread = agent.GetNewThread();


            Console.WriteLine(await agent.RunAsync("请为我讲一个笑话",thread));


            var serializedThread = thread.Serialize();


            Console.WriteLine("\n--- Serialized thread ---\n");
            Console.WriteLine(JsonSerializer.Serialize(serializedThread, new JsonSerializerOptions { WriteIndented = true }));


            var resumedThread = agent.DeserializeThread(serializedThread);

            Console.WriteLine(await agent.RunAsync("再讲一个笑话",resumedThread));

            var messageStore = resumedThread.GetService<VectorChatMessageStore>();

            Console.WriteLine(); Console.WriteLine($"\nThread is stored in vector store under key: {messageStore?.ThreadDbKey}");


        }

    }

}
