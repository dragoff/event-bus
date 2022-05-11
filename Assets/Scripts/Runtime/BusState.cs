using System;
using System.Collections.Generic;
using UnityEngine;

namespace EventBusSpace
{
	internal interface IBusState
	{
	}

	public class BusState<T, TK> : BusEvent<T, TK>, IBusState, IEquatable<BusState<T, TK>>
	{
		public (T, TK) Value
		{
			get => (Data1, Data2);
			set => Publish(value.Item1, value.Item2);
		}

		/// <summary>
		/// Returns previous Value, before changing.
		/// This property have value only inside subscriber's callbacks.
		/// </summary>
		public (T, TK) PrevValue { get; protected set; }

		/// <summary>
		/// Assign value and do not call Publish.
		/// </summary>
		public void Assign(T data1, TK data2)
		{
			Data1 = data1;
			Data2 = data2;
		}

		public override void Publish(T data1, TK data2)
		{
			if (!CanPublish)
				return;

			PrevValue = (Data1, Data2);

			Assign(data1, data2);
			Publish();

			PrevValue = default;
		}

		public override SubscriberInfo BindInternal(Component comp, BindDirection way, Action<T, TK> callback)
		{
			if (way != BindDirection.OnlyPublish)
				try
				{
					callback(Data1, Data2);
				}
				catch (Exception ex)
				{
					Debug.LogError(ex);
				}

			return base.BindInternal(comp, way, callback);
		}

		/// <summary>
		/// Converts <see cref="BusState{T,TK}"/> to T implicitly
		/// </summary>
		public static implicit operator T(BusState<T, TK> busState)
		{
			return busState.Data1;
		}

		/// <summary>
		/// Converts <see cref="BusState{T,TK}"/> to TK implicitly
		/// </summary>
		public static implicit operator TK(BusState<T, TK> busState)
		{
			return busState.Data2;
		}

		/// <summary>
		/// Converts <see cref="BusState{T,TK}"/> to (T,TK) implicitly
		/// </summary>
		public static implicit operator (T, TK)(BusState<T, TK> busState)
		{
			return (busState.Data1, busState.Data2);
		}

		public static BusState<T, TK> operator +(BusState<T, TK> busState, (T, TK) data)
		{
			busState.Publish(data.Item1,data.Item2);
			return busState;
		}

		public static bool operator ==(BusState<T, TK> busState, BusState<T, TK> otherBusState)
		{
			if (busState == null)
			{
				Debug.LogError($"{nameof(busState)} is NULL, comparing via \"==\" with {typeof(T)}:[{otherBusState}]");
				return false;
			}

			return busState.Equals(otherBusState);
		}

		public static bool operator ==(BusState<T, TK> busState, T val)
		{
			return busState != null && Equals(busState.Data1, val);
		}

		public static bool operator ==(BusState<T, TK> busState, TK val)
		{
			return busState != null && Equals(busState.Data2, val);
		}

		public static bool operator !=(BusState<T, TK> busState, T val)
		{
			return busState != null && !Equals(busState.Data1, val);
		}

		public static bool operator !=(BusState<T, TK> busState, TK val)
		{
			return busState != null && !Equals(busState.Data2, val);
		}

		public static bool operator !=(BusState<T, TK> busState, BusState<T, TK> otherBusState)
		{
			if (!(busState == null))
				return !busState.Equals(otherBusState);

			Debug.LogError($"{nameof(busState)} is NULL, comparing via \"!=\" with {typeof(T)}:[{otherBusState}]");
			return false;
		}

		public bool Equals(BusState<T, TK> other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;

			return Value.Equals(other.Value);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;

			return Equals((BusState<T, TK>)obj);
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public void Clear()
		{
			Assign(default, default);
		}
	}

	/// <summary>
	/// Stores Value and calls Publish when value was assigned.
	/// Implements Publisher-Subscriber pattern.
	/// </summary>
	public class BusState<T> : BusEvent<T>, IBusState
	{
		/// <summary>Default constructor</summary>
		public BusState()
		{
		}

		/// <summary>
		/// Creates the State<T> with value, w/o calling of Publish
		/// </summary>
		public BusState(T data)
		{
			Data = data;
		}

		/// <summary>Assign Value and Publish it</summary>
		public override void Publish(T data)
		{
			if (!CanPublish)
				return;

			PrevValue = Data;

			Data = data;
			Publish();

			PrevValue = default;
		}

		public virtual void Repeat() => Publish(Data);

		/// <summary>Assign Value and Publish it, if value was changed</summary>
		public void PublishIfChanged(T data)
		{
			if (!Equals(Data, data))
				Publish(data);
		}

		public override SubscriberInfo BindInternal(Component comp, BindDirection way, Action<T> callback)
		{
			if (way != BindDirection.OnlyPublish)
				try
				{
					callback(Data);
				}
				catch (Exception ex)
				{
					Debug.LogError(ex);
				}

			return base.BindInternal(comp, way, callback);
		}

		/// <summary>
		/// Returns Value of State.
		/// Assigns value and calls Publish().
		/// </summary>
		public T Value
		{
			get => Data;
			set => Publish(value);
		}

		/// <summary>
		/// Returns previous Value, before changing.
		/// This property have value only inside subscriber's callbacks.
		/// </summary>
		public T PrevValue { get; protected set; }

		/// <summary>
		/// Assign value and do not call Publish.
		/// </summary>
		public void Assign(T value)
		{
			Data = value;
		}

		/// <summary>
		/// Converts State<T> to T implicitly
		/// </summary>
		public static implicit operator T(BusState<T> busState)
		{
			return busState.Data;
		}

		/// <summary>
		/// Allows to assign value to State<T> by following way: MyState += value;
		/// </summary>
		public static BusState<T> operator +(BusState<T> busState, T val)
		{
			busState.Publish(val);
			return busState;
		}

		public static bool operator ==(BusState<T> busState, T val)
		{
			if (busState != null)
				return Equals(busState.Data, val);

			Debug.LogWarning($"{nameof(busState)} is NULL, comparing via \"==\" with {typeof(T)}:[{val}]");
			return Equals(null, val);
		}

		public static bool operator !=(BusState<T> busState, T val)
		{
			if (busState != null)
				return !Equals(busState.Data, val);

			Debug.LogWarning($"{nameof(busState)} is NULL, comparing via \"==\" with {typeof(T)}:[{val}]");
			return Equals(null, val);
		}

		protected bool Equals(BusState<T> other)
		{
			return EqualityComparer<T>.Default.Equals(Value, other.Value);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;

			return Equals((BusState<T>)obj);
		}

		public override int GetHashCode()
		{
			return EqualityComparer<T>.Default.GetHashCode(Value);
		}

		public override string ToString()
		{
			var name = Name ?? $"State<{typeof(T).Name}>";
			return $"{name}: {Data}";
		}

		protected override string ToLogString()
		{
			var name = Name ?? $"State<{typeof(T).Name}>";
			return $"<color=lightblue>{name}</color>: {Data}";
		}

		public void Clear()
		{
			Assign(default);
		}
	}
}