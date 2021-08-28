using System;
using System.Linq;
using System.Threading.Tasks;
using Planly.Persistence;
using Planly.Web.Server.Data;
using Planly.Web.Server.Identity;
using Planly.Web.Server.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Planly.Web.Server
{
	/// <summary>
	/// Holds the main entry point of the program.
	/// </summary>
	public class Program
	{
		/// <summary>
		/// Defines the entry point of the application.
		/// </summary>
		/// <param name="args">The command line arguments passed to the program.</param>
		public static async Task Main(string[] args)
		{
			var host = CreateHostBuilder(args).Build();

			await MigrateDatebaseAsync(host.Services);

			await CreateAdministratorRoleAsync(host.Services);

			await host.RunAsync();
		}

		private static async Task CreateAdministratorRoleAsync(IServiceProvider services)
		{
			using var scope = services.CreateScope();
			var adminRole = new IdentityRole
			{
				Name = "Administrator"
			};
			var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
			var roleExists = await roleManager.RoleExistsAsync(adminRole.Name);
			if (!roleExists)
				await roleManager.CreateAsync(adminRole);

			var userManager = scope.ServiceProvider.GetRequiredService<CustomUserManager>();
			var adminExists = roleExists && (await userManager.GetUsersInRoleAsync(adminRole.Name)).Any();
			if (!adminExists)
			{
				const string AdminEmail = "gallergur@gmail.com";
				var admin = new ApplicationUser
				{
					UserName = AdminEmail,
					Email = AdminEmail
				};

				await userManager.CreateAsync(admin, password: "changeMeInProduction");
				await userManager.AddToRoleAsync(admin, adminRole.Name);
			}
		}

		private static IHostBuilder CreateHostBuilder(string[] args)
		{
			return Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
		}

		private static async Task MigrateDatebaseAsync(IServiceProvider serviceProvider)
		{
			using var serviceScope = serviceProvider.CreateScope();

			var identityContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
			await identityContext.Database.MigrateAsync();

			var mainDbContext = serviceScope.ServiceProvider.GetRequiredService<CustomDbContext>();
			await mainDbContext.Database.MigrateAsync();
		}
	}
}