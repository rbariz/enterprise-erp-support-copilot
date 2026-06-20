using EnterpriseErpSupportCopilot.Application.Abstractions;
using EnterpriseErpSupportCopilot.Contracts.Tickets;
using EnterpriseErpSupportCopilot.Domain.Tickets;

namespace EnterpriseErpSupportCopilot.Application.Tickets
{
    public sealed class TicketService
    {
        private readonly ITicketRepository _tickets;
        private readonly IUnitOfWork _unitOfWork;

        public TicketService(ITicketRepository tickets, IUnitOfWork unitOfWork)
        {
            _tickets = tickets;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<TicketDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            var tickets = await _tickets.GetAllAsync(cancellationToken);
            return tickets.Select(TicketMapper.ToDto).ToList();
        }

        public async Task<TicketDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var ticket = await _tickets.GetByIdAsync(id, cancellationToken);
            return ticket is null ? null : TicketMapper.ToDto(ticket);
        }

        public async Task<TicketDto> CreateAsync(CreateTicketRequest request, CancellationToken cancellationToken)
        {
            var ticket = new SupportTicket
            {
                Reference = $"ERP-{DateTime.UtcNow:yyyyMMddHHmmss}",
                Title = request.Title,
                Description = request.Description,
                Category = Enum.Parse<TicketCategory>(request.Category, true),
                Severity = Enum.Parse<TicketSeverity>(request.Severity, true)
            };

            await _tickets.AddAsync(ticket, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return TicketMapper.ToDto(ticket);
        }

        public async Task<TicketDto?> UpdateStatusAsync(
    Guid id,
    UpdateTicketStatusRequest request,
    CancellationToken cancellationToken)
        {
            var ticket = await _tickets.GetByIdAsync(id, cancellationToken);

            if (ticket is null)
            {
                return null;
            }

            ticket.Status = Enum.Parse<TicketStatus>(request.Status, true);

            await _tickets.UpdateAsync(ticket, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return TicketMapper.ToDto(ticket);
        }
    }
}
