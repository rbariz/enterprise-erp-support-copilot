using EnterpriseErpSupportCopilot.Domain.Incidents;
using EnterpriseErpSupportCopilot.Domain.Knowledge;
using EnterpriseErpSupportCopilot.Domain.Tickets;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseErpSupportCopilot.Infrastructure.Persistence
{
    public static class SupportCopilotSeeder
    {
        public static async Task SeedAsync(
            SupportCopilotDbContext db,
            CancellationToken cancellationToken = default)
        {
            await db.Database.MigrateAsync(cancellationToken);

            if (!await db.Tickets.AnyAsync(cancellationToken))
            {
                await db.Tickets.AddRangeAsync(GetTickets(), cancellationToken);
            }

            if (!await db.KnowledgeArticles.AnyAsync(cancellationToken))
            {
                await db.KnowledgeArticles.AddRangeAsync(GetKnowledgeArticles(), cancellationToken);
            }

            if (!await db.Set<ResolvedIncident>().AnyAsync(cancellationToken))
            {
                await db.Set<ResolvedIncident>().AddRangeAsync(GetResolvedIncidents(), cancellationToken);
            }

            await db.SaveChangesAsync(cancellationToken);
        }

        private static List<SupportTicket> GetTickets()
        {
            return
            [
                new SupportTicket
            {
                Reference = "ERP-2026001",
                Title = "Concurrent Program Ended in Error",
                Description = "Payables Open Interface Import completed with status Error. Several supplier invoices remain unprocessed.",
                Category = TicketCategory.ConcurrentProgram,
                Severity = TicketSeverity.High
            },
            new SupportTicket
            {
                Reference = "ERP-2026002",
                Title = "PO Approval Workflow Stuck",
                Description = "Purchase order approval workflow has been pending for more than 72 hours. No notification has been delivered to the approver.",
                Category = TicketCategory.Workflow,
                Severity = TicketSeverity.Critical
            },
            new SupportTicket
            {
                Reference = "ERP-2026003",
                Title = "Supplier Interface Rejected",
                Description = "Supplier import failed due to duplicate supplier number and missing tax registration data.",
                Category = TicketCategory.Interface,
                Severity = TicketSeverity.High
            },
            new SupportTicket
            {
                Reference = "ERP-2026004",
                Title = "GL Journal Import Failed",
                Description = "Journal import was rejected because the accounting period is closed for the target ledger.",
                Category = TicketCategory.GeneralLedger,
                Severity = TicketSeverity.Medium
            },
            new SupportTicket
            {
                Reference = "ERP-2026005",
                Title = "OAF Page Error",
                Description = "Unexpected exception raised while opening supplier profile page in Oracle Application Framework.",
                Category = TicketCategory.OafPage,
                Severity = TicketSeverity.High
            },
            new SupportTicket
            {
                Reference = "ERP-2026006",
                Title = "Forms Runtime Error",
                Description = "Oracle Forms transaction screen raises FRM error during invoice validation.",
                Category = TicketCategory.OracleForms,
                Severity = TicketSeverity.Medium
            },
            new SupportTicket
            {
                Reference = "ERP-2026007",
                Title = "Inventory Transaction Stuck",
                Description = "Inventory transactions remain stuck in interface table and are not processed by the transaction manager.",
                Category = TicketCategory.Inventory,
                Severity = TicketSeverity.High
            },
            new SupportTicket
            {
                Reference = "ERP-2026008",
                Title = "Workflow Notification Not Sent",
                Description = "Approval notifications are not delivered to users although workflow items are active.",
                Category = TicketCategory.Workflow,
                Severity = TicketSeverity.High
            },
            new SupportTicket
            {
                Reference = "ERP-2026009",
                Title = "Interface Timeout",
                Description = "External integration with procurement system fails intermittently due to timeout during file transfer.",
                Category = TicketCategory.Integration,
                Severity = TicketSeverity.Medium
            },
            new SupportTicket
            {
                Reference = "ERP-2026010",
                Title = "Invoice Import Rejected",
                Description = "AP invoice import rejected multiple records because supplier site and payment terms are invalid.",
                Category = TicketCategory.Payables,
                Severity = TicketSeverity.High
            }
            ];
        }

        private static List<KnowledgeArticle> GetKnowledgeArticles()
        {
            return
            [
                new KnowledgeArticle
            {
                Title = "Concurrent Program Failure Analysis",
                Tags = "oracle,ebs,concurrent-request,log,output,error",
                Content = "When a concurrent program ends in Error, review the request log, output file, concurrent manager status, parameters and related interface tables. Common causes include invalid data, missing setup, closed periods or unavailable concurrent managers."
            },
            new KnowledgeArticle
            {
                Title = "Payables Open Interface Import Troubleshooting",
                Tags = "oracle,ebs,payables,ap,invoice,interface,import",
                Content = "For Payables Open Interface Import issues, validate supplier, supplier site, invoice number, invoice amount, currency, payment terms and accounting date. Check rejected records in AP interface tables and correct source data before resubmission."
            },
            new KnowledgeArticle
            {
                Title = "PO Approval Workflow Stuck",
                Tags = "oracle,ebs,purchasing,po,workflow,approval,notification",
                Content = "Purchase Order approval workflows may become stuck because of inactive approvers, workflow mailer issues, invalid approval hierarchy or blocked workflow activities. Review workflow status monitor, pending notifications and approval hierarchy setup."
            },
            new KnowledgeArticle
            {
                Title = "Workflow Notification Troubleshooting",
                Tags = "oracle,ebs,workflow,notification,mailer",
                Content = "If workflow notifications are not sent, verify workflow mailer status, notification queue, recipient email setup, user roles and mailer logs. Restarting the workflow mailer may be required after correcting configuration."
            },
            new KnowledgeArticle
            {
                Title = "Supplier Interface Validation",
                Tags = "oracle,ebs,supplier,interface,ap,validation",
                Content = "Supplier interface errors often come from duplicate supplier numbers, missing mandatory attributes, invalid tax information or invalid supplier site data. Validate source files and compare against existing supplier records."
            },
            new KnowledgeArticle
            {
                Title = "ORA-00001 Unique Constraint Violation",
                Tags = "oracle,error,ora-00001,duplicate,constraint",
                Content = "ORA-00001 occurs when an insert or update violates a unique constraint. Review the constraint name, identify the target table and compare incoming data with existing records to remove duplicates."
            },
            new KnowledgeArticle
            {
                Title = "GL Journal Import Period Closed",
                Tags = "oracle,ebs,general-ledger,gl,journal,period",
                Content = "Journal import may fail when the accounting period is closed or not open for the target ledger. Validate ledger, period status, accounting date, source and category before reimport."
            },
            new KnowledgeArticle
            {
                Title = "Oracle Forms Runtime Error Analysis",
                Tags = "oracle,forms,frm,error,runtime",
                Content = "Oracle Forms runtime errors should be analyzed through FRM error code, server logs, form personalization, triggers and related PL/SQL packages. Reproduce the issue with the same responsibility and user context."
            },
            new KnowledgeArticle
            {
                Title = "OAF Page Exception Troubleshooting",
                Tags = "oracle,oaf,page,error,exception",
                Content = "OAF page errors may be caused by personalization conflicts, controller exceptions, invalid bindings, missing profile options or backend data issues. Review application logs and disable recent personalizations if needed."
            },
            new KnowledgeArticle
            {
                Title = "Inventory Interface Transactions Stuck",
                Tags = "oracle,ebs,inventory,interface,transaction,stuck",
                Content = "Inventory transactions can remain stuck due to invalid item setup, organization mismatch, transaction date issues, missing accounts or transaction manager failures. Review interface error messages and transaction processor logs."
            }
            ];
        }

        private static List<ResolvedIncident> GetResolvedIncidents()
        {
            return
            [
                new ResolvedIncident
            {
                ProblemSummary = "PO Approval Workflow Stuck",
                RootCause = "Workflow Mailer service was stopped and pending notifications were not delivered.",
                Resolution = "Restarted Workflow Mailer, verified notification queue and resent pending approval notifications.",
                Tags = "workflow,po,approval,mailer"
            },
            new ResolvedIncident
            {
                ProblemSummary = "Supplier Interface Rejected",
                RootCause = "Duplicate supplier number already existed in the supplier master data.",
                Resolution = "Corrected supplier number in source file and reloaded interface data.",
                Tags = "supplier,interface,duplicate,ora-00001"
            },
            new ResolvedIncident
            {
                ProblemSummary = "GL Journal Import Failed",
                RootCause = "Accounting period was closed for the target ledger.",
                Resolution = "Updated accounting date to an open period and relaunched journal import.",
                Tags = "gl,journal,period,import"
            },
            new ResolvedIncident
            {
                ProblemSummary = "Payables Open Interface Import Error",
                RootCause = "Supplier site was missing for several imported invoices.",
                Resolution = "Created missing supplier sites and resubmitted rejected AP interface records.",
                Tags = "payables,ap,invoice,interface"
            },
            new ResolvedIncident
            {
                ProblemSummary = "Inventory Transaction Stuck",
                RootCause = "Inventory transaction manager was inactive.",
                Resolution = "Restarted transaction manager and reprocessed pending interface transactions.",
                Tags = "inventory,transaction,manager,interface"
            }
            ];
        }
    }
}
