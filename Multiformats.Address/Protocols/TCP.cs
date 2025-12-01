namespace Multiformats.Address.Protocols;

/// <summary>
/// TCP
/// </summary>
public record TCP : Number
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TCP"/> class.
    /// </summary>
    public TCP()
        : base("tcp", 6)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TCP"/> class.
    /// </summary>
    /// <param name="port">The port.</param>
    public TCP(ushort port)
        : this() => Value = port;
}
