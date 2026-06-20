using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseErpSupportCopilot.Contracts.Knowledge
{
    public sealed record KnowledgeArticleDto(
    Guid Id,
    string Title,
    string Content,
    string Tags,
    DateTime CreatedAt
);
}
