using System.Composition;

namespace Multiformats.Hash.Algorithms;

/// <summary>
/// Represents the identity hash algorithm implementation for Multihash.
/// </summary>
[Export(typeof(IMultihashAlgorithm))]
[MultihashAlgorithmExport(HashType.ID, "id", 32)]
public class ID : MultihashAlgorithm
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ID"/> class.
    /// </summary>
    public ID()
        : base(HashType.ID, "id", 32)
    {
    }

    /// <inheritdoc/>
    public override byte[] ComputeHash(byte[] data, int length = -1)
    {
        return length >= 0 && length != data.Length
            ? throw new Exception($"The length of the identity hash ({length}) must be equal to the length of the data ({data.Length})")
            : data;
    }
}
