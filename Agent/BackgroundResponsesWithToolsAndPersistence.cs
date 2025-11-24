namespace MyAgentFramework.Agent
{

    /// <summary>
    /// 处理长时间运行操作的后台响应（background responses）
    /// 通过延续令牌（continuation tokens）轮询操作状态
    /// 在轮询周期间持久化/恢复代理状态
    /// </summary>
    public static class BackgroundResponsesWithToolsAndPersistence
    {

        //模拟持久化存储
        private static readonly Dictionary<string, JsonElement?> stateStore = [];

        public static async Task Function()
        {

            //Initialize Deepseek Client
            var endpoint = new Uri(Environment.GetEnvironmentVariable("DEEPSEEK_ENDPOINT") ?? throw new InvalidOperationException("DEEPSEEK_ENDPOINT_ENDPOINT is not set."));

            string? chatModel = Environment.GetEnvironmentVariable("DEEPSEEK_CHAT_MODEL") ?? "deepseek-chat";

            string? apiKey = Environment.GetEnvironmentVariable("DEEPSEEK_API_KEY") ?? throw new InvalidOperationException("DEEPSEEK_API_KEY is not set.");


            //Process Channel
            var apiKeyCredential = new ApiKeyCredential(apiKey);

            var openAIClientOptions = new OpenAIClientOptions { Endpoint = endpoint };


            //AgentOptions
            string instructions = "你是一位太空题材小说作家。在创作小说之前，务必先研究相关事实，并为主角们生成人物档案。请完整地撰写章节，无需征求批准或反馈。不要询问用户关于语气、风格、节奏或格式的偏好——只需根据请求直接创作小说即可。";

            var researchSpaceFactsAsync = AIFunctionFactory.Create(ResearchSpaceFactsAsync);

            var generateCharacterProfilesAsync = AIFunctionFactory.Create(GenerateCharacterProfilesAsync);

            var agentOptions = new ChatClientAgentOptions { Instructions = instructions, Name = "SpaceNovelWriter", ChatOptions = new() { Tools = [researchSpaceFactsAsync,generateCharacterProfilesAsync] } };

            //Agent
            var agent = new OpenAIClient(apiKeyCredential, openAIClientOptions).GetChatClient(chatModel).CreateAIAgent(agentOptions);


            //AgentRunOptions 启用后台响应模式(仅Azure OpenAI支持)
            AgentRunOptions options = new() { AllowBackgroundResponses = true };


            //对话线程
            var thread = agent.GetNewThread();


            var response = await agent.RunAsync("写一部长篇小说，讲述一支宇航员小队探索一片未知星系的故事。", thread, options);


            //轮询逻辑
            while (response.ContinuationToken is not null)
            {

                PersistAgentState(thread, response.ContinuationToken);

                await Task.Delay(TimeSpan.FromSeconds(10));

                RestoreAgentState(agent, out thread, out object? continuationToken);

                options.ContinuationToken = continuationToken;

                response = await agent.RunAsync(thread, options);
            }

            Console.WriteLine(response.Text);

        }


        /// <summary>
        /// 状态持久化方法
        /// </summary>
        /// <param name="thread"></param>
        /// <param name="continuationToken"></param>
        static void PersistAgentState(AgentThread thread, object? continuationToken)
        {
            stateStore["thread"] = thread.Serialize();
            stateStore["continuationToken"] = JsonSerializer.SerializeToElement(continuationToken, AgentAbstractionsJsonUtilities.DefaultOptions.GetTypeInfo(typeof(ResponseContinuationToken)));
        }

        /// <summary>
        /// 恢复状态方法
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="thread"></param>
        /// <param name="continuationToken"></param>
        /// <exception cref="InvalidOperationException"></exception>
        static void RestoreAgentState(AIAgent agent, out AgentThread thread, out object? continuationToken)
        {
            JsonElement serializedThread = stateStore["thread"] ?? throw new InvalidOperationException("No serialized thread found in state store.");
            JsonElement? serializedToken = stateStore["continuationToken"];

            thread = agent.DeserializeThread(serializedThread);
            continuationToken = serializedToken?.Deserialize(AgentAbstractionsJsonUtilities.DefaultOptions.GetTypeInfo(typeof(ResponseContinuationToken)));
        }



        /// <summary>
        /// 为小说生成科学事实依据
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        [Description("Researches relevant space facts and scientific information for writing a science fiction novel")]
        static async Task<string> ResearchSpaceFactsAsync(string topic)
        {

            Console.WriteLine($"[ResearchSpaceFacts] Researching topic: {topic}");

            // 模拟10秒耗时操作
            await Task.Delay(TimeSpan.FromSeconds(10));


            string result = topic.ToUpperInvariant() switch
            {
                var t when t.Contains("GALAXY") => "Research findings: Galaxies contain billions of stars. Uncharted galaxies may have unique stellar formations, exotic matter, and unexplored phenomena like dark energy concentrations.",
                var t when t.Contains("SPACE") || t.Contains("TRAVEL") => "Research findings: Interstellar travel requires advanced propulsion systems. Challenges include radiation exposure, life support, and navigation through unknown space.",
                var t when t.Contains("ASTRONAUT") => "Research findings: Astronauts undergo rigorous training in zero-gravity environments, emergency protocols, spacecraft systems, and team dynamics for long-duration missions.",
                _ => $"Research findings: General space exploration facts related to {topic}. Deep space missions require advanced technology, crew resilience, and contingency planning for unknown scenarios."
            };


            Console.WriteLine("[ResearchSpaceFacts] Research complete");

            return result;

        }


        /// <summary>
        /// 角色生成函数
        /// </summary>
        /// <returns></returns>
        [Description("Generates character profiles for the main astronaut characters in the novel")]
        static async Task<IEnumerable<string>> GenerateCharacterProfilesAsync()
        {
            Console.WriteLine("[GenerateCharacterProfiles] Generating character profiles...");

            // Simulate a character generation operation
            await Task.Delay(TimeSpan.FromSeconds(10));

            string[] profiles = [
                "Captain Elena Voss: A seasoned mission commander with 15 years of experience. Strong-willed and decisive, she struggles with the weight of responsibility for her crew. Former military pilot turned astronaut.",
            "Dr. James Chen: Chief science officer and astrophysicist. Brilliant but socially awkward, he finds solace in data and discovery. His curiosity often pushes the mission into uncharted territory.",
            "Lieutenant Maya Torres: Navigation specialist and youngest crew member. Optimistic and tech-savvy, she brings fresh perspective and innovative problem-solving to challenges.",
            "Commander Marcus Rivera: Chief engineer with expertise in spacecraft systems. Pragmatic and resourceful, he can fix almost anything with limited resources. Values crew safety above all.",
            "Dr. Amara Okafor: Medical officer and psychologist. Empathetic and observant, she helps maintain crew morale and mental health during the long journey. Expert in space medicine."
            ];

            Console.WriteLine($"[GenerateCharacterProfiles] Generated {profiles.Length} character profiles");
            return profiles;
        }



    }
}
