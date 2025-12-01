namespace Multiformats.Address.Protocols;

/// <summary>
/// P2PWebSocketStar
/// </summary>
public record P2PWebSocketStar : MultiaddressProtocol
{
    /// <summary>
    /// Constructor for P2PWebSocketStar class.
    /// </summary>
    /// <returns>An instance of the P2PWebSocketStar class.</returns>
    public P2PWebSocketStar()
        : base("p2p-websocket-star", 479, 0)
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
