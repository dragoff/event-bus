using System;
using System.Runtime.CompilerServices;

namespace EventBusSpace
{
	public static class AwaitExtensions
	{
		public static BusEventAwaiter GetAwaiter(this BusEvent busEvent)
		{
			return new BusEventAwaiter(busEvent);
		}

		public static BusEventAwaiter<T> GetAwaiter<T>(this BusEvent<T> busEvent)
		{
			return new BusEventAwaiter<T>(busEvent, false);
		}

		public static BusEventAwaiter<T> GetAwaiter<T>(this BusEventFirst<T> busEventFirst)
		{
			return new BusEventAwaiter<T>(busEventFirst.BusEvent, true);
		}

		public static BusEventFirst<T> AwaitFirst<T>(this BusEvent<T> busEvent)
		{
			return new BusEventFirst<T>(busEvent);
		}

		public class BusEventFirst<T>
		{
			public BusEvent<T> BusEvent { get; }

			public BusEventFirst(BusEvent<T> busEvent)
			{
				BusEvent = busEvent;
			}
		}

		public class BusEventAwaiter<T> : INotifyCompletion
		{
			public bool IsCompleted => isEventFired;

			private readonly bool subscribeFirst;
			private readonly BusEvent<T> busEvent;
			private bool isEventFired = false;
			private T result;
			private Action continuation;

			public BusEventAwaiter(BusEvent<T> busEvent, bool subscribeFirst)
			{
				this.subscribeFirst = subscribeFirst;
				this.busEvent = busEvent;
			}

			public T GetResult() => result;

			public void OnCompleted(Action action)
			{
				this.continuation = action;

				if (subscribeFirst)
					busEvent.SubscribeRawFirst(BusEventFired);
				else
					busEvent.SubscribeRaw(BusEventFired);
			}

			private void BusEventFired(T res)
			{
				busEvent.Unsubscribe(BusEventFired);
				this.result = res;
				isEventFired = true;
				continuation?.Invoke();
			}
		}

		public class BusEventAwaiter : INotifyCompletion
		{
			public bool IsCompleted => isEventFired;

			private readonly BusEvent busEvent;
			private bool isEventFired = false;
			private Action continuation;

			public BusEventAwaiter(BusEvent busEvent)
			{
				this.busEvent = busEvent;
			}

			public void GetResult()
			{
			}

			public void OnCompleted(Action action)
			{
				this.continuation = action;
				busEvent.SubscribeRaw(BusEventFired);
			}

			private void BusEventFired()
			{
				busEvent.Unsubscribe(BusEventFired);
				isEventFired = true;
				continuation?.Invoke();
			}
		}
	}
}