using System;

namespace EventBusSpace
{
    /// <summary>
    /// Do not show the Event in Log
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class HideInLogAttribute : Attribute
    {
    }
}