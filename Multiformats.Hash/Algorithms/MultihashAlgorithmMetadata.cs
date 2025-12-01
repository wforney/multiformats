namespace Multiformats.Hash.Algorithms;

/// <summary>
/// Represents metadata for a multihash algorithm.
/// </summary>
public class MultihashAlgorithmMetadata : IMultihashAlgorithmMetadata
{
    /// <inheritdoc/>
    public HashType Code { get; set; }

    /// <inheritdoc/>
    public int DefaultLength { get; set; }

    /// <inheritdoc/>
    public string Name { get; set; } = default!;
}
