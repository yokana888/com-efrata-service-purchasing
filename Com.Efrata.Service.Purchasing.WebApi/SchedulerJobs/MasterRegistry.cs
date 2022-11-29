using Com.Efrata.Service.Purchasing.Lib.Utilities.CacheManager;
using FluentScheduler;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.WebApi.SchedulerJobs
{
    public class MasterRegistry : Registry
    {
        public MasterRegistry(IServiceProvider serviceProvider)
        {
            var coreService = serviceProvider.GetService<ICoreData>();
            Schedule(() =>
            {
                coreService.SetBankAccount();
                coreService.SetCategoryCOA();
                coreService.SetDivisionCOA();
                coreService.SetPPhCOA();
                coreService.SetUnitCOA();
            }).ToRunNow();

        }
    }

    public class ResponseHeaderFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            // Get all response header declarations for a given operation
            if (operation.Parameters == null)
                operation.Parameters = new List<IParameter>();

            operation.Parameters.Add(new NonBodyParameter
            {
                Name = "x-timezone-offset",
                In = "header",
                Type = "string",
                Required = true
            });
        }
    }
}
