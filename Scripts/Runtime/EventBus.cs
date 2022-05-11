using System;
using System.Linq;
using UnityEngine;

namespace EventBusSpace
{
	//  Example:
	// class MyBus : EventBus
	// {
	// 	public static BusEvent EventZero;
	//
	// 	[HideLog]
	// 	public static BusEvent<string> EventOne;
	//
	// 	public static BusState<int> StateInt;
	//
	// 	static MyBus() => InitFields<MyBus>();
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
					@event.HideInLog = fi.GetCustomAttributes(typeof(HideLogAttribute), false).Any();
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