namespace Multiformats.Address.Protocols;
/// <summary>
/// HTTPS
/// </summary>
public record HTTPS : MultiaddressProtocol{
    /// <summary>
    /// Constructor for HTTPS class.
    /// </summary>
    /// <returns>An instance of the HTTPS class.</returns>
    public HTTPS()
        : base("https", 480, 0)    {    }

    /// <inheritdoc />
    public override void Decode(string value)    {    }

    /// <inheritdoc />
    public override void Decode(byte[] bytes)    {    }

    /// <inheritdoc />
    public override byte[] ToBytes() => EmptyBuffer;}
