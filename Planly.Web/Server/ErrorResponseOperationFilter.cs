using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Planly.Web.Server
{
	internal class ErrorResponseOperationFilter : IOperationFilter
	{
		public void Apply(OpenApiOperation operation, OperationFilterContext context)
		{
			operation.Responses.Add("400", new OpenApiResponse { Description = "Invalid request" });
			operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthenticated" });
			operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });
			var oAuthScheme = new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
			};

			operation.Security = new List<OpenApiSecurityRequirement>
			{
				new OpenApiSecurityRequirement
				{
					[ oAuthScheme ] = new[]{ "Planly.Web.ServerAPI" }
				}
			};
		}
	}
}