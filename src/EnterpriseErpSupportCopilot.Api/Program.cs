using EnterpriseErpSupportCopilot.Application;
using EnterpriseErpSupportCopilot.Application.Abstractions;
using EnterpriseErpSupportCopilot.Application.Analysis;
using EnterpriseErpSupportCopilot.Application.Rag;
using EnterpriseErpSupportCopilot.Application.Tickets;
using EnterpriseErpSupportCopilot.Contracts.Analysis;
using EnterpriseErpSupportCopilot.Contracts.Dashboard;
using EnterpriseErpSupportCopilot.Contracts.Incidents;
using EnterpriseErpSupportCopilot.Contracts.Knowledge;
using EnterpriseErpSupportCopilot.Contracts.Tickets;
using EnterpriseErpSupportCopilot.Domain.Analysis;
using EnterpriseErpSupportCopilot.Domain.Incidents;
using EnterpriseErpSupportCopilot.Domain.Tickets;
using EnterpriseErpSupportCopilot.Infrastructure;
using EnterpriseErpSupportCopilot.Infrastructure.Ai;
using EnterpriseErpSupportCopilot.Infrastructure.Knowledge;
using EnterpriseErpSupportCopilot.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("web", policy =>
    {
        policy.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
    });
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseCors("web");

app.MapGet("/api/tickets", async (
    TicketService service,
    CancellationToken cancellationToken) =>
{
    var tickets = await service.GetAllAsync(cancellationToken);
    return Results.Ok(tickets);
});

app.MapGet("/api/tickets/{id:guid}", async (
    Guid id,
    TicketService service,
    CancellationToken cancellationToken) =>
{
    var ticket = await service.GetByIdAsync(id, cancellationToken);
    return ticket is null ? Results.NotFound() : Results.Ok(ticket);
});

app.MapPost("/api/tickets", async (
    CreateTicketRequest request,
    TicketService service,
    CancellationToken cancellationToken) =>
{
    var ticket = await service.CreateAsync(request, cancellationToken);
    return Results.Created($"/api/tickets/{ticket.Id}", ticket);
});

app.MapPost("/api/tickets/{id:guid}/analyze", async (
    Guid id,
    AnalyzeTicketService service,
    CancellationToken cancellationToken) =>
{
    var analysis = await service.AnalyzeAsync(id, cancellationToken);
    return analysis is null ? Results.NotFound() : Results.Ok(analysis);
});

app.MapGet("/api/knowledge", async (
    SupportCopilotDbContext db,
    CancellationToken cancellationToken) =>
{
    var articles = await db.KnowledgeArticles
        .OrderBy(x => x.Title)
        .ToListAsync(cancellationToken);

    return Results.Ok(articles);
});
app.MapGet("/api/tickets/{id:guid}/analysis/latest", async (
    Guid id,
    SupportCopilotDbContext db,
    CancellationToken cancellationToken) =>
{
    var analysis = await db.Analyses
        .Where(x => x.TicketId == id)
        .OrderByDescending(x => x.CreatedAt)
        .FirstOrDefaultAsync(cancellationToken);

    return analysis is null ? Results.NotFound() : Results.Ok(analysis);
});

app.MapGet("/api/dashboard/summary", async (
    SupportCopilotDbContext db,
    CancellationToken cancellationToken) =>
{
    var summary = new DashboardSummaryDto(
        TotalTickets: await db.Tickets.CountAsync(cancellationToken),
        OpenTickets: await db.Tickets.CountAsync(x => x.Status == EnterpriseErpSupportCopilot.Domain.Tickets.TicketStatus.Open, cancellationToken),
        CriticalTickets: await db.Tickets.CountAsync(x => x.Severity == EnterpriseErpSupportCopilot.Domain.Tickets.TicketSeverity.Critical, cancellationToken),
        KnowledgeArticles: await db.KnowledgeArticles.CountAsync(cancellationToken),
        ResolvedIncidents: await db.ResolvedIncidents.CountAsync(cancellationToken),
        AnalysesGenerated: await db.Analyses.CountAsync(cancellationToken),
        UsefulFeedback: await db.AnalysisFeedbacks.CountAsync(x => x.IsUseful, cancellationToken),
        NotUsefulFeedback: await db.AnalysisFeedbacks.CountAsync(x => !x.IsUseful, cancellationToken)
    );

    return Results.Ok(summary);
});

app.MapGet("/api/incidents/resolved", async (
    SupportCopilotDbContext db,
    CancellationToken cancellationToken) =>
{
    var incidents = await db.ResolvedIncidents
        .OrderByDescending(x => x.ResolvedAt)
        .ToListAsync(cancellationToken);

    return Results.Ok(incidents);
});

app.MapGet("/api/tickets/{id:guid}/analysis/history", async (
    Guid id,
    SupportCopilotDbContext db,
    CancellationToken cancellationToken) =>
{
    var history = await db.Analyses
        .Where(x => x.TicketId == id)
        .OrderByDescending(x => x.CreatedAt)
        .ToListAsync(cancellationToken);

    return Results.Ok(history);
});

app.MapPut("/api/tickets/{id:guid}/status", async (
    Guid id,
    UpdateTicketStatusRequest request,
    TicketService service,
    CancellationToken cancellationToken) =>
{
    var ticket = await service.UpdateStatusAsync(id, request, cancellationToken);
    return ticket is null ? Results.NotFound() : Results.Ok(ticket);
});

app.MapPost("/api/incidents/resolved", async (
    CreateResolvedIncidentRequest request,
    SupportCopilotDbContext db,
    CancellationToken cancellationToken) =>
{
    var incident = new ResolvedIncident
    {
        ProblemSummary = request.ProblemSummary,
        RootCause = request.RootCause,
        Resolution = request.Resolution,
        Tags = request.Tags,
        ResolvedAt = DateTime.UtcNow
    };

    await db.ResolvedIncidents.AddAsync(incident, cancellationToken);
    await db.SaveChangesAsync(cancellationToken);

    return Results.Created($"/api/incidents/resolved/{incident.Id}", incident);
});


app.MapPost("/api/analysis/feedback", async (
    CreateAnalysisFeedbackRequest request,
    SupportCopilotDbContext db,
    CancellationToken cancellationToken) =>
{
    var analysisExists = await db.Analyses
        .AnyAsync(x => x.Id == request.AnalysisId, cancellationToken);

    if (!analysisExists)
    {
        return Results.NotFound();
    }

    var feedback = new AnalysisFeedback
    {
        AnalysisId = request.AnalysisId,
        IsUseful = request.IsUseful,
        Comment = request.Comment
    };

    await db.AnalysisFeedbacks.AddAsync(feedback, cancellationToken);
    await db.SaveChangesAsync(cancellationToken);

    return Results.Created($"/api/analysis/feedback/{feedback.Id}", feedback);
});
/*
app.MapGet("/api/tickets/{id:guid}/analysis/context", async (
    Guid id,
    SupportCopilotDbContext db,
    CancellationToken cancellationToken) =>
{
    var ticket = await db.Tickets.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    if (ticket is null)
    {
        return Results.NotFound();
    }

    var terms = $"{ticket.Title} {ticket.Description} {ticket.Category}".ToLowerInvariant()
        .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Where(x => x.Length >= 3)
        .Distinct()
        .Take(10)
        .ToArray();

    var knowledge = await db.KnowledgeArticles
        .Where(x => terms.Any(term =>
            x.Title.ToLower().Contains(term) ||
            x.Content.ToLower().Contains(term) ||
            x.Tags.ToLower().Contains(term)))
        .Take(5)
        .ToListAsync(cancellationToken);

    var incidents = await db.ResolvedIncidents
        .Where(x => terms.Any(term =>
            x.ProblemSummary.ToLower().Contains(term) ||
            x.RootCause.ToLower().Contains(term) ||
            x.Resolution.ToLower().Contains(term) ||
            x.Tags.ToLower().Contains(term)))
        .Take(5)
        .ToListAsync(cancellationToken);

    return Results.Ok(new
    {
        KnowledgeArticles = knowledge,
        SimilarIncidents = incidents
    });
});
*/

app.MapGet("/api/tickets/{id:guid}/analysis/context", async (
    Guid id,
    SupportCopilotDbContext db,
    IRagContextBuilder ragContextBuilder,
    CancellationToken cancellationToken) =>
{
    var ticket = await db.Tickets.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    if (ticket is null)
    {
        return Results.NotFound();
    }

    var context = await ragContextBuilder.BuildAsync(ticket, cancellationToken);

    var query = $"""
            {ticket.Title}
            {ticket.Description}
            {ticket.Category}
            """;

    var knowledge = context.KnowledgeArticles.Select(x => new KnowledgeSourceDto(
        x.Article.Id,
        x.Article.Title,
        x.Article.Content,
        x.Article.Tags,
        x.Distance,
        SimilarityScore: (int)Math.Round((1 - x.Distance) * 100),
        MatchedTerms: RagExplainabilityHelper.FindMatches(
            query,
            x.Article.Tags),
        x.KeywordScore,
        x.FinalScore
    )).ToList();

    var incidents = context.SimilarIncidents.Select(x => new ResolvedIncidentDto(
        x.Id,
        x.ProblemSummary,
        x.RootCause,
        x.Resolution,
        x.Tags,
        x.ResolvedAt
    )).ToList();

    return Results.Ok(new TicketAnalysisContextDto(
        knowledge,
        incidents
    ));
});


app.MapGet("/api/dashboard/overview", async (
    SupportCopilotDbContext db,
    CancellationToken cancellationToken) =>
{
    var summary = new DashboardSummaryDto(
        TotalTickets: await db.Tickets.CountAsync(cancellationToken),
        OpenTickets: await db.Tickets.CountAsync(x => x.Status == TicketStatus.Open, cancellationToken),
        CriticalTickets: await db.Tickets.CountAsync(x => x.Severity == TicketSeverity.Critical, cancellationToken),
        KnowledgeArticles: await db.KnowledgeArticles.CountAsync(cancellationToken),
        ResolvedIncidents: await db.ResolvedIncidents.CountAsync(cancellationToken),
        AnalysesGenerated: await db.Analyses.CountAsync(cancellationToken),
        UsefulFeedback: await db.AnalysisFeedbacks.CountAsync(x => x.IsUseful, cancellationToken),
        NotUsefulFeedback: await db.AnalysisFeedbacks.CountAsync(x => !x.IsUseful, cancellationToken)
    );
    var activities = new List<ActivityItemDto>();

    var recentTickets = await db.Tickets
        .OrderByDescending(x => x.CreatedAt)
        .Take(5)
        .Select(x => new TicketDto(
            x.Id,
            x.Reference,
            x.Title,
            x.Description,
            x.Category.ToString(),
            x.Severity.ToString(),
            x.Status.ToString(),
            x.CreatedAt))
        .ToListAsync(cancellationToken);

    var recentAnalyses = await db.Analyses
        .OrderByDescending(x => x.CreatedAt)
        .Take(5)
        .Select(x => new AiTicketAnalysisDto(
            x.Id,
            x.TicketId,
            x.Summary,
            x.ProbableRootCause,
            x.RecommendedActions,
            x.RiskAssessment,
            x.ConfidenceScore,
            x.CreatedAt))
        .ToListAsync(cancellationToken);

    activities.AddRange(
    recentTickets.Select(x =>
        new ActivityItemDto(
            x.CreatedAt,
            "Ticket",
            $"Ticket {x.Reference} created")));

    activities.AddRange(
    recentAnalyses.Select(x =>
        new ActivityItemDto(
            x.CreatedAt,
            "Analysis",
            "AI analysis generated")));

    var recentActivities = activities
    .OrderByDescending(x => x.Timestamp)
    .Take(10)
    .ToList();

    return Results.Ok(new DashboardOverviewDto(
        summary,
        recentTickets,
        recentAnalyses,
        recentActivities));
});

app.MapGet("/api/dashboard/ai-provider", (
    IOptions<AiProviderOptions> options) =>
{
    var ai = options.Value;

    return Results.Ok(new AiProviderStatusDto(
        ai.Provider,
        ai.OpenAiCompatibleModel,
        !string.IsNullOrWhiteSpace(ai.OpenAiCompatibleApiKey),
        ai.Temperature,
        ai.MaxTokens));
});
app.MapGet("/api/dashboard/analytics", async (
    SupportCopilotDbContext db,
    CancellationToken cancellationToken) =>
{
    var bySeverity = await db.Tickets
        .GroupBy(x => x.Severity)
        .Select(x => new GroupCountDto(x.Key.ToString(), x.Count()))
        .ToListAsync(cancellationToken);

    var byCategory = await db.Tickets
        .GroupBy(x => x.Category)
        .Select(x => new GroupCountDto(x.Key.ToString(), x.Count()))
        .ToListAsync(cancellationToken);

    var byStatus = await db.Tickets
        .GroupBy(x => x.Status)
        .Select(x => new GroupCountDto(x.Key.ToString(), x.Count()))
        .ToListAsync(cancellationToken);

    return Results.Ok(new AnalyticsDto(bySeverity, byCategory, byStatus));
});


app.MapGet("/api/tickets/{id:guid}/timeline", async (
    Guid id,
    SupportCopilotDbContext db,
    CancellationToken cancellationToken) =>
{
    var ticket = await db.Tickets
        .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    if (ticket is null)
    {
        return Results.NotFound();
    }

    var items = new List<TicketTimelineItemDto>
    {
        new(
            ticket.CreatedAt,
            "Ticket",
            "Ticket created",
            $"{ticket.Reference} - {ticket.Title}")
    };

    var analyses = await db.Analyses
        .Where(x => x.TicketId == id)
        .OrderByDescending(x => x.CreatedAt)
        .ToListAsync(cancellationToken);

    items.AddRange(analyses.Select(x =>
        new TicketTimelineItemDto(
            x.CreatedAt,
            "AI",
            "AI analysis generated",
            $"Confidence {x.ConfidenceScore}%")));

    var timeline = items
        .OrderByDescending(x => x.Timestamp)
        .ToList();

    return Results.Ok(timeline);
});

app.MapPost("/api/knowledge/embeddings/generate", async (
    KnowledgeEmbeddingService service,
    CancellationToken cancellationToken) =>
{
    var count = await service.GenerateMissingEmbeddingsAsync(cancellationToken);

    return Results.Ok(new
    {
        Generated = count
    });
});


app.MapGet("/api/knowledge/search", async (
    string q,
    IKnowledgeSearchService search,
    CancellationToken cancellationToken) =>
{
    var results = await search.SearchAsync(
        q,
        5,
        cancellationToken);

    return Results.Ok(results.Select(x => new
    {
        x.Article.Id,
        x.Article.Title,
        x.Article.Content,
        x.Article.Tags,
        x.Distance,
        SimilarityScore = (int)Math.Round((1 - x.Distance) * 100)
    }));
});

app.MapGet("/api/tickets/{id:guid}/prompt-preview", async (
    Guid id,
    SupportCopilotDbContext db,
    IRagContextBuilder ragContextBuilder,
    IPromptPreviewBuilder promptBuilder,
    CancellationToken cancellationToken) =>
{
    var ticket = await db.Tickets.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    if (ticket is null)
    {
        return Results.NotFound();
    }

    var context = await ragContextBuilder.BuildAsync(ticket, cancellationToken);

    var knowledge = context.KnowledgeArticles
        .Select(x => x.Article)
        .ToList();

    var preview = promptBuilder.Build(
        ticket,
        knowledge,
        context.SimilarIncidents);

    return Results.Ok(preview);
});

app.MapGet("/api/analysis/feedback", async (
    SupportCopilotDbContext db,
    CancellationToken cancellationToken) =>
{
    var feedback = await db.AnalysisFeedbacks
        .OrderByDescending(x => x.CreatedAt)
        .Select(x => new AnalysisFeedbackDto(
            x.Id,
            x.AnalysisId,
            x.IsUseful,
            x.Comment,
            x.CreatedAt))
        .ToListAsync(cancellationToken);

    return Results.Ok(feedback);
});

app.MapGet("/api/health", () => Results.Ok(new
{
    status = "Healthy",
    service = "Enterprise ERP Support Copilot"
}));


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SupportCopilotDbContext>();
    await SupportCopilotSeeder.SeedAsync(db);
}

app.Run();
