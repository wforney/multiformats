using Murmur;
using System.Composition;
using System.Security.Cryptography;

namespace Multiformats.Hash.Algorithms;

/// <summary>
/// Implements the Murmur3-32 multihash algorithm.
/// </summary>
[Export(typeof(IMultihashAlgorithm))]
[MultihashAlgorithmExport(HashType.MURMUR3_32, "murmur3-32", 4)]
public class MURMUR3_32 : MultihashAlgorithm
{
    /// <summary>
    /// Factory function for creating a Murmur3-32 <see cref="HashAlgorithm"/> instance.
    /// </summary>
    private readonly Func<HashAlgorithm> _factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="MURMUR3_32"/> class.
    /// </summary>
    public MURMUR3_32()
        : base(HashType.MURMUR3_32, "murmur3-32", 4) => _factory = () => MurmurHash.Create32(managed: false);

    /// <inheritdoc/>
    public override byte[] ComputeHash(byte[] data, int length = -1) => _factory().ComputeHash(data);
}
