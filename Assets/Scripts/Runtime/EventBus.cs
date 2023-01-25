using System;
using System.Linq;
using UnityEngine;

namespace EventBusSpace
{
	//  Example:
	// class MyBus : EventBus
	// {
	//     [LogEvent]
	//     public static BusEvent EventZero;
	//     public static BusEvent<string> EventOne;
	//     public static BusState<int> StateInt;
	//     static MyBus() => InitFields<MyBus>();
	// }
	//
	// public class SampleScript : MonoBehaviour
	// {
	//     private void Start()
	//     {
	//         MyBus.EventOne.Subscribe(this, s => Debug.Log($"MyBus.EventOne happened: {s}"));
	//         MyBus.EventZero.Subscribe(this, () => Debug.Log($"MyBus.EventZero happened"));
	//         MyBus.StateInt.Subscribe(this, () => Debug.Log($"MyBus.StateInt changed: {MyBus.StateInt.Value}"));
	//
	//         MyBus.EventOne += "test";
	//         MyBus.EventZero += true;
	//         MyBus.StateInt.Value = int.MaxValue;
	//     }
	// }

	/// <summary>
	/// Base class for any Bus unit.
	/// </summary>
	public class EventBus
	{
		/// <summary>
		/// Auto create fields inherited from EventBase
		/// </summary>
		protected static void InitFields<T>()
		{
			var thisType = typeof(T);
			thisType
				.GetFields()
				.Where(fi =>
					typeof(BusEventBase).IsAssignableFrom(fi.FieldType)
					&& fi.GetValue(null) == null)
				.ForEach(fi =>
				{
					var @event = (BusEventBase)Activator.CreateInstance(fi.FieldType);
					@event.Name = $"<b>{thisType.Name}</b>.{fi.Name}";
					@event.LogEvent = fi.GetCustomAttributes(typeof(LogEventAttribute), false).Any();
					@event.EnableChainPublishing = fi.GetCustomAttributes(typeof(AllowChainAttribute), false).Any();
					fi.SetValue(null, @event);
				});

			Debug.Log($"<b><color=lightblue>{thisType.Name}</color></b> has been instantiated.");
		}
	}

	public class EventBus<T> : EventBus where T : class
	{
		public EventBus() => InitFields<T>();
	}
}