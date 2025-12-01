namespace Multiformats.Hash.Algorithms;

/// <summary>
/// Represents a multihash algorithm implementation.
/// </summary>
public interface IMultihashAlgorithm
{
    /// <summary>
    /// Gets the code identifying the hash algorithm.
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

    /// <summary>
    /// Computes the hash of the specified data.
    /// </summary>
    /// <param name="data">The input data to hash.</param>
    /// <param name="length">
    /// The desired length of the hash output in bytes. If -1, uses the default length.
    /// </param>
    /// <returns>The computed hash as a byte array.</returns>
    byte[] ComputeHash(byte[] data, int length = -1);

    /// <summary>
    /// Asynchronously computes the hash of the specified data.
    /// </summary>
    /// <param name="data">The input data to hash.</param>
    /// <param name="length">
    /// The desired length of the hash output in bytes. If -1, uses the default length.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the computed
    /// hash as a byte array.
    /// </returns>
    Task<byte[]> ComputeHashAsync(byte[] data, int length = -1);
}
