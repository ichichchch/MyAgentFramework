//Company Machine Using
Console.InputEncoding = Encoding.UTF8;
Console.OutputEncoding = Encoding.UTF8;

//初始化
//await Running.RunningAsync();

//多轮对话
//await MultiturnConversation.MultConversationAsync();

//函数调用
//await UsingFunctionTools.FunctionCalling();

//函数调用 + 用户审批
//await UsingFunctionToolsWithApprovals.FunctionCallingWithApprovals();

//结构化输出
//await StructuredOutput.StructureFunction();

//支持将对话线程（conversation thread）持久化到磁盘，以便后续恢复使用
await PersistedConversations.PersistedConversationsFunction();
