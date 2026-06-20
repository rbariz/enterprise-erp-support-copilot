namespace EnterpriseErpSupportCopilot.Domain.Tickets
{
    public class SupportTicket
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Reference { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;

        public TicketCategory Category { get; set; }
        public TicketSeverity Severity { get; set; }
        public TicketStatus Status { get; set; } = TicketStatus.Open;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
