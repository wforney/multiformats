using System.Composition;

namespace Multiformats.Hash.Algorithms;

/// <summary>
/// Represents the DBL-SHA2-256 multihash algorithm implementation. This algorithm applies SHA-256
/// twice to the input data.
/// </summary>
[Export(typeof(IMultihashAlgorithm))]
[MultihashAlgorithmExport(HashType.DBL_SHA2_256, "dbl-sha2-256", 32)]
public class DBL_SHA2_256 : MultihashAlgorithm
{
    /// <summary>
    /// The underlying SHA-256 hash algorithm instance.
    /// </summary>
    private readonly System.Security.Cryptography.HashAlgorithm _algo;

    /// <summary>
    /// Initializes a new instance of the <see cref="DBL_SHA2_256"/> class.
    /// </summary>
    public DBL_SHA2_256()
        : base(HashType.DBL_SHA2_256, "dbl-sha2-256", 32) =>
        _algo = System.Security.Cryptography.SHA256.Create();

    /// <inheritdoc/>
    public override byte[] ComputeHash(byte[] data, int length = -1) => _algo.ComputeHash(_algo.ComputeHash(data));
}
