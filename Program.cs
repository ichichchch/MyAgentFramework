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
await UsingFunctionToolsWithApprovals.FunctionCallingWithApprovals();
