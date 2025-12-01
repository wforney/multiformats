using BinaryEncoding;
using Multiformats.Base;
using Multiformats.Hash.Algorithms;
using System.Diagnostics.CodeAnalysis;

namespace Multiformats.Hash;

/// <summary>
/// Represents a multihash, a self-describing hash format supporting multiple hash algorithms.
/// </summary>
public class Multihash
{
    /// <summary>
    /// Supported multibase encodings for parsing and formatting multihashes.
    /// </summary>
    private static readonly MultibaseEncoding[] _encodings =
    [
        MultibaseEncoding.Base16Lower,
        MultibaseEncoding.Base32Lower,
        MultibaseEncoding.Base58Btc,
        MultibaseEncoding.Base64,
        MultibaseEncoding.Base2,
        MultibaseEncoding.Base8,
    ];

    /// <summary>
    /// Registry of supported hash algorithms.
    /// </summary>
    private static readonly Registry _registry = new();

    private readonly Lazy<byte[]> _bytes;

    /// <summary>
    /// Initializes a new instance of the <see cref="Multihash"/> class from raw bytes.
    /// </summary>
    /// <param name="bytes">The multihash-encoded byte array.</param>
    protected Multihash(byte[] bytes) => _bytes = new Lazy<byte[]>(() => bytes);

    /// <summary>
    /// Initializes a new instance of the <see cref="Multihash"/> class from hash code and digest.
    /// </summary>
    /// <param name="code">The hash algorithm code.</param>
    /// <param name="digest">The hash digest.</param>
    protected Multihash(HashType code, byte[] digest)
    {
        Code = code;
        Name = GetName((int)code);
        Digest = digest;

        _bytes = new Lazy<byte[]>(() => Encode(digest, code));
    }

    /// <summary>
    /// Gets the supported hash codes.
    /// </summary>
    public static HashType[] SupportedHashCodes => _registry.SupportedHashTypes;

    /// <summary>
    /// Gets the hash algorithm code.
    /// </summary>
    public HashType Code { get; }

    /// <summary>
    /// Gets the hash digest.
    /// </summary>
    public byte[] Digest { get; } = [];

    /// <summary>
    /// Gets the length of the digest.
    /// </summary>
    public int Length => Digest?.Length ?? 0;

    /// <summary>
    /// Gets the name of the hash algorithm.
    /// </summary>
    public string Name { get; } = string.Empty;

    /// <summary>
    /// Casts a byte array to a <see cref="Multihash"/> instance.
    /// </summary>
    /// <param name="buf">The byte array.</param>
    /// <returns>The <see cref="Multihash"/> instance.</returns>
    public static Multihash Cast(byte[] buf) => Decode(buf);

    /// <summary>
    /// Decodes a byte array into a <see cref="Multihash"/> instance.
    /// </summary>
    /// <param name="buf">The byte array.</param>
    /// <returns>The <see cref="Multihash"/> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="buf"/> is null.</exception>
    /// <exception cref="Exception">
    /// Thrown if <paramref name="buf"/> is too short or has inconsistent length.
    /// </exception>
    public static Multihash Decode(byte[] buf)
    {
        ArgumentNullException.ThrowIfNull(buf);

        if (buf.Length < 2)
        {
            throw new Exception("Too short");
        }

        var offset = Binary.Varint.Read(buf, 0, out uint code);
        offset += Binary.Varint.Read(buf, offset, out uint length);

        return length > buf.Length - offset ? throw new Exception("Incosistent length") : new Multihash((HashType)code, buf.Slice(offset));
    }

    /// <summary>
    /// Encodes data and hash code into a multihash byte array.
    /// </summary>
    /// <param name="data">The hash digest.</param>
    /// <param name="code">The hash algorithm code.</param>
    /// <returns>The multihash-encoded byte array.</returns>
    public static byte[] Encode(byte[] data, HashType code) =>
        Binary.Varint.GetBytes((uint)code).Concat(Binary.Varint.GetBytes((uint)data.Length), data);

    /// <summary>
    /// Encodes a base32 string and hash code into a <see cref="Multihash"/> instance.
    /// </summary>
    /// <param name="s">The base32-encoded string.</param>
    /// <param name="code">The hash algorithm code.</param>
    /// <returns>The <see cref="Multihash"/> instance.</returns>
    public static Multihash Encode(string s, HashType code) => Encode(Multibase.Base32.Decode(s), code);

    /// <summary>
    /// Encodes data using the specified algorithm type into a multihash byte array.
    /// </summary>
    /// <typeparam name="TAlgorithm">The hash algorithm type.</typeparam>
    /// <param name="data">The hash digest.</param>
    /// <returns>The multihash-encoded byte array.</returns>
    /// <exception cref="NotSupportedException">Thrown if the algorithm is not supported.</exception>
    public static byte[] Encode<TAlgorithm>(byte[] data) where TAlgorithm : IMultihashAlgorithm
    {
        var algo = Registry.GetHashType<TAlgorithm>();
        return algo.HasValue
            ? Binary.Varint.GetBytes((uint)algo.Value).Concat(Binary.Varint.GetBytes((uint)data.Length), data)
            : throw new NotSupportedException($"{typeof(TAlgorithm)} is not supported.");
    }

    /// <summary>
    /// Decodes a base58 string into a <see cref="Multihash"/> instance.
    /// </summary>
    /// <param name="s">The base58-encoded string.</param>
    /// <returns>The <see cref="Multihash"/> instance.</returns>
    [Obsolete("Use Parse/TryParse instead")]
    public static Multihash FromB58String(string s) => Cast(Multibase.Base58.Decode(s));

    /// <summary>
    /// Decodes a hex string into a <see cref="Multihash"/> instance.
    /// </summary>
    /// <param name="s">The hex-encoded string.</param>
    /// <returns>The <see cref="Multihash"/> instance.</returns>
    [Obsolete("Use Parse/TryParse instead")]
    public static Multihash FromHexString(string s) => Cast(Multibase.Base16.Decode(s));

    /// <summary>
    /// Gets the hash code for a given algorithm name.
    /// </summary>
    /// <param name="name">The algorithm name.</param>
    /// <returns>The hash code, or null if not found.</returns>
    public static HashType? GetCode(string name) =>
        Enum.TryParse(name.Replace("-", "_"), true, out HashType result) ? result : new HashType?();

    /// <summary>
    /// Gets the name of a hash algorithm from its code.
    /// </summary>
    /// <param name="code">The hash code.</param>
    /// <returns>The algorithm name.</returns>
    public static string GetName(HashType code) => code.ToString().Replace("_", "-").ToLower();

    /// <summary>
    /// Gets the name of a hash algorithm from its integer code.
    /// </summary>
    /// <param name="code">The integer hash code.</param>
    /// <returns>The algorithm name, or "unsupported" if not found.</returns>
    public static string GetName(int code) => Enum.IsDefined(typeof(HashType), (HashType)code) ? GetName((HashType)code) : "unsupported";

    /// <summary>
    /// Implicitly converts a <see cref="Multihash"/> to a byte array.
    /// </summary>
    /// <param name="mh">The <see cref="Multihash"/> instance.</param>
    public static implicit operator byte[](Multihash mh) => mh._bytes.Value;

    /// <summary>
    /// Implicitly converts a byte array to a <see cref="Multihash"/>.
    /// </summary>
    /// <param name="buf">The byte array.</param>
    public static implicit operator Multihash(byte[] buf) => Decode(buf);

    /// <summary>
    /// Implicitly converts a string to a <see cref="Multihash"/>.
    /// </summary>
    /// <param name="s">The string.</param>
    public static implicit operator Multihash(string s) => Parse(s);

    /// <summary>
    /// Implicitly converts a <see cref="Multihash"/> to a string using base16 encoding.
    /// </summary>
    /// <param name="mh">The <see cref="Multihash"/> instance.</param>
    public static implicit operator string(Multihash mh) => mh.ToString(MultibaseEncoding.Base16Lower);

    /// <summary>
    /// Parses a string into a <see cref="Multihash"/> instance.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <returns>The <see cref="Multihash"/> instance.</returns>
    /// <exception cref="FormatException">Thrown if the string is not a valid multihash.</exception>
    public static Multihash Parse(string s) => TryParse(s, out var mh) ? mh : throw new FormatException("Not a valid multihash");

    /// <summary>
    /// Parses a string with a specific encoding into a <see cref="Multihash"/> instance.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <param name="encoding">The multibase encoding.</param>
    /// <returns>The <see cref="Multihash"/> instance.</returns>
    /// <exception cref="FormatException">Thrown if the string is not a valid multihash.</exception>
    public static Multihash? Parse(string s, MultibaseEncoding encoding) =>
        TryParse(s, encoding, out var mh) ? mh : throw new FormatException("Not a valid multihash");

    /// <summary>
    /// Computes the multihash of the given data using the specified hash code.
    /// </summary>
    /// <param name="code">The hash algorithm code.</param>
    /// <param name="data">The input data.</param>
    /// <param name="length">The digest length, or -1 for default.</param>
    /// <returns>The <see cref="Multihash"/> instance.</returns>
    public static Multihash Sum(HashType code, byte[] data, int length = -1) => _registry.Use(code, algo => Sum(algo, data, length));

    /// <summary>
    /// Computes the multihash of the given data using the specified algorithm type.
    /// </summary>
    /// <typeparam name="TAlgorithm">The hash algorithm type.</typeparam>
    /// <param name="data">The input data.</param>
    /// <param name="length">The digest length, or -1 for default.</param>
    /// <returns>The <see cref="Multihash"/> instance.</returns>
    public static Multihash Sum<TAlgorithm>(byte[] data, int length = -1) where TAlgorithm : IMultihashAlgorithm =>
        _registry.Use<TAlgorithm, Multihash>(algo => Sum(algo, data, length));

    /// <summary>
    /// Asynchronously computes the multihash of the given data using the specified hash code.
    /// </summary>
    /// <param name="type">The hash algorithm code.</param>
    /// <param name="data">The input data.</param>
    /// <param name="length">The digest length, or -1 for default.</param>
    /// <returns>
    /// A task representing the asynchronous operation, with the <see cref="Multihash"/> result.
    /// </returns>
    public static Task<Multihash> SumAsync(HashType type, byte[] data, int length = -1) =>
        _registry.UseAsync(type, algo => SumAsync(algo, data, length));

    /// <summary>
    /// Asynchronously computes the multihash of the given data using the specified algorithm type.
    /// </summary>
    /// <typeparam name="TAlgorithm">The hash algorithm type.</typeparam>
    /// <param name="data">The input data.</param>
    /// <param name="length">The digest length, or -1 for default.</param>
    /// <returns>
    /// A task representing the asynchronous operation, with the <see cref="Multihash"/> result.
    /// </returns>
    public static Task<Multihash> SumAsync<TAlgorithm>(byte[] data, int length = -1) where TAlgorithm : IMultihashAlgorithm =>
        _registry.UseAsync<TAlgorithm, Multihash>(algo => SumAsync(algo, data, length));

    /// <summary>
    /// Attempts to parse a string into a <see cref="Multihash"/> and detects the encoding.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <param name="mh">The resulting <see cref="Multihash"/> instance, or null.</param>
    /// <param name="encoding">The detected encoding.</param>
    /// <returns>True if parsing succeeded; otherwise, false.</returns>
    public static bool TryParse(string s, [NotNullWhen(true)] out Multihash? mh, out MultibaseEncoding encoding)
    {
        foreach (var _encoding in _encodings)
        {
            try
            {
                encoding = _encoding;
                if (TryParse(s, encoding, out mh))
                {
                    return true;
                }
            }
            catch
            {
                // ignored
            }
        }

        mh = null;
        encoding = (MultibaseEncoding)(-1);
        return false;
    }

    /// <summary>
    /// Attempts to parse a string into a <see cref="Multihash"/>.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <param name="mh">The resulting <see cref="Multihash"/> instance, or null.</param>
    /// <returns>True if parsing succeeded; otherwise, false.</returns>
    public static bool TryParse(string s, [NotNullWhen(true)] out Multihash? mh) => TryParse(s, out mh, out _);

    /// <summary>
    /// Attempts to parse a string with a specific encoding into a <see cref="Multihash"/>.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <param name="encoding">The multibase encoding.</param>
    /// <param name="mh">The resulting <see cref="Multihash"/> instance, or null.</param>
    /// <returns>True if parsing succeeded; otherwise, false.</returns>
    public static bool TryParse(string s, MultibaseEncoding encoding, [NotNullWhen(true)] out Multihash? mh)
    {
        try
        {
            var bytes = Multibase.DecodeRaw(encoding, s);
            mh = Decode(bytes);
            return true;
        }
        catch
        {
            // ignored
        }

        mh = null;
        return false;
    }

    /// <summary>
    /// Returns the base58-encoded string representation of the multihash.
    /// </summary>
    /// <returns>The base58-encoded string.</returns>
    [Obsolete("Use ToString() instead")]
    public string B58String() => ToString(MultibaseEncoding.Base58Btc);

    /// <inheritdoc/>
    public override bool Equals(object? obj) =>
        obj as Multihash is not null && _bytes.Value.SequenceEqual(((Multihash)obj)._bytes.Value);

    /// <inheritdoc/>
    public override int GetHashCode() => (int)Code ^ Length ^ Digest.Sum(b => b);

    /// <summary>
    /// Gets the multihash-encoded byte array.
    /// </summary>
    /// <returns>The byte array.</returns>
    public byte[] ToBytes() => _bytes.Value;

    /// <summary>
    /// Returns the hex-encoded string representation of the multihash.
    /// </summary>
    /// <returns>The hex-encoded string.</returns>
    [Obsolete("Use ToString() instead")]
    public string ToHexString() => ToString(MultibaseEncoding.Base16Lower);

    /// <summary>
    /// Returns the string representation of the multihash using the specified encoding.
    /// </summary>
    /// <param name="encoding">The multibase encoding.</param>
    /// <returns>The encoded string.</returns>
    public string ToString(MultibaseEncoding encoding) => Multibase.EncodeRaw(encoding, _bytes.Value);

    /// <inheritdoc/>
    public override string ToString() => ToString(MultibaseEncoding.Base16Lower);

    /// <summary>
    /// Verifies that the given data matches the digest of this multihash.
    /// </summary>
    /// <param name="data">The data to verify.</param>
    /// <returns>True if the data matches; otherwise, false.</returns>
    public bool Verify(byte[] data) => Sum(Code, data, Length).Equals(this);

    /// <summary>
    /// Asynchronously verifies that the given data matches the digest of this multihash.
    /// </summary>
    /// <param name="data">The data to verify.</param>
    /// <returns>A task representing the asynchronous operation, with the verification result.</returns>
    public Task<bool> VerifyAsync(byte[] data) =>
        SumAsync(Code, data, Length).ContinueWith(mh => mh.Result?.Equals(this) ?? false);

    private static Multihash Sum(IMultihashAlgorithm algo, byte[] data, int length) =>
        new(
            algo.Code,
            algo
                .ComputeHash(data, length)
                .Slice(0, length != -1 ? length : algo.DefaultLength));

    private static async Task<Multihash> SumAsync(IMultihashAlgorithm algo, byte[] data, int length)
    {
        var hash = await algo.ComputeHashAsync(data, length);
        var count = length != -1 ? length : algo.DefaultLength;
        return new Multihash(
            algo.Code,
            hash.Slice(0, count));
    }
}
