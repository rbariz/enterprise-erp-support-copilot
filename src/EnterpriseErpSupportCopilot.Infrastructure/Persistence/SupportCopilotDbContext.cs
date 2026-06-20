using EnterpriseErpSupportCopilot.Domain.Analysis;
using EnterpriseErpSupportCopilot.Domain.Incidents;
using EnterpriseErpSupportCopilot.Domain.Knowledge;
using EnterpriseErpSupportCopilot.Domain.Tickets;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseErpSupportCopilot.Infrastructure.Persistence
{
    public sealed class SupportCopilotDbContext : DbContext
    {
        public SupportCopilotDbContext(DbContextOptions<SupportCopilotDbContext> options)
            : base(options)
        {
        }

        public DbSet<SupportTicket> Tickets => Set<SupportTicket>();
        public DbSet<KnowledgeArticle> KnowledgeArticles => Set<KnowledgeArticle>();
        public DbSet<AiTicketAnalysis> Analyses => Set<AiTicketAnalysis>();

        public DbSet<ResolvedIncident> ResolvedIncidents => Set<ResolvedIncident>();

        public DbSet<AnalysisFeedback> AnalysisFeedbacks => Set<AnalysisFeedback>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SupportTicket>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Reference).HasMaxLength(50).IsRequired();
                entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
                entity.Property(x => x.Description).IsRequired();
                entity.Property(x => x.Category).HasConversion<string>().HasMaxLength(50);
                entity.Property(x => x.Severity).HasConversion<string>().HasMaxLength(50);
                entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(50);
            });
            modelBuilder.HasPostgresExtension("vector");


            modelBuilder.Entity<KnowledgeArticle>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
                entity.Property(x => x.Content).IsRequired();
                entity.Property(x => x.Tags).HasMaxLength(500);
                entity.Property(x => x.Embedding).HasColumnType("vector(768)");
            });

            modelBuilder.Entity<AiTicketAnalysis>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Summary).IsRequired();
                entity.Property(x => x.ProbableRootCause).IsRequired();
                entity.Property(x => x.RecommendedActions).IsRequired();
                entity.Property(x => x.RiskAssessment).IsRequired();
            });

            modelBuilder.Entity<ResolvedIncident>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.ProblemSummary).HasMaxLength(300).IsRequired();
                entity.Property(x => x.RootCause).IsRequired();
                entity.Property(x => x.Resolution).IsRequired();
                entity.Property(x => x.Tags).HasMaxLength(500);
            });

            modelBuilder.Entity<AnalysisFeedback>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Comment).HasMaxLength(1000);
            });
        }
    }
}
