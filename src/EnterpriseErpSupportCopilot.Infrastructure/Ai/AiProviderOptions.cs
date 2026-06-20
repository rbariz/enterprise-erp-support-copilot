namespace EnterpriseErpSupportCopilot.Infrastructure.Ai
{
    public sealed class AiProviderOptions
    {
        public string Provider { get; set; } = "openai-compatible";
        public string OpenAiCompatibleBaseUrl { get; set; } = "";
        public string OpenAiCompatibleModel { get; set; } = "";
        public string OpenAiCompatibleApiKey { get; set; } = "";
        public double Temperature { get; set; } = 0.2;
        public int MaxTokens { get; set; } = 1200;

        // public string OpenAiCompatibleEmbeddingModel { get; set; } = "text-embedding-3-small";

        public string EmbeddingProvider { get; set; } = "ollama";
        public string OllamaBaseUrl { get; set; } = "http://localhost:11434";
        public string OllamaEmbeddingModel { get; set; } = "nomic-embed-text";
    }
}
