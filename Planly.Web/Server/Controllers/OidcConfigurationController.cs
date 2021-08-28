using System;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Mvc;

namespace Planly.Web.Server.Controllers
{
	/// <summary>
	/// Exposes endpoints for retrieving OpenID Connect Configuration documents.
	/// </summary>
	/// <seealso cref="Controller" />
	public class OidcConfigurationController : Controller
	{
		private readonly IClientRequestParametersProvider clientRequestParametersProvider;

		public OidcConfigurationController(IClientRequestParametersProvider clientRequestParametersProvider)
		{
			this.clientRequestParametersProvider = clientRequestParametersProvider;
		}

		/// <summary>
		/// Gets the request parameters for a specific client.
		/// </summary>
		/// <param name="clientId">The client's ID.</param>
		/// <response code="200">The response contains the request parameters found for the client.</response>
		/// <response code="404">The client does not exist.</response>
		[HttpGet("_configuration/{clientId}")]
		public IActionResult GetClientRequestParameters([FromRoute] string clientId)
		{
			try
			{
				var parameters = clientRequestParametersProvider.GetClientParameters(HttpContext, clientId);
				return Ok(parameters);
			}
			catch (InvalidOperationException)
			{
				return NotFound();
			}
		}
	}
}