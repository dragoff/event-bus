using System;

namespace EventBusSpace
{
    /// <summary>
    /// Publish this event for all players in room
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class NetworkedEventAttribute : Attribute
    {
    }
}