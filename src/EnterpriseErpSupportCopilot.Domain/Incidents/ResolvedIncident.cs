using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseErpSupportCopilot.Domain.Incidents
{
    public class ResolvedIncident
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string ProblemSummary { get; set; } = default!;
        public string RootCause { get; set; } = default!;
        public string Resolution { get; set; } = default!;
        public string Tags { get; set; } = string.Empty;

        public DateTime ResolvedAt { get; set; } = DateTime.UtcNow;
    }
}
