using EnterpriseErpSupportCopilot.Application.Abstractions;
using EnterpriseErpSupportCopilot.Infrastructure.Ai;
using EnterpriseErpSupportCopilot.Infrastructure.Knowledge;
using EnterpriseErpSupportCopilot.Infrastructure.Persistence;
using EnterpriseErpSupportCopilot.Infrastructure.Rag;
using EnterpriseErpSupportCopilot.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseErpSupportCopilot.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<SupportCopilotDbContext>(options =>
                     options.UseNpgsql(
                         configuration.GetConnectionString("DefaultConnection"),
                         npgsqlOptions =>
                         {
                             npgsqlOptions.UseVector();
                         })
                     .UseSnakeCaseNamingConvention());

            services.AddScoped<ITicketRepository, TicketRepository>();
            services.AddScoped<IKnowledgeRepository, KnowledgeRepository>();
            services.AddScoped<IAnalysisRepository, AnalysisRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //services.AddScoped<ITicketAnalysisProvider, StubTicketAnalysisProvider>();

            services.Configure<AiProviderOptions>(
    configuration.GetSection("AiProvider"));

            services.AddHttpClient<OpenAiCompatibleTicketAnalysisProvider>();

            services.AddScoped<ITicketAnalysisProvider>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<AiProviderOptions>>().Value;

                if (options.Provider.Equals("openai-compatible", StringComparison.OrdinalIgnoreCase))
                {
                    return sp.GetRequiredService<OpenAiCompatibleTicketAnalysisProvider>();
                }

                return sp.GetRequiredService<StubTicketAnalysisProvider>();
            });


            services.AddHttpClient<OllamaEmbeddingProvider>();

            services.AddScoped<IEmbeddingProvider>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<AiProviderOptions>>().Value;

                if (options.EmbeddingProvider.Equals("ollama", StringComparison.OrdinalIgnoreCase))
                {
                    return sp.GetRequiredService<OllamaEmbeddingProvider>();
                }

                return sp.GetRequiredService<OpenAiCompatibleEmbeddingProvider>();
            });

            services.AddScoped<StubTicketAnalysisProvider>();

            services.AddScoped<IResolvedIncidentRepository, ResolvedIncidentRepository>();

            services.AddHttpClient<OpenAiCompatibleEmbeddingProvider>();


            services.AddScoped<KnowledgeEmbeddingService>();

            services.AddScoped<IKnowledgeSearchService, KnowledgeSearchService>();

            services.AddScoped<IRagContextBuilder, RagContextBuilder>();

            return services;
        }
    }
}
