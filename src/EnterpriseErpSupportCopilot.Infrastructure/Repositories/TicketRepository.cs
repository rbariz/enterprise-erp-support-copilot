using EnterpriseErpSupportCopilot.Application.Abstractions;
using EnterpriseErpSupportCopilot.Domain.Tickets;
using EnterpriseErpSupportCopilot.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace EnterpriseErpSupportCopilot.Infrastructure.Repositories
{
    public sealed class TicketRepository : ITicketRepository
    {
        private readonly SupportCopilotDbContext _db;

        public TicketRepository(SupportCopilotDbContext db)
        {
            _db = db;
        }

        public Task<List<SupportTicket>> GetAllAsync(CancellationToken cancellationToken)
        {
            return _db.Tickets
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public Task<SupportTicket?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return _db.Tickets.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task AddAsync(SupportTicket ticket, CancellationToken cancellationToken)
        {
            await _db.Tickets.AddAsync(ticket, cancellationToken);
        }

        public Task UpdateAsync(SupportTicket ticket, CancellationToken cancellationToken)
        {
            _db.Tickets.Update(ticket);
            return Task.CompletedTask;
        }
    }
}
