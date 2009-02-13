namespace Magnum.ActorModel.WebSpecs
{
	using System.Diagnostics;
	using System.Threading;
	using Channels;

	public class RequestService :
		IStartable
	{
		private readonly CommandContext _queue;
		private readonly Channel<SimpleRequest> _requestChannel;
		private readonly Channel<SimpleResponse> _responseChannel;

		public RequestService(CommandContext queue, Channel<SimpleRequest> requestChannel, Channel<SimpleResponse> responseChannel)
		{
			_queue = queue;
			_responseChannel = responseChannel;
			_requestChannel = requestChannel;
		}

		public void Start()
		{
			_requestChannel.Subscribe(_queue, Consume);

			_queue.Start();
			Trace.WriteLine("Started Request Service");
		}

		public void Consume(SimpleRequest message)
		{
			Trace.WriteLine("Request Received");

			var response = new SimpleResponse
			               	{
			               		CorrelationId = message.CorrelationId,
			               		Message = "Created on Thread ID: " + Thread.CurrentThread.ManagedThreadId,
			               	};

			_responseChannel.Publish(response);
		}
	}
}