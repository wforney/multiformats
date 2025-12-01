namespace Multiformats.Address.Protocols;

/// <summary>
/// SCTP
/// </summary>
public record SCTP : Number
{
    /// <summary>
    /// Initializes a new <see cref="SCTP"/> instance.
    /// </summary>
    public SCTP()
        : base("sctp", 132)
    {
    }

    /// <summary>
    /// Initializes a new <see cref="SCTP"/> instance.
    /// </summary>
    /// <param name="port">The port.</param>
    public SCTP(int port)
        : this() => Value = port;
}
