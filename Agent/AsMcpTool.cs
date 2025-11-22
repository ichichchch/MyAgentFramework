namespace MyAgentFramework.Agent
{
    /// <summary>
    /// 作为MCP工具
    /// </summary>
    public static class AsMcpTool
    {
        public static async Task MCPFunction()
        {


            //Env Apikey
            string? apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? throw new InvalidOperationException("OPENAI_API_KEY is not set.");



            //持久化Client
#pragma warning disable OPENAI001 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。
            var assistantClient = new OpenAIClient(apiKey).GetAssistantClient();
#pragma warning restore OPENAI001 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。


            //持久化Agent
            var PersistentAgent = await assistantClient.CreateAIAgentAsync(model: "gpt-4o-mini",instructions:"你是一位经验丰富的心理医生",name:"Zer!",description:"吧吧拉叭叭");

            //Agent
            var agent = await assistantClient.GetAIAgentAsync(PersistentAgent.Id);


            //Agent 转换为 MCP Tool(通过AsAIFunction适配器)
            var tool = McpServerTool.Create(agent.AsAIFunction());

            //创建空主机生成器
            var builder = Host.CreateEmptyApplicationBuilder(null);

            //DI
            builder.Services.AddMcpServer().WithStdioServerTransport().WithTools([tool]);


            //Run Mcp Tool
            await builder.Build().RunAsync();

         
        }
    }
}
