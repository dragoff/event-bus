using System;

namespace EventBusSpace
{
	/// <summary>
	/// Do not show the Event in Log
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class LogEventAttribute : Attribute
	{
	}

	/// <summary>
	/// Allow event call self from subscribers (beware of infinity loop)
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class AllowChainAttribute : Attribute
	{
	}
}