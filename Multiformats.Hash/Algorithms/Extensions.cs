using Org.BouncyCastle.Crypto;

namespace Multiformats.Hash.Algorithms;

/// <summary>
/// Provides extension methods for <see cref="IDigest"/>.
/// </summary>
internal static class Extensions
{
    /// <summary>
    /// Computes the hash value for the specified data using the given <see cref="IDigest"/> instance.
    /// </summary>
    /// <param name="digest">The digest algorithm to use for hashing.</param>
    /// <param name="data">The input data to hash.</param>
    /// <returns>The computed hash as a byte array.</returns>
    public static byte[] ComputeHash(this IDigest digest, byte[] data)
    {
        digest.Reset();
        digest.BlockUpdate(data, 0, data.Length);
        var hash = new byte[digest.GetByteLength()];
        _ = digest.DoFinal(hash, 0);
        return hash;
    }
}
