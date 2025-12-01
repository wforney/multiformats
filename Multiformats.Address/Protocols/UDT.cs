namespace Multiformats.Address.Protocols;

/// <summary>
/// UDT
/// </summary>
public record UDT : MultiaddressProtocol
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UDT"/> class.
    /// </summary>
    public UDT()
        : base("udt", 302, 0)
    {
    }

    /// <inheritdoc />
    public override void Decode(string value)
    {
    }

    /// <inheritdoc />
    public override void Decode(byte[] bytes)
    {
    }

    /// <inheritdoc />
    public override byte[] ToBytes() => EmptyBuffer;
}
