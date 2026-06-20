using EnterpriseErpSupportCopilot.Application.Abstractions;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace EnterpriseErpSupportCopilot.Infrastructure.Ai
{
    public sealed class OpenAiCompatibleEmbeddingProvider : IEmbeddingProvider
    {
        private readonly HttpClient _http;
        private readonly AiProviderOptions _options;

        public OpenAiCompatibleEmbeddingProvider(
            HttpClient http,
            IOptions<AiProviderOptions> options)
        {
            _http = http;
            _options = options.Value;
        }

        public async Task<float[]> EmbedAsync(string text, CancellationToken cancellationToken)
        {
            _http.BaseAddress = new Uri(_options.OpenAiCompatibleBaseUrl.TrimEnd('/') + "/");
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _options.OpenAiCompatibleApiKey);

            var request = new
            {
                model = _options.OllamaEmbeddingModel,
                input = text
            };

            var response = await _http.PostAsJsonAsync(
                "embeddings",
                request,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var payload = await response.Content.ReadFromJsonAsync<EmbeddingResponse>(
                cancellationToken: cancellationToken);

            return payload?.Data.FirstOrDefault()?.Embedding ?? [];
        }

        private sealed class EmbeddingResponse
        {
            [JsonPropertyName("data")]
            public List<EmbeddingItem> Data { get; set; } = [];
        }

        private sealed class EmbeddingItem
        {
            [JsonPropertyName("embedding")]
            public float[] Embedding { get; set; } = [];
        }
    }
}
