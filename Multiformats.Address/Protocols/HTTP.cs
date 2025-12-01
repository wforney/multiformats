namespace Multiformats.Address.Protocols;
/// <summary>
/// HTTP
/// </summary>
public record HTTP : MultiaddressProtocol{
    /// <summary>
    /// Constructor for the HTTP class.
    /// </summary>
    /// <returns>An instance of the HTTP class.</returns>
    public HTTP()
        : base("http", 480, 0)    {    }

    /// <inheritdoc />
    public override void Decode(string value)    {    }

    /// <inheritdoc />
    public override void Decode(byte[] bytes)    {    }

    /// <inheritdoc />
    public override byte[] ToBytes() => EmptyBuffer;}
