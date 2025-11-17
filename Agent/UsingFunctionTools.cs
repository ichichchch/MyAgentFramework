namespace MyAgentFramework.Agent
{

    /// <summary>
    /// 使用函数工具
    /// </summary>
    public static class UsingFunctionTools
    {
        public static async Task FunctionCalling()
        {

            //Initialize Deepseek Client
            var endpoint = new Uri(Environment.GetEnvironmentVariable("DEEPSEEK_ENDPOINT") ?? throw new InvalidOperationException("DEEPSEEK_ENDPOINT_ENDPOINT is not set."));

            string? chatModel = Environment.GetEnvironmentVariable("DEEPSEEK_CHAT_MODEL") ?? "deepseek-chat";

            string? apiKey = Environment.GetEnvironmentVariable("DEEPSEEK_API_KEY") ?? throw new InvalidOperationException("DEEPSEEK_API_KEY is not set.");


            //Process Channel
            var apiKeyCredential = new ApiKeyCredential(apiKey);

            var openAIClientOptions = new OpenAIClientOptions { Endpoint = endpoint };


            //Agent -- with Function Calling
            var agent = new OpenAIClient(apiKeyCredential, openAIClientOptions)
                .GetChatClient(chatModel).CreateAIAgent("是一位温暖贴心的天气预报主持人，名叫 Zer0。请用通俗易懂、轻松自然、像朋友聊天一样的语气，为观众播报天气。不仅要说明当前或未来的天气状况（如温度、降水、风力等），还要贴心地给出穿衣建议、出行提醒或生活小贴士，让人感受到被关心。", "Zer0", tools:[AIFunctionFactory.Create(GetWeatherAsync)]);


            //非流式输出
            Console.WriteLine(await agent.RunAsync("我在纬度22.1987、经度113.5439附近，请帮我查一下最近半小时的实时天气情况，并告诉我现在出门是否需要加件外套？"));


            //流式输出
            await foreach (var responese in agent.RunStreamingAsync("明天在纬度22.1987、经度113.5439会是什么天气？请告诉我预计的气温范围，并建议我是否需要多穿点衣服。"))
            {
                Console.WriteLine(responese);
            }



        }




        /// <summary>
        /// Weather Function
        /// </summary>
        /// <returns></returns>
        [Description("获取当地的天气情况")]
        private static async Task<string> GetWeatherAsync([Description("获取天气信息的位置")] string location)
        {

            using var httpClient = new HttpClient();

            string? weatherUrl = $"http://api.weatherapi.com/v1/current.json?key=c0013f0c23944d5384913054251410&q={location}&aqi=no";


            Console.WriteLine("已调用函数工具");


            return await httpClient.GetStringAsync(weatherUrl);

        }

    }
}
