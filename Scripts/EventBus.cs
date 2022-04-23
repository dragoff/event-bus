using System;
using System.Linq;
using UnityEngine;

namespace EventBusSpace
{
	/// <summary>
	/// Base class for any Bus unit.
	/// Usage:
	/// private class MyBus : EventBus
	/// {
	///		public static BusEvent EventZero;
	/// 	[HideInLog]
	///		public static BusEvent<string> RREventOne;
	///		public static State<int> StateInt
	/// 
	///		static MyBus() => InitFields<MyBus>();
	/// }
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
					@event.HideInLog = fi.GetCustomAttributes(typeof(HideInLogAttribute), false).Any();
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