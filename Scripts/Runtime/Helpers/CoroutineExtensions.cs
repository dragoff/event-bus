using System;
using System.Collections;

//--------------------
//  EXAMPLE OF USAGE
//--------------------
//
// IEnumerator MyCoroutine()
// {
//     yield return Bus.MyState.Wait();
//     Debug.Log("MyState published " + Bus.MyState.Value);
//
//     var waiter = Bus.MyEvent.GetWaiter();
//     yield return waiter;
//     Debug.Log("MyEvent published " + waiter.Data);
// }

namespace EventBusSpace
{
    public interface IWaiter : IEnumerator
    {
        bool Invoked { get; }
    }

    /// <summary>Helps to join events and coroutines</summary>
    public static class CoroutineExtensions
    {
        /// <summary>
        /// Use this method to wait of Event in coroutine.
        /// Usage: yield return MyEvent.Wait();
        /// </summary>
        public static IEnumerator Wait(this BusEvent busEvent, Func<bool> condition = null)
        {
            var invoked = false;
            busEvent.SubscribeRaw(() => invoked = true)
                .Condition(() => condition == null || condition())
                .InvokeOnce();
            while (!invoked)
                yield return null;
        }

        /// <summary>
        /// Use this method to wait of State in coroutine.
        /// Usage: yield return MyState.Wait();
        /// </summary>
        public static IEnumerator Wait<T>(this BusState<T> busState, Func<T, bool> condition = null)
        {
            var invoked = false;
            busState.SubscribeRaw((data) => invoked = true)
                .Condition((data) => condition == null || condition(data))
                .InvokeOnce();
            while (!invoked)
                yield return null;
        }

        /// <summary>
        /// Use this method to wait of Event<T> in coroutine.
        /// Usage: 
        /// var waiter = MyEvent.GetWaiter();
        /// yield waiter;
        /// </summary>
        public static Waiter GetWaiter(this BusEvent busEvent, Func<bool> condition = null)
        {
            return new Waiter(busEvent, condition);
        }

        /// <summary>
        /// Use this method to wait of Event<T> in coroutine.
        /// Usage: 
        /// var waiter = MyEvent.GetWaiter();
        /// yield waiter;
        /// var data = waiter.Data;
        /// </summary>
        public static Waiter<T> GetWaiter<T>(this BusEvent<T> busEvent, Func<T, bool> condition = null)
        {
            return new Waiter<T>(busEvent, condition);
        }

        /// <summary>
        /// Use this method to wait of Event<T1, T2> in coroutine.
        /// Usage: 
        /// var waiter = MyEvent.GetWaiter();
        /// yield waiter;
        /// var data1 = waiter.Data1;
        /// </summary>
        public static Waiter<T1, T2> GetWaiter<T1, T2>(this BusEvent<T1, T2> busEvent, Func<T1, T2, bool> condition = null)
        {
            return new Waiter<T1, T2>(busEvent, condition);
        }
    }

    /// <summary>
    /// Use this class to wait Event in coroutines.
    /// Implements IEnumerator interface.
    /// </summary>
    public class Waiter : IWaiter
    {
        public bool Invoked { get; private set; }

        public Waiter(BusEvent e, Func<bool> condition = null)
        {
            e.SubscribeRaw(() =>
            {
                Invoked = true;
            })
            .Condition(() => condition == null || condition())
            .InvokeOnce();
        }

        object IEnumerator.Current => null;

        bool IEnumerator.MoveNext()
        {
            return !Invoked;
        }

        void IEnumerator.Reset()
        {
        }
    }

    /// <summary>
    /// Use this class to wait Event in coroutines.
    /// Implements IEnumerator interface.
    /// </summary>
    public class Waiter<T> : IWaiter
    {
        public T Data { get; private set; } = default;
        public bool Invoked { get; private set; }

        public Waiter(BusEvent<T> e, Func<T, bool> condition = null)
        {
            e.SubscribeRaw((data) =>
            {
                Invoked = true;
                Data = data;
            })
            .Condition((data) => condition == null || condition(data))
            .InvokeOnce();
        }

        object IEnumerator.Current => null;

        bool IEnumerator.MoveNext()
        {
            return !Invoked;
        }

        void IEnumerator.Reset()
        {
        }
    }

    /// <summary>
    /// Use this class to wait Event in coroutines.
    /// Implements IEnumerator interface.
    /// </summary>
    public class Waiter<T1, T2> : IWaiter
    {
        public T1 Data1 { get; private set; } = default;
        public T2 Data2 { get; private set; } = default;
        public bool Invoked { get; private set; }

        public Waiter(BusEvent<T1, T2> busEvent, Func<T1, T2, bool> condition = null)
        {
            busEvent.SubscribeRaw((data1, data2) =>
            {
                Invoked = true;
                Data1 = data1;
                Data2 = data2;
            })
            .Condition((data1, data2) => condition == null || condition(data1, data2))
            .InvokeOnce();
        }

        object IEnumerator.Current => null;

        bool IEnumerator.MoveNext()
        {
            return !Invoked;
        }

        void IEnumerator.Reset()
        {
        }
    }
}