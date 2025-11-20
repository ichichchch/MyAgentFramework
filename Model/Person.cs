namespace MyAgentFramework.Model
{
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
