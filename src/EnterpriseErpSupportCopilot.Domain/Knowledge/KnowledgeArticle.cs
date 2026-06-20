using System;
using System.Collections.Generic;
using System.Linq;
using Pgvector;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseErpSupportCopilot.Domain.Knowledge
{

    public class KnowledgeArticle
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Title { get; set; } = default!;
        public string Content { get; set; } = default!;
        public string Tags { get; set; } = string.Empty;

        public Vector? Embedding { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
