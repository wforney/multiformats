namespace Multiformats.Address.Protocols;/// <summary>
/// P2PCircuit
/// </summary>
public record P2PCircuit : MultiaddressProtocol{
    /// <summary>
    /// Constructor for the P2PCircuit class.
    /// </summary>
    /// <returns>An instance of the P2PCircuit class.</returns>
    public P2PCircuit()
        : base("p2p-circuit", 290, 0)    {    }

    /// <inheritdoc />
    public override void Decode(string value)    {    }

    /// <inheritdoc />
    public override void Decode(byte[] bytes)    {    }

    /// <inheritdoc />
    public override byte[] ToBytes() => EmptyBuffer;}
