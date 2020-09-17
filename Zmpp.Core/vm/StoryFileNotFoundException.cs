namespace Zmpp.Core.Vm
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The exception that is thrown when a story file is not found.
    /// </summary>
    [Serializable()]
    public class StoryFileNotFoundException : Exception
    {
        public StoryFileNotFoundException() : base() { }
        public StoryFileNotFoundException(string message) : base(message) { }
        public StoryFileNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected StoryFileNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
