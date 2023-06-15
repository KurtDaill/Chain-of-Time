using Godot;
using System;

[Serializable]
public partial class UnexpectedParentException : Exception
{
    public UnexpectedParentException() : base() { }
    public UnexpectedParentException(string message) : base(message) { }
    public UnexpectedParentException(string message, Exception inner) : base(message, inner) { }

    // A constructor is needed for serialization when an
    // exception propagates from a remoting server to the client.
    protected UnexpectedParentException(System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}