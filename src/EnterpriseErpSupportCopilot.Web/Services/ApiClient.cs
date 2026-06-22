using EnterpriseErpSupportCopilot.Contracts.Analysis;
using EnterpriseErpSupportCopilot.Contracts.Dashboard;
using EnterpriseErpSupportCopilot.Contracts.Incidents;
using EnterpriseErpSupportCopilot.Contracts.Knowledge;
using EnterpriseErpSupportCopilot.Contracts.Tickets;
using System.Net.Http.Json;
using System.Text.Json;

namespace EnterpriseErpSupportCopilot.Web.Services;

public sealed class ApiClient
{
    private readonly HttpClient _http;

    public ApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<TicketDto>> GetTicketsAsync()
    {
        return await _http.GetFromJsonAsync<List<TicketDto>>("/api/tickets") ?? [];
    }

    public async Task<TicketDto?> GetTicketAsync(Guid id)
    {
        return await _http.GetFromJsonAsync<TicketDto>($"/api/tickets/{id}");
    }

    public async Task<TicketAnalysisResultDto?> AnalyzeTicketAsync(Guid id)
    {
        var response = await _http.PostAsync($"/api/tickets/{id}/analyze", null);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<TicketAnalysisResultDto>(JsonOptions);
    }

    public async Task<AiTicketAnalysisDto?> GetLatestAnalysisAsync(Guid id)
    {
        try
        {
            return await _http.GetFromJsonAsync<AiTicketAnalysisDto>($"/api/tickets/{id}/analysis/latest");
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<KnowledgeArticleDto>> GetKnowledgeAsync()
    {
        return await _http.GetFromJsonAsync<List<KnowledgeArticleDto>>("/api/knowledge") ?? [];
    }

    public async Task<DashboardSummaryDto?> GetDashboardSummaryAsync()
    {
        return await _http.GetFromJsonAsync<DashboardSummaryDto>("/api/dashboard/summary");
    }

    public async Task<List<ResolvedIncidentDto>> GetResolvedIncidentsAsync()
    {
        return await _http.GetFromJsonAsync<List<ResolvedIncidentDto>>("/api/incidents/resolved") ?? [];
    }

    public async Task<TicketDto?> CreateTicketAsync(CreateTicketRequest request)
    {
        var response = await _http.PostAsJsonAsync("/api/tickets", request);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<TicketDto>();
    }

    public async Task<List<AiTicketAnalysisDto>> GetAnalysisHistoryAsync(Guid id)
    {
        return await _http.GetFromJsonAsync<List<AiTicketAnalysisDto>>(
            $"/api/tickets/{id}/analysis/history") ?? [];
    }

    public async Task<TicketDto?> UpdateTicketStatusAsync(Guid id, string status)
    {
        var response = await _http.PutAsJsonAsync(
            $"/api/tickets/{id}/status",
            new UpdateTicketStatusRequest(status));

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<TicketDto>();
    }

    public async Task<ResolvedIncidentDto?> CreateResolvedIncidentAsync(
    CreateResolvedIncidentRequest request)
    {
        var response = await _http.PostAsJsonAsync("/api/incidents/resolved", request);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<ResolvedIncidentDto>();
    }

    public async Task SubmitAnalysisFeedbackAsync(CreateAnalysisFeedbackRequest request)
    {
        await _http.PostAsJsonAsync("/api/analysis/feedback", request);
    }

    public async Task<TicketAnalysisContextDto?> GetTicketAnalysisContextAsync(Guid id)
    {
        return await _http.GetFromJsonAsync<TicketAnalysisContextDto>(
            $"/api/tickets/{id}/analysis/context");
    }

    public async Task<DashboardOverviewDto?> GetDashboardOverviewAsync()
    {
        return await _http.GetFromJsonAsync<DashboardOverviewDto>("/api/dashboard/overview");
    }

    public async Task<AiProviderStatusDto?> GetAiProviderStatusAsync()
    {
        return await _http.GetFromJsonAsync<AiProviderStatusDto>("/api/dashboard/ai-provider");
    }
    public async Task<AnalyticsDto?> GetAnalyticsAsync()
    {
        return await _http.GetFromJsonAsync<AnalyticsDto>("/api/dashboard/analytics");
    }

    public async Task<List<TicketTimelineItemDto>> GetTicketTimelineAsync(Guid id)
    {
        return await _http.GetFromJsonAsync<List<TicketTimelineItemDto>>(
            $"/api/tickets/{id}/timeline") ?? [];
    }

    public async Task<PromptPreviewDto?> GetPromptPreviewAsync(Guid id)
    {
        return await _http.GetFromJsonAsync<PromptPreviewDto>(
            $"/api/tickets/{id}/prompt-preview");
    }

    public async Task<List<AnalysisFeedbackDto>> GetAnalysisFeedbackAsync()
    {
        return await _http.GetFromJsonAsync<List<AnalysisFeedbackDto>>(
            "/api/analysis/feedback") ?? [];
    }


    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    
}