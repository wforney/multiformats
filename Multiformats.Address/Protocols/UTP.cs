namespace Multiformats.Address.Protocols;

/// <summary>
/// UTP
/// </summary>
public record UTP : MultiaddressProtocol
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UTP"/> class.
    /// </summary>
    public UTP()
        : base("utp", 301, 0)
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
