namespace Multiformats.Address.Protocols;

/// <summary>
/// UDP
/// </summary>
public record UDP : Number
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UDP"/> class.
    /// </summary>
    public UDP()
        : base("udp", 17)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UDP"/> class.
    /// </summary>
    /// <param name="port">The port.</param>
    public UDP(ushort port)
        : this() => Value = port;
}
