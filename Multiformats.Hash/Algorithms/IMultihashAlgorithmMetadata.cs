namespace Multiformats.Hash.Algorithms;

/// <summary>
/// Provides metadata for a multihash algorithm.
/// </summary>
public interface IMultihashAlgorithmMetadata
{
    /// <summary>
    /// Gets the code representing the hash algorithm.
    /// </summary>
    HashType Code { get; }

    /// <summary>
    /// Gets the default length of the hash output in bytes.
    /// </summary>
    int DefaultLength { get; }

    /// <summary>
    /// Gets the name of the hash algorithm.
    /// </summary>
    string Name { get; }
}
