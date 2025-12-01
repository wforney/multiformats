using Murmur;
using System.Composition;
using System.Security.Cryptography;

namespace Multiformats.Hash.Algorithms;

/// <summary>
/// Implements the Murmur3 128-bit hash algorithm for multihash.
/// </summary>
[Export(typeof(IMultihashAlgorithm))]
[MultihashAlgorithmExport(HashType.MURMUR3_128, "murmur3-128", 16)]
public class MURMUR3_128 : MultihashAlgorithm
{
    /// <summary>
    /// Factory method for creating a Murmur3 128-bit hash algorithm instance.
    /// </summary>
    private readonly Func<HashAlgorithm> _factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="MURMUR3_128"/> class.
    /// </summary>
    public MURMUR3_128()
        : base(HashType.MURMUR3_128, "murmur3-128", 16) => _factory = () => MurmurHash.Create128(managed: false, preference: AlgorithmPreference.X64);

    /// <inheritdoc/>
    public override byte[] ComputeHash(byte[] data, int length = -1)
    {
        var value = _factory().ComputeHash(data);

        Array.Reverse(value, 0, 8);
        Array.Reverse(value, 8, 8);

        return value;
    }
}
