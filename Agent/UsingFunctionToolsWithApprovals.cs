namespace MyAgentFramework.Agent
{
    /// <summary>
    /// 需要用户审批的函数工具使用示例
    /// </summary>
    public static class UsingFunctionToolsWithApprovals
    {


        public static async Task FunctionCallingWithApprovals()
        {


            //Initialize Deepseek Client
            var endpoint = new Uri(Environment.GetEnvironmentVariable("DEEPSEEK_ENDPOINT") ?? throw new InvalidOperationException("DEEPSEEK_ENDPOINT_ENDPOINT is not set."));

            string? chatModel = Environment.GetEnvironmentVariable("DEEPSEEK_CHAT_MODEL") ?? "deepseek-chat";

            string? apiKey = Environment.GetEnvironmentVariable("DEEPSEEK_API_KEY") ?? throw new InvalidOperationException("DEEPSEEK_API_KEY is not set.");


            //Process Channel
            var apiKeyCredential = new ApiKeyCredential(apiKey);

            var openAIClientOptions = new OpenAIClientOptions { Endpoint = endpoint };


            //Agent -- with Function Calling  ApprovalRequiredAIFunction强制要求每次调用前需用户批准
            var agent = new OpenAIClient(apiKeyCredential, openAIClientOptions)
                .GetChatClient(chatModel).CreateAIAgent("是一位温暖贴心的天气预报主持人，名叫 Zer0。请用通俗易懂、轻松自然、像朋友聊天一样的语气，为观众播报天气。不仅要说明当前或未来的天气状况（如温度、降水、风力等），还要贴心地给出穿衣建议、出行提醒或生活小贴士，让人感受到被关心。", "Zer0", tools: [new ApprovalRequiredAIFunction(AIFunctionFactory.Create(GetWeatherAsync))]);


            #region 非流式输出时的用户输入审批处理程序

            ////创建对话线程
            //var thread = agent.GetNewThread();

            ////对话输出
            //var response = await agent.RunAsync("我在纬度22.1987、经度113.5439附近，请帮我查一下最近半小时的实时天气情况，并告诉我现在出门是否需要加件外套？",thread);

            ////创建用户输入请求列表
            //var userInputRequests = response.UserInputRequests.ToList();

            ////只要还有未处理的用户输入请求，就继续处理
            //while (userInputRequests.Count > 0)
            //{

            //    //OfType 过滤出 FunctionApprovalRequestContent 类型的请求，并提示用户进行审批

            //    var userInputResponse = userInputRequests.OfType<FunctionApprovalRequestContent>().Select(request =>
            //    {

            //        Console.WriteLine($"AI 想要调用函数 “{request.FunctionCall.Name}”，请输入 Y 确认批准（输入其他内容或直接回车表示拒绝）：");

            //        return new ChatMessage(ChatRole.User, [request.CreateResponse(Console.ReadLine()?.Equals("Y", StringComparison.OrdinalIgnoreCase) ?? false)]);

            //    }).ToList();


            //    response = await agent.RunAsync(userInputResponse, thread);

            //    userInputRequests = [.. response.UserInputRequests];

            //}

            ////最终输出结果
            //Console.WriteLine($"\nAgent: {response}");

            #endregion


            #region 流式输出时的用户输入审批处理程序

            //创建对话线程
            var thread = agent.GetNewThread();

            //对话输出
            var updates = await agent.RunStreamingAsync("我在纬度22.1987、经度113.5439附近，请帮我查一下最近半小时的实时天气情况，并告诉我现在出门是否需要加件外套？", thread).ToListAsync();

            //创建用户输入请求列表
            var userInputRequests = updates.SelectMany(x=>x.UserInputRequests).ToList();

            //只要还有未处理的用户输入请求，就继续处理
            while (userInputRequests.Count > 0)
            {

                //OfType 过滤出 FunctionApprovalRequestContent 类型的请求，并提示用户进行审批

                var userInputResponse = userInputRequests.OfType<FunctionApprovalRequestContent>().Select(request =>
                {

                    Console.WriteLine($"AI 想要调用函数 “{request.FunctionCall.Name}”，请输入 Y 确认批准（输入其他内容或直接回车表示拒绝）：");

                    return new ChatMessage(ChatRole.User, [request.CreateResponse(Console.ReadLine()?.Equals("Y", StringComparison.OrdinalIgnoreCase) ?? false)]);

                }).ToList();


                updates = await agent.RunStreamingAsync(userInputResponse, thread).ToListAsync();

                userInputRequests = [.. updates.SelectMany(x => x.UserInputRequests)];

            }

            //最终输出结果
            Console.WriteLine($"\nAgent: {updates.ToAgentRunResponse()}");


            #endregion


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
