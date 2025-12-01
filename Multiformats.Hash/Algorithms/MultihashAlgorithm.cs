namespace Multiformats.Hash.Algorithms;

/// <summary>
/// Represents an abstract base class for multihash algorithms.
/// </summary>
public abstract class MultihashAlgorithm : IMultihashAlgorithm
{
    /// <summary>
    /// Provides a thread-safe random number generator for hash code calculation.
    /// </summary>
    private static readonly Lazy<Random> _random = new(() => new Random(Environment.TickCount));

    /// <summary>
    /// Lazily computes the hash code for the algorithm instance.
    /// </summary>
    private readonly Lazy<int> _hashCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="MultihashAlgorithm"/> class.
    /// </summary>
    /// <param name="code">The hash type code.</param>
    /// <param name="name">The name of the algorithm.</param>
    /// <param name="defaultLength">The default output length of the hash.</param>
    protected MultihashAlgorithm(HashType code, string name, int defaultLength)
    {
        Code = code;
        Name = name;
        DefaultLength = defaultLength;

        _hashCode = new Lazy<int>(() => (int)Code ^ Name.GetHashCode() ^ DefaultLength ^ _random.Value.Next());
    }

    /// <inheritdoc/>
    public HashType Code { get; }

    /// <inheritdoc/>
    public int DefaultLength { get; }

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public abstract byte[] ComputeHash(byte[] data, int length = -1);

    /// <inheritdoc/>
    public virtual Task<byte[]> ComputeHashAsync(byte[] data, int length = -1) => Task.Factory.StartNew(() => ComputeHash(data, length));

    /// <inheritdoc/>
    public override int GetHashCode() => _hashCode.Value;
}
