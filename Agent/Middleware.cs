namespace MyAgentFramework.Agent
{
    public static class Middleware
    {

        public static async Task Function()
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
                .GetChatClient(chatModel).CreateAIAgent("你是一位超算力人工智能，你将帮助我完成各种任务", "Zer0");




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
