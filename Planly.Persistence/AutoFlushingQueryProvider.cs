using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;

namespace Planly.Persistence
{
	internal class AutoFlushingQueryProvider : IAsyncQueryProvider
	{
		private static readonly MethodInfo ExecuteCoreAsyncMethod;
		private static readonly MethodInfo ExecuteEnumerableAsyncMethod;
		private readonly CustomDbContext dbContext;
		private readonly IAsyncQueryProvider internalProvider;

		static AutoFlushingQueryProvider()
		{
			ExecuteCoreAsyncMethod = typeof(AutoFlushingQueryProvider)
				.GetMethod(nameof(ExecuteCoreAsync), BindingFlags.NonPublic | BindingFlags.Instance)
				?? throw new Exception($"Expected '{typeof(AutoFlushingQueryProvider)}' to have a private instance method \"{nameof(ExecuteCoreAsync)}\"");
			ExecuteEnumerableAsyncMethod = typeof(AutoFlushingQueryProvider)
					.GetMethod(nameof(ExecuteEnumerableAsync), BindingFlags.NonPublic | BindingFlags.Instance)
					?? throw new Exception($"Expected '{typeof(AutoFlushingQueryProvider)}' to have a private instance method \"{nameof(ExecuteEnumerableAsync)}\"");
		}

		public AutoFlushingQueryProvider(CustomDbContext dbContext, IAsyncQueryProvider internalProvider)
		{
			this.dbContext = dbContext;
			this.internalProvider = internalProvider;
		}

		public IQueryable CreateQuery(Expression expression)
		{
			throw new NotImplementedException();
		}

		public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
		{
			var query = internalProvider.CreateQuery<TElement>(expression);
			return new QueryableDecorator<TElement>(this, query);
		}

		public object? Execute(Expression expression)
		{
			throw new NotSupportedException("Queries should be asynchronous.");
		}

		public TResult Execute<TResult>(Expression expression)
		{
			throw new NotSupportedException("Queries should be asynchronous.");
		}

		public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
		{
			var internalResultType = typeof(TResult).GenericTypeArguments.First();
			var method = ExecuteCoreAsyncMethod;

			if (typeof(TResult).GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>))
				method = ExecuteEnumerableAsyncMethod;

			// Calls method<internalResultType>(expression, cancellationToken)
			var result = method
				?.MakeGenericMethod(internalResultType)
				?.Invoke(this, new object[] { expression, cancellationToken });

			if (result is not TResult)
				throw new Exception($"Expected '{nameof(ExecuteCoreAsync)}' to return '{typeof(TResult)}'");

			return (TResult)result;
		}

		private async Task<TResult> ExecuteCoreAsync<TResult>(Expression expression, CancellationToken cancellationToken)
		{
			await dbContext.SaveChangesAsync(cancellationToken);
			return await internalProvider.ExecuteAsync<Task<TResult>>(expression, cancellationToken);
		}

		private async IAsyncEnumerable<TResult> ExecuteEnumerableAsync<TResult>(
			Expression expression, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await dbContext.SaveChangesAsync(cancellationToken);
			var entities = internalProvider.ExecuteAsync<IAsyncEnumerable<TResult>>(expression, cancellationToken);
			await foreach (var entity in entities)
				yield return entity;
		}
	}
}