namespace EnterpriseErpSupportCopilot.Application.Abstractions
{
    public interface IEmbeddingProvider
    {
        Task<float[]> EmbedAsync(string text, CancellationToken cancellationToken);
    }
}
