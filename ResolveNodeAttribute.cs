using System;

namespace GodotCSTools
{
    /// <summary>
    /// Fields with this attribute will have their value set based on a NodePath in another field.
    /// </summary>
    /// <remarks>
    /// This is primarily useful for getting the value of an exported NodePath field.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ResolveNodeAttribute : Attribute
    {
        public string TargetFieldName
        {
            get;
            set;
        }

        /// <summary>
        /// Constructs a <see cref="ResolveNodeAttribute"/>.
        /// </summary>
        /// <param name="fieldName">The name of the field containing a NodePath.</param>
        public ResolveNodeAttribute(string fieldName)
        {
            this.TargetFieldName = fieldName;
        }
    }
}
