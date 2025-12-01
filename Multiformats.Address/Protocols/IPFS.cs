using Multiformats.Hash;
namespace Multiformats.Address.Protocols;
/// <summary>
/// IPFS
/// </summary>
[Obsolete("Use P2P instead")]public record IPFS : MultiaddressProtocol{
    /// <summary>
    /// Constructor for IPFS class.
    /// </summary>
    /// <returns>An instance of the IPFS class.</returns>
    public IPFS()
        : base("ipfs", 421, -1)    {    }

    /// <summary>
    /// Initializes a new instance of the IPFS class with the specified address.
    /// </summary>
    /// <param name="address">The address of the IPFS node.</param>
    /// <returns>A new instance of the IPFS class.</returns>
    public IPFS(string address)
        : this(Multihash.FromB58String(address))    {    }

    /// <summary>
    /// Constructor for IPFS class with a given Multihash address.
    /// </summary>
    /// <param name="address">The Multihash address.</param>
    /// <returns>An instance of the IPFS class.</returns>
    public IPFS(Multihash address)
        : this() => Value = address;

    /// <inheritdoc />
    public override void Decode(string value) => Value = Multihash.FromB58String(value);

    /// <inheritdoc />
    public override void Decode(byte[] bytes) => Value = Multihash.Decode(bytes);

    /// <inheritdoc />
    public override byte[] ToBytes() => (Multihash?)Value ?? Array.Empty<byte>();

    /// <inheritdoc />
    public override string ToString() => ((Multihash?)Value)?.B58String() ?? string.Empty;}
