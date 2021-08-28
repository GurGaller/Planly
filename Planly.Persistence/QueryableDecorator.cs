using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace Planly.Persistence
{
	internal class QueryableDecorator<TResult> : IQueryable<TResult>, IOrderedQueryable<TResult>,
		IAsyncEnumerable<TResult>
	{
		private readonly IQueryable<TResult> internalQuery;
		private readonly AutoFlushingQueryProvider queryProvider;

		public QueryableDecorator(AutoFlushingQueryProvider queryProvider, IQueryable<TResult> internalQuery)
		{
			this.queryProvider = queryProvider;
			this.internalQuery = internalQuery;
		}

		public IQueryProvider Provider => queryProvider;
		public Type ElementType => internalQuery.ElementType;

		public Expression Expression => internalQuery.Expression;

		public IAsyncEnumerator<TResult> GetAsyncEnumerator(CancellationToken cancellationToken = default)
		{
			return queryProvider
				.ExecuteAsync<IAsyncEnumerable<TResult>>(Expression, cancellationToken)
				.GetAsyncEnumerator(cancellationToken);
		}

		public IEnumerator<TResult> GetEnumerator()
		{
			throw new NotSupportedException("You should not iterate over a Queryable object.");
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}