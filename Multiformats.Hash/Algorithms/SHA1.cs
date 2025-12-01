using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using System.Composition;

namespace Multiformats.Hash.Algorithms;

/// <summary>
/// Provides the SHA-1 multihash algorithm implementation.
/// </summary>
[Export(typeof(IMultihashAlgorithm))]
[MultihashAlgorithmExport(HashType.SHA1, "sha1", 20)]
public class SHA1 : MultihashAlgorithm
{
    /// <summary>
    /// Factory function to create a new <see cref="Sha1Digest"/> instance.
    /// </summary>
    private readonly Func<IDigest> _factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="SHA1"/> class.
    /// </summary>
    public SHA1()
        : base(HashType.SHA1, "sha1", 20) => _factory = () => new Sha1Digest();

    /// <inheritdoc/>
    public override byte[] ComputeHash(byte[] data, int length = -1) => _factory().ComputeHash(data);
}
