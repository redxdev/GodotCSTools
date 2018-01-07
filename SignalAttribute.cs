using System;

namespace GodotCSTools
{
    /// <summary>
    /// Registers a delegate as a Godot signal.
    /// </summary>
    /// <
    [AttributeUsage(AttributeTargets.Delegate, AllowMultiple = false)]
    public class SignalAttribute : Attribute
    {
        public string SignalName
        {
            get;
            set;
        } = null;

        public object[] Arguments
        {
            get;
            set;
        } = null;

        /// <summary>
        /// Construct a <see cref="SignalAttribute"/> with the name of the signal being the name of the delegate.
        /// </summary>
        public SignalAttribute()
        {
        }

        /// <summary>
        /// Construct a <see cref="SignalAttribute"/> with a custom name.
        /// </summary>
        /// <param name="name">The name of the signal.</param>
        public SignalAttribute(string name)
        {
            SignalName = name;
        }
    }
}
