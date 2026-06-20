using EnterpriseErpSupportCopilot.Application.Abstractions;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace EnterpriseErpSupportCopilot.Infrastructure.Ai
{
    public sealed class OllamaEmbeddingProvider : IEmbeddingProvider
    {
        private readonly HttpClient _http;
        private readonly AiProviderOptions _options;

        public OllamaEmbeddingProvider(
            HttpClient http,
            IOptions<AiProviderOptions> options)
        {
            _http = http;
            _options = options.Value;
        }

        public async Task<float[]> EmbedAsync(string text, CancellationToken cancellationToken)
        {
            var url = $"{_options.OllamaBaseUrl.TrimEnd('/')}/api/embed";

            var request = new
            {
                model = _options.OllamaEmbeddingModel,
                input = text
            };

            var response = await _http.PostAsJsonAsync(
                url,
                request,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var payload = await response.Content.ReadFromJsonAsync<OllamaEmbedResponse>(
                cancellationToken: cancellationToken);

            return payload?.Embeddings.FirstOrDefault() ?? [];
        }

        private sealed class OllamaEmbedResponse
        {
            [JsonPropertyName("embeddings")]
            public List<float[]> Embeddings { get; set; } = [];
        }
    }
}
