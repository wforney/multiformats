namespace Multiformats.Address.Protocols;

/// <summary>
/// WebSocketSecure
/// </summary>
public record WebSocketSecure : MultiaddressProtocol
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebSocketSecure"/> class.
    /// </summary>
    public WebSocketSecure()
        : base("wss", 478, 0)
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
