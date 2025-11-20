//using OpenTelemetry.Trace;
//namespace MyAgentFramework.Agent
//{
//    /// <summary>
//    /// 可观测性，遥测
//    /// </summary>
//    public static class Observability
//    {
//        public static async Task ObservabilityFucntion()
//        {

//            //Initialize Deepseek Client
//            var endpoint = new Uri(Environment.GetEnvironmentVariable("DEEPSEEK_ENDPOINT") ?? throw new InvalidOperationException("DEEPSEEK_ENDPOINT_ENDPOINT is not set."));

//            string? chatModel = Environment.GetEnvironmentVariable("DEEPSEEK_CHAT_MODEL") ?? "deepseek-chat";

//            string? apiKey = Environment.GetEnvironmentVariable("DEEPSEEK_API_KEY") ?? throw new InvalidOperationException("DEEPSEEK_API_KEY is not set.");


//            //Process Channel
//            var apiKeyCredential = new ApiKeyCredential(apiKey);

//            var openAIClientOptions = new OpenAIClientOptions { Endpoint = endpoint };


//            string sourceName = Guid.NewGuid().ToString("N");

//            //OpenTelemetry
//            var tracerProviderBuilder = Sdk.CreateTracerProviderBuilder().AddSource(sourceName);



//            //Agent
//            var agent = new OpenAIClient(apiKeyCredential, openAIClientOptions)
//                .GetChatClient(chatModel).CreateAIAgent("你是一位超算力人工智能，你将帮助我完成各种任务", "Zer0");




//        }
//    }
//}
