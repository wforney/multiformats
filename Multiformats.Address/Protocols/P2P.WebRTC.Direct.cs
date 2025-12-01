namespace Multiformats.Address.Protocols;/// <summary>
/// P2PWebRTCDirect
/// </summary>
public record P2PWebRTCDirect : MultiaddressProtocol{
    /// <summary>
    /// Constructor for P2PWebRTCDirect class.
    /// </summary>
    /// <returns>An instance of the P2PWebRTCDirect class.</returns>
    public P2PWebRTCDirect()
            : base("p2p-webrtc-direct", 276, 0)    {    }

    /// <inheritdoc />
    public override void Decode(string value)    {    }

    /// <inheritdoc />
    public override void Decode(byte[] bytes)    {    }

    /// <inheritdoc />
    public override byte[] ToBytes() => EmptyBuffer;}
