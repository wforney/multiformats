namespace Multiformats.Address.Protocols;

/// <summary>
/// WebSocket
/// </summary>
public record WebSocket : MultiaddressProtocol
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebSocket"/> class.
    /// </summary>
    public WebSocket()
        : base("ws", 477, 0)
    {
    }

    /// <inheritdoc />
    public override void Decode(byte[] bytes)
    {
    }

    /// <inheritdoc />
    public override void Decode(string value)
    {
    }

    /// <inheritdoc />
    public override byte[] ToBytes() => EmptyBuffer;
}
