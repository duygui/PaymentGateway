using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace PaymentGatewayAPI.Filters
{
    public class HeaderParameterOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "ApiKey", 
                In = ParameterLocation.Header, 
                Description = "Api Key (GUID)", 
                Required = true,  
                Schema = new OpenApiSchema
                {
                    Type = "string"
                }
            });
        }
    }
}

