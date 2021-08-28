using System.Collections.Generic;
using System.Linq;
using Planly.DomainModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;

namespace Planly.Persistence
{
	internal class AggregateSet<TRoot> : QueryableDecorator<TRoot>
		where TRoot : AggregateRoot<TRoot>
	{
		private static readonly List<string> includePaths = new();

		public AggregateSet(CustomDbContext dbContext)
			: base(GetQueryProvider(dbContext), GetInternalQueryable(dbContext))
		{
		}

		// TODO - refactor this method. It is taken from StackOverflow. It is too complex and unreadable.
		private static IEnumerable<string> FindIncludePaths(CustomDbContext dbContext)
		{
			var entityType = dbContext.Model.FindEntityType(typeof(TRoot));
			var includedNavigations = new HashSet<INavigation>();
			var stack = new Stack<IEnumerator<INavigation>>();
			while (true)
			{
				var entityNavigations = new List<INavigation>();
				foreach (var navigation in entityType.GetNavigations())
				{
					if (includedNavigations.Add(navigation))
						entityNavigations.Add(navigation);
				}

				if (entityNavigations.Count == 0)
				{
					if (stack.Count > 0)
						yield return string.Join(".", stack.Reverse().Select(e => e.Current.Name));
				}
				else
				{
					foreach (var navigation in entityNavigations)
					{
						var inverseNavigation = navigation.Inverse;
						if (inverseNavigation is not null)
							includedNavigations.Add(inverseNavigation);
					}
					stack.Push(entityNavigations.GetEnumerator());
				}
				while (stack.Count > 0 && !stack.Peek().MoveNext())
					stack.Pop();
				if (stack.Count == 0)
					break;
				entityType = stack.Peek().Current.TargetEntityType;
			}
		}

		private static IQueryable<TRoot> GetInternalQueryable(CustomDbContext dbContext)
		{
			var aggregates = dbContext.Set<TRoot>();
			return IncludeAll(aggregates, dbContext);
		}

		private static AutoFlushingQueryProvider GetQueryProvider(CustomDbContext dbContext)
		{
			var dbSet = dbContext.Set<TRoot>().AsQueryable();
			return new AutoFlushingQueryProvider(dbContext, (IAsyncQueryProvider)dbSet.Provider);
		}

		private static IQueryable<TRoot> IncludeAll(IQueryable<TRoot> entities, CustomDbContext dbContext)
		{
			if (!includePaths.Any())
				includePaths.AddRange(FindIncludePaths(dbContext));

			return includePaths
				.Aggregate(entities, (query, path) => query.Include(path));
		}
	}
}