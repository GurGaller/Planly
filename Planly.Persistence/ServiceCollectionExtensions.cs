using Planly.Application.DomainEvents;
using Planly.Application.ExternalCalendars;
using Planly.Application.Tranactions;
using Planly.DomainModel.ExternalCalendars;
using Planly.DomainModel.Schedules;
using Planly.DomainModel.Sessions;
using Planly.DomainModel.Tasks;
using Planly.Persistence.ExternalCalendars;
using Planly.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Planly.Persistence
{
	/// <summary>
	/// Extension methods for setting up an Entity Framework Core based implementation
	/// of the persistence layer in an <see cref="IServiceCollection"/>.
	/// </summary>
	public static class ServiceCollectionExtensions
	{
		/// <summary>
		/// Adds the required services for an EF Core based persistence mechanism to a service collection.
		/// </summary>
		/// <param name="services">The service collection.</param>
		/// <returns>The same instance of the service collection, after the operation was completed.</returns>
		public static IServiceCollection AddEFCorePersistence(this IServiceCollection services)
		{
			var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

			services.AddDbContext<CustomDbContext>(options =>
			{
				options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
			});

			services.AddTransient<ISessionRepository, EFCoreSessionRepository>();
			services.AddTransient<IScheduleRepository, EFCoreScheduleRepository>();
			services.AddTransient<ITaskRepository, EFCoreTaskRepository>();
			services.AddTransient<IExternalCalendarRepository, EFCoreExternalCalendarRepository>();

			services.AddTransient<IDomainEventStore, EFCoreDomainEventStore>();
			services.AddTransient<IExternalCalendarKeyStore, EFCoreExternalCalendarKeyStore>();

			services.AddTransient<IUnitOfWorkFactory, EFCoreUnitOfWorkFactory>();

			return services;
		}
	}
}