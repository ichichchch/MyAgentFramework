namespace MyAgentFramework.Agent
{

    /// <summary>
    /// 将对话线程（thread）序列化到磁盘再恢复
    /// </summary>
    public static class PersistedConversations
    {
        public static async Task PersistedConversationsFunction()
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
            // 每个线程代表一次独立的对话会话，内部会维护消息历史。
            var thread = agent.GetNewThread();


            //输出
            Console.WriteLine(await agent.RunAsync("我好困，你困不困", thread));


            // 将当前对话线程的状态（包括所有历史消息）序列化为一个 JsonElement 对象；
            // JsonElement 是 .NET 中表示不可变 JSON 数据的类型，便于后续存储或传输。
            JsonElement serializedThread = thread.Serialize();


            // 安全生成临时文件路径
            string tempDirectory = Path.GetTempPath();   
            string randomFileName = Path.GetRandomFileName();  
            string filePath = Path.Combine(tempDirectory, randomFileName);


            // 将序列化后的线程数据（JSON 格式）异步写入临时文件；
            // 使用 JsonSerializer.Serialize 将 JsonElement 转为 JSON 字符串并保存。
            await File.WriteAllTextAsync(filePath,JsonSerializer.Serialize(serializedThread));

            // 从临时文件中异步读取 JSON 字符串，并反序列化为 JsonElement 对象；
            // 这模拟了从磁盘、数据库或网络加载已保存的对话状态。
            JsonElement reloadedSerializedThread = JsonSerializer.Deserialize<JsonElement>(await File.ReadAllTextAsync(filePath));


            // 使用代理的 DeserializeThread 方法，将加载的 JSON 数据反序列化为一个新的 AgentThread 对象；
            var resumedThread = agent.DeserializeThread(reloadedSerializedThread);


            // 继续对话：向恢复后的线程发送新消息；
            Console.WriteLine(await agent.RunAsync("你不困你给我讲个笑话", resumedThread));

            //对话完成后，删除临时文件
            if(File.Exists(filePath)) File.Delete(filePath);


        }
    }
}
