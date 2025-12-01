namespace Multiformats.Address.Protocols;

/// <summary>
/// QUIC
/// </summary>
public record QUIC : MultiaddressProtocol
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QUIC"/> record.
    /// </summary>
    public QUIC()
        : base("quic", 460, 0)
    {
    }

    /// <inheritdoc/>
    public override void Decode(string value)
    {
    }

    /// <inheritdoc/>
    public override void Decode(byte[] bytes)
    {
    }

    /// <inheritdoc/>
    public override byte[] ToBytes() => EmptyBuffer;
}
