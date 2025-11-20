namespace MyAgentFramework.Command
{
    public class VectorChatMessageStore : ChatMessageStore
    {

        private readonly VectorStore _vectorStore;

        public string? ThreadDbKey { get; private set; }


        /// <summary>
        /// 一个 ChatMessageStore 的示例实现，用于将聊天消息存储在向量存储（Vector Store）中
        /// </summary>
        /// <param name="vectorStore"></param>
        /// <param name="serializedStoreState"></param>
        /// <param name="jsonSerializerOptions"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public VectorChatMessageStore(VectorStore vectorStore,JsonElement serializedStoreState,JsonSerializerOptions? jsonSerializerOptions = null)
        {

            //VectorStore
            this._vectorStore = vectorStore??throw new ArgumentNullException(nameof(vectorStore));

            //JsonElement
            if(serializedStoreState.ValueKind is JsonValueKind.String)
            {
                this.ThreadDbKey = serializedStoreState.Deserialize<string>();
            }

        }




        public override async Task AddMessagesAsync(IEnumerable<ChatMessage> messages, CancellationToken cancellationToken = default)
        {
            
            //如果为空则GUID赋值
            this.ThreadDbKey ??= Guid.NewGuid().ToString("N");


            //对象转换
            var chatHistoryItems = messages.Select(x => new ChatHistoryItem
            {

                Key = this.ThreadDbKey + x.MessageId,
                Timestamp = DateTimeOffset.UtcNow,
                ThreadId = this.ThreadDbKey,
                SerializedMessage = JsonSerializer.Serialize(x),
                MessageText = x.Text

            });
            

            var collection = this._vectorStore.GetCollection<string, ChatHistoryItem>("ChatHistory");


            await collection.EnsureCollectionExistsAsync(cancellationToken);


            await collection.UpsertAsync(chatHistoryItems,cancellationToken);


        }

        public override async Task<IEnumerable<ChatMessage>> GetMessagesAsync(CancellationToken cancellationToken = default)
        {

            var collection = this._vectorStore.GetCollection<string, ChatHistoryItem>("ChatHistory");

            await collection.EnsureCollectionExistsAsync(cancellationToken);

            var records = await collection.GetAsync(
                x => x.ThreadId == this.ThreadDbKey,
                10,
                new() { OrderBy = x => x.Descending(y => y.Timestamp) },
                cancellationToken).ToListAsync(cancellationToken);

            var messages = records.ConvertAll(x => JsonSerializer.Deserialize<ChatMessage>(x.SerializedMessage!)!);

            messages.Reverse();

            return messages;

        }

        public override JsonElement Serialize(JsonSerializerOptions? jsonSerializerOptions = null) => JsonSerializer.SerializeToElement(this.ThreadDbKey);
      
    }

}
