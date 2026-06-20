using EnterpriseErpSupportCopilot.Application.Analysis;
using EnterpriseErpSupportCopilot.Application.Tickets;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseErpSupportCopilot.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<TicketService>();
            services.AddScoped<AnalyzeTicketService>();

            return services;
        }
    }
}
