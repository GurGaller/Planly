using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Requests;
using Microsoft.Extensions.Logging;

namespace Planly.Web.Server.ExternalCalendars.Google
{
	internal class GoogleApiCommandBuffer
	{
		private const int GoogleBatchLimit = 50;

		private static readonly HttpStatusCode[] ErrorCodesWorthRetrying = new[]
		{
			HttpStatusCode.Forbidden,
			HttpStatusCode.InternalServerError,
			HttpStatusCode.ServiceUnavailable
		};

		private readonly ConcurrentQueue<IClientServiceRequest> commands = new();
		private readonly ILogger<GoogleApiCommandBuffer> logger;

		public GoogleApiCommandBuffer(ILogger<GoogleApiCommandBuffer> logger)
		{
			this.logger = logger;
		}

		public void Add(IClientServiceRequest command)
		{
			commands.Enqueue(command);
		}

		public Task SendBatchAsync(CancellationToken cancellationToken)
		{
			if (!commands.TryPeek(out var firstCommand))
				return Task.CompletedTask;

			var batch = new BatchRequest(firstCommand.Service);

			while (batch.Count < GoogleBatchLimit && commands.TryDequeue(out var command))
			{
				batch.Queue<object>(command, (_, error, _, _) =>
				{
					if (error is not null)
					{
						if (ErrorCodesWorthRetrying.Contains((HttpStatusCode)error.Code))
							commands.Enqueue(command);

						logger.LogError("Google API returned an error {@Error} for request {@Request}", error, command);
					}
				});
			}

			return batch.ExecuteAsync(cancellationToken);
		}
	}
}