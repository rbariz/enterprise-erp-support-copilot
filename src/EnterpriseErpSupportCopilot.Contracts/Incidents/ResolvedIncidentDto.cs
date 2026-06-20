using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseErpSupportCopilot.Contracts.Incidents
{
    public sealed record ResolvedIncidentDto(
    Guid Id,
    string ProblemSummary,
    string RootCause,
    string Resolution,
    string Tags,
    DateTime ResolvedAt
);
}
