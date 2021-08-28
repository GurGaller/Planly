using System;
using System.Linq;
using System.Reflection;
using FluentValidation;
using Planly.Application.DomainEvents;
using Planly.Application.Tranactions;
using Planly.Application.Validation;
using Planly.DomainModel;
using Planly.DomainModel.ExternalCalendars;
using Planly.DomainModel.Tasks;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Planly.Application
{
	/// <summary>
	/// Defines extension methods that register Application layer services to DI containers.
	/// </summary>
	public static class ServiceCollectionExtensions
	{
		/// <summary>
		/// Registers services of the Application layer.
		/// </summary>
		/// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
		/// <returns>A reference to this instance after the operation has completed.</returns>
		public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
		{
			services.AddDomainLayer();

			// Add request pipeline services
			services
				.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>))
				.AddTransient(typeof(IRequestPreProcessor<>), typeof(RequestValidationPreProcessor<>));

			services.AddMediatR(Assembly.GetExecutingAssembly());
			services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

			services.AddTransient<DomainEventProcessor>();

			return services;
		}

		private static void AddDomainEventHandlers(this IServiceCollection services)
		{
			var classes = Assembly.GetExecutingAssembly().DefinedTypes
				.Concat(typeof(Entity<>).Assembly.DefinedTypes)
				.Where(t => t.IsClass)
				.Where(t => !t.IsAbstract);

			foreach (var type in classes)
				services.AddIfIsDomainEventHandler(type);
		}

		/// <summary>
		/// Adds services required by the Domain Layer to an <see cref="IServiceCollection"/>.
		/// </summary>
		/// <param name="services">The <see cref="IServiceCollection"/>.</param>
		/// <returns>A reference to this instance after the operation has completed.</returns>
		private static IServiceCollection AddDomainLayer(this IServiceCollection services)
		{
			services.AddTransient<SessionScheduler>();
			services.AddTransient<CalendarConnector>();

			services.AddTransient<ISchedulingStrategy, LinearSchedulingStrategy>();

			services.AddDomainEventHandlers();

			return services;
		}

		private static void AddIfIsDomainEventHandler(this IServiceCollection services, Type type)
		{
			var typeIsHandler = false;
			foreach (var interfaceType in type.GetInterfaces())
			{
				if (!interfaceType.IsGenericType)
					continue;

				if (interfaceType.GetGenericTypeDefinition().IsAssignableTo(typeof(IDomainEventHandler<>)))
				{
					typeIsHandler = true;
					services.AddTransient(interfaceType, type);
				}
			}

			if (typeIsHandler)
				services.AddTransient(type);
		}
	}
}