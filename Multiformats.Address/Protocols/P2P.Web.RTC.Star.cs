namespace Multiformats.Address.Protocols;/// <summary>
/// P2PWebRTCStar
/// </summary>
public record P2PWebRTCStar : MultiaddressProtocol{
    /// <summary>
    /// Constructor for P2PWebRTCStar class.
    /// </summary>
    /// <returns>An instance of the P2PWebRTCStar class.</returns>
    public P2PWebRTCStar()
        : base("p2p-webrtc-star", 275, 0)    {    }

    /// <inheritdoc />
    public override void Decode(string value)    {    }

    /// <inheritdoc />
    public override void Decode(byte[] bytes)    {    }

    /// <inheritdoc />
    public override byte[] ToBytes() => EmptyBuffer;}
