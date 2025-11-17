namespace MyAgentFramework.Agent
{


    /// <summary>
    /// 结构化输出
    /// </summary>
    public static class StructuredOutput
    {
        public static async Task StructureFunction()
        {

            //Initialize Deepseek Client
            var endpoint = new Uri(Environment.GetEnvironmentVariable("QWEN_ENDPOINT") ?? throw new InvalidOperationException("DEEPSEEK_ENDPOINT_ENDPOINT is not set."));

            string? chatModel = Environment.GetEnvironmentVariable("QWEN_CHAT_MODEL") ?? "deepseek-chat";

            string? apiKey = Environment.GetEnvironmentVariable("QWEN_API_KEY") ?? throw new InvalidOperationException("DEEPSEEK_API_KEY is not set.");


            //Process Channel
            var apiKeyCredential = new ApiKeyCredential(apiKey);

            var openAIClientOptions = new OpenAIClientOptions { Endpoint = endpoint };


            //ChatClient
            var chatClient = new OpenAIClient(apiKeyCredential, openAIClientOptions).GetChatClient(chatModel);


            //Agent01 - 非流式輸出
            var agent01 = chatClient.CreateAIAgent("你是一个乐于助人的助手，擅长将信息组织成结构化格式。", "StructAgent");


            //定义输出格式并输出
            //向模型提示：“请以JSON格式输出，符合Person结构”
            var response = await agent01.RunAsync<Person>("请以JSON格式输出：请帮我介绍一个叫Alice的30岁软件工程师，她喜欢旅行和摄影。");


            //打印结构化输出结果
            Console.WriteLine("Assistant Output:");
            Console.WriteLine($"Name: {response.Result.Name}");
            Console.WriteLine($"Age: {response.Result.Age}");
            Console.WriteLine($"Occupation: {response.Result.Occupation}");



            //Agent02 - 預定義響應格式 流式輸出
            var agent02 = chatClient.CreateAIAgent(new ChatClientAgentOptions(name:"HelpfulAssistant",instructions: "你总是以严格的 JSON 格式响应，仅包含所需字段，不要任何额外文本。")
            {
                ChatOptions = new()
                {
                    ResponseFormat = ChatResponseFormat.ForJsonSchema<Person>()
                }
            });

            //定义输出格式并流式输出
            var updates = agent02.RunStreamingAsync("请帮我介绍一个叫Bob的25岁平面设计师，他喜欢音乐和绘画。");

            //等待流式输出完成并反序列化为Person对象
            var personInfo = (await updates.ToAgentRunResponseAsync()).Deserialize<Person>(JsonSerializerOptions.Web);

            //打印结构化输出结果
            Console.WriteLine("Assistant Output:");
            Console.WriteLine($"Name: {personInfo.Name}");
            Console.WriteLine($"Age: {personInfo.Age}");
            Console.WriteLine($"Occupation: {personInfo.Occupation}");


        }
    }











    /// <summary>
    /// 结构化输出示例类
    /// </summary>
    [Description("A person with a name, age, and occupation.")]
    public class Person
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("age")]
        public int? Age { get; set; }

        [JsonPropertyName("occupation")]
        public string? Occupation { get; set; }
    }


}
