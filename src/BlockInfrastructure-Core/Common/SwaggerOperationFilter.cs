using System.Diagnostics.CodeAnalysis;
using BlockInfrastructure.Common.Services;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BlockInfrastructure.Core.Common;

[ExcludeFromCodeCoverage]
public class SwaggerOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var endpointAuthorizationEnabled = context.ApiDescription.ActionDescriptor.EndpointMetadata
                                                  .Any(a => a.GetType() == typeof(JwtAuthenticationFilter));

        if (endpointAuthorizationEnabled)
        {
            operation.Security.Add(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "JwtAuthenticationFilter"
                        }
                    },
                    new List<string>()
                }
            });
        }
    }
}