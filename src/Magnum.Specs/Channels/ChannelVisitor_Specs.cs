namespace Magnum.Specs.Channels
{
	using System.Collections.Generic;
	using Fibers;
	using Magnum.Channels;
	using Magnum.Extensions;
	using NUnit.Framework;

	[TestFixture]
	public class Visiting_a_channel_network
	{
		[Test]
		public void Should_capture_all_of_the_nodes_involved()
		{
			var channel = new ConsumerChannel<int>(new SynchronousFiber(), x => { });
			var filter = new FilterChannel<int>(new SynchronousFiber(), channel, x => true);

			new ChannelVisitor().Visit(filter);
		}

		[Test]
		public void Should_capture_the_interval_channel()
		{
			var channel = new ConsumerChannel<ICollection<int>>(new SynchronousFiber(), x => { });
			var scheduler = new TimerFiberScheduler(new SynchronousFiber());
			var interval = new IntervalChannel<int>(new SynchronousFiber(), scheduler, 5.Minutes(), channel);

			new ChannelVisitor().Visit(interval);
		}

		[Test]
		public void Should_capture_the_instance_channel()
		{
			var provider = new DelegateChannelProvider<int>(x => new ConsumerChannel<int>(new SynchronousFiber(), y => { }));
			var channel = new InstanceChannel<int>(provider);

			new ChannelVisitor().Visit(channel);
		}

		[Test]
		public void Should_capture_the_instance_channel_with_thread_provider()
		{
			var provider = new DelegateChannelProvider<int>(x => new ConsumerChannel<int>(new SynchronousFiber(), y => { }));
			var threadProvider = new ThreadStaticChannelProvider<int>(provider);
			var channel = new InstanceChannel<int>(threadProvider);

			new ChannelVisitor().Visit(channel);
		}

		[Test]
		public void Should_capture_the_async_result_channel_and_state()
		{
			var channel = new AsyncResultChannel<int>(new ConsumerChannel<int>(new SynchronousFiber(), y => { }), x => { }, 0);

			new ChannelVisitor().Visit(channel);

			channel.Send(0);

			new ChannelVisitor().Visit(channel);
		}

		[Test]
		public void Should_capture_the_transform_channel_types()
		{
			var channel = new ConsumerChannel<string>(new SynchronousFiber(), x => { });
			var transform = new TranformChannel<int, string>(new SynchronousFiber(), channel, x => x.ToString());

			new ChannelVisitor().Visit(transform);
		}
	}
}
