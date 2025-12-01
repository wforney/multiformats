using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using System.Composition;

namespace Multiformats.Hash.Algorithms;

/// <summary>
/// Provides the MD4 multihash algorithm implementation.
/// </summary>
[Export(typeof(IMultihashAlgorithm))]
[MultihashAlgorithmExport(HashType.MD4, "md4", 16)]
public class MD4 : MultihashAlgorithm
{
    /// <summary>
    /// Factory function to create a new <see cref="IDigest"/> instance for MD4.
    /// </summary>
    private readonly Func<IDigest> _factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="MD4"/> class.
    /// </summary>
    public MD4()
        : base(HashType.MD4, "md4", 16) => _factory = () => new MD4Digest();

    /// <inheritdoc/>
    public override byte[] ComputeHash(byte[] data, int length = -1) => _factory().ComputeHash(data);
}
