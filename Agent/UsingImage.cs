namespace MyAgentFramework.Agent
{

    /// <summary>
    /// 使用图片
    /// </summary>
    public static class UsingImage
    {
        public static async Task ImageFunction()
        {

            //Initialize Deepseek Client
            string? chatModel = Environment.GetEnvironmentVariable("OPENAI_CHAT_MODEL") ?? "gpt-4o-mini";

            string? apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? throw new InvalidOperationException("OPENAI_API_KEY is not set.");


            //Process Channel



            //Agent
            var agent = new OpenAIClient(apiKey).GetChatClient(chatModel).CreateAIAgent("你是一名忠实麦当劳用户","maimai");


            //ChatMessage
            var message = new ChatMessage(ChatRole.User, [new TextContent("在这张照片中你看到了什么"), new UriContent("https://picsum.photos/200/300", "image/jpeg")]);


            //对话线程
            var thread = agent.GetNewThread();


            //流式输出响应
            await foreach(var update in agent.RunStreamingAsync(message, thread))
            {
                Console.WriteLine(update);
            }

        }
    }
}
