using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using System.Composition;

namespace Multiformats.Hash.Algorithms;

/// <summary>
/// Provides an implementation of the MD5 multihash algorithm using BouncyCastle.
/// </summary>
[Export(typeof(IMultihashAlgorithm))]
[MultihashAlgorithmExport(HashType.MD5, "md5", 16)]
public class MD5 : MultihashAlgorithm
{
    /// <summary>
    /// Factory method for creating a new <see cref="MD5Digest"/> instance.
    /// </summary>
    private readonly Func<IDigest> _factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="MD5"/> class.
    /// </summary>
    public MD5()
        : base(HashType.MD5, "md5", 16) => _factory = () => new MD5Digest();

    /// <inheritdoc/>
    public override byte[] ComputeHash(byte[] data, int length = -1) => _factory().ComputeHash(data);
}
