using Multiformats.Hash;
namespace Multiformats.Address.Protocols;
/// <summary>
/// P2P
/// </summary>
public record P2P : MultiaddressProtocol{
    /// <summary>
    /// Constructor for the P2P class.
    /// </summary>
    /// <returns>An instance of the P2P class.</returns>
    public P2P()
        : base("p2p", 420, -1)    {    }

    /// <summary>
    /// Constructs a P2P object from a given address.
    /// </summary>
    /// <param name="address">The address of the P2P object.</param>
    /// <returns>A P2P object.</returns>
    [Obsolete("This constructor is obsolete.")]
    public P2P(string address)        : this(Multihash.FromB58String(address))    {    }

    /// <summary>
    /// Constructor for P2P class with a given Multihash address.
    /// </summary>
    /// <param name="address">The Multihash address.</param>
    /// <returns>A new instance of the P2P class.</returns>
    public P2P(Multihash address)
            : this() => Value = address;

#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
    /// <inheritdoc />
    [Obsolete("Use byte array decode instead.")]
    public override void Decode(string value) => Value = Multihash.FromB58String(value);
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member

    /// <inheritdoc />
    public override void Decode(byte[] bytes) => Value = Multihash.Decode(bytes);

    /// <inheritdoc />
    public override byte[] ToBytes() => (Multihash?)Value ?? Array.Empty<byte>();

#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
    /// <inheritdoc />
    [Obsolete("Do not use ToString. Use ToBytes.")]
    public override string ToString() => ((Multihash?)Value)?.B58String() ?? string.Empty;
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
}
