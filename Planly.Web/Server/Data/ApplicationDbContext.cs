using Planly.Web.Server.Models;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Planly.Web.Server.Data
{
	/// <summary>
	/// A database context for dealing with authentication concerns.
	/// </summary>
	/// <seealso cref="ApiAuthorizationDbContext{ApplicationUser}" />
	public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
		/// </summary>
		/// <param name="options">The <see cref="DbContextOptions" />.</param>
		/// <param name="operationalStoreOptions">Options of the IdentityServer4 store.</param>
		public ApplicationDbContext(
			DbContextOptions options,
			IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
		{
		}
	}
}