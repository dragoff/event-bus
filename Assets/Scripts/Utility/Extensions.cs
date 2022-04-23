using System;
using System.Collections.Generic;

namespace EventBusSpace
{
	public static class Extensions
	{
		public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
		{
			foreach (var element in enumerable)
				action(element);
		}
	}
	
	public static class EmptyDelegate
	{
		public static readonly Action Action = () => { };
	}

	public static class EmptyDelegate<T>
	{
		public static readonly Action<T> Action = x => { };
	}

	public static class EmptyDelegate<T, TK>
	{
		public static readonly Action<T, TK> Action = (x, y) => { };
	}
}