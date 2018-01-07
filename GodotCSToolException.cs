using System;

namespace GodotCSTools
{
    public class GodotCSToolException : Exception
    {
        public GodotCSToolException()
        {
        }

        public GodotCSToolException(string message)
            : base(message)
        {
        }

        public GodotCSToolException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
