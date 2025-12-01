namespace Multiformats.Base;

/// <summary>
/// Abstract base class for multibase encoding and decoding operations. Provides static methods for
/// encoding and decoding data using various multibase encodings.
/// </summary>
public abstract class Multibase
{
    /// <summary>
    /// Dictionary mapping <see cref="MultibaseEncoding"/> to their corresponding <see
    /// cref="Multibase"/> implementations.
    /// </summary>
    protected static readonly Dictionary<MultibaseEncoding, Multibase> _bases;

    /// <summary>
    /// Static constructor initializes the supported multibase encodings.
    /// </summary>
    static Multibase()
    {
        _bases = new Dictionary<MultibaseEncoding, Multibase>
        {
            {MultibaseEncoding.Identity, new Identity()},
            {MultibaseEncoding.Base2, new Base2()},
            {MultibaseEncoding.Base8, new Base8()},
            {MultibaseEncoding.Base10, new Base10()},
            {MultibaseEncoding.Base16Lower, new Base16Lower()},
            {MultibaseEncoding.Base16Upper, new Base16Upper()},
            {MultibaseEncoding.Base32Lower, new Base32Lower()},
            {MultibaseEncoding.Base32Upper, new Base32Upper()},
            {MultibaseEncoding.Base32PaddedLower, new Base32PaddedLower()},
            {MultibaseEncoding.Base32PaddedUpper, new Base32PaddedUpper()},
            {MultibaseEncoding.Base32HexLower, new Base32HexLower()},
            {MultibaseEncoding.Base32HexUpper, new Base32HexUpper()},
            {MultibaseEncoding.Base32HexPaddedLower, new Base32HexPaddedLower()},
            {MultibaseEncoding.Base32HexPaddedUpper, new Base32HexPaddedUpper()},
            {MultibaseEncoding.Base32Z, new Base32Z()},
            {MultibaseEncoding.Base58Btc, new Base58Btc()},
            {MultibaseEncoding.Base58Flickr, new Base58Flickr()},
            {MultibaseEncoding.Base64, new Base64Normal()},
            {MultibaseEncoding.Base64Padded, new Base64Padded()},
            {MultibaseEncoding.Base64Url, new Base64Url()},
            {MultibaseEncoding.Base64UrlPadded, new Base64UrlPadded()},
        };
    }

    /// <summary>
    /// Gets the <see cref="Multibase"/> instance for Base10 encoding.
    /// </summary>
    public static Multibase Base10 => _bases[MultibaseEncoding.Base10];

    /// <summary>
    /// Gets the <see cref="Multibase"/> instance for Base16Lower encoding.
    /// </summary>
    public static Multibase Base16 => _bases[MultibaseEncoding.Base16Lower];

    /// <summary>
    /// Gets the <see cref="Multibase"/> instance for Base2 encoding.
    /// </summary>
    public static Multibase Base2 => _bases[MultibaseEncoding.Base2];

    /// <summary>
    /// Gets the <see cref="Multibase"/> instance for Base32Lower encoding.
    /// </summary>
    public static Multibase Base32 => _bases[MultibaseEncoding.Base32Lower];

    /// <summary>
    /// Gets the <see cref="Multibase"/> instance for Base58Btc encoding.
    /// </summary>
    public static Multibase Base58 => _bases[MultibaseEncoding.Base58Btc];

    /// <summary>
    /// Gets the <see cref="Multibase"/> instance for Base64 encoding.
    /// </summary>
    public static Multibase Base64 => _bases[MultibaseEncoding.Base64];

    /// <summary>
    /// Gets the <see cref="Multibase"/> instance for Base8 encoding.
    /// </summary>
    public static Multibase Base8 => _bases[MultibaseEncoding.Base8];

    /// <summary>
    /// Gets the alphabet used for encoding and decoding.
    /// </summary>
    protected abstract char[] Alphabet { get; }

    /// <summary>
    /// Gets the name of the encoding.
    /// </summary>
    protected abstract string Name { get; }

    /// <summary>
    /// Gets the prefix character for the encoding.
    /// </summary>
    protected abstract char Prefix { get; }

    /// <summary>
    /// Decodes a multibase encoded string.
    /// </summary>
    /// <param name="input">Encoded string.</param>
    /// <param name="encoding">Encoding used.</param>
    /// <param name="strict">If true, disallows non-valid characters for the given encoding.</param>
    /// <returns>Decoded bytes.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="input"/> is null or empty.
    /// </exception>
    /// <exception cref="NotSupportedException">Thrown if the encoding prefix is unknown.</exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the input contains invalid characters for the encoding.
    /// </exception>
    public static byte[] Decode(string input, out MultibaseEncoding encoding, bool strict = true)
    {
        if (string.IsNullOrEmpty(input))
        {
            throw new ArgumentNullException(nameof(input));
        }

        var @base = _bases.Values.SingleOrDefault(b => b.Prefix == input[0])
            ?? throw new NotSupportedException($"{input[0]} is an unknown encoding prefix.");

        var value = input[1..];
        encoding = _bases.SingleOrDefault(kv => kv.Value.Equals(@base)).Key;

        return strict && !@base.IsValid(value)
            ? throw new InvalidOperationException($"{value} contains invalid chars for {encoding}.")
            : @base.Decode(value);
    }

    /// <summary>
    /// Decodes a multibase encoded string.
    /// </summary>
    /// <param name="input">Encoded string.</param>
    /// <param name="encoding">Encoding name used.</param>
    /// <param name="strict">If true, disallows non-valid characters for the given encoding.</param>
    /// <returns>Decoded bytes.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="input"/> is null or empty.
    /// </exception>
    /// <exception cref="NotSupportedException">Thrown if the encoding prefix is unknown.</exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the input contains invalid characters for the encoding.
    /// </exception>
    public static byte[] Decode(string input, out string encoding, bool strict = true)
    {
        if (string.IsNullOrEmpty(input))
        {
            throw new ArgumentNullException(nameof(input));
        }

        var @base = _bases.Values.SingleOrDefault(b => b.Prefix == input[0])
            ?? throw new NotSupportedException($"{input[0]} is an unknown encoding prefix.");

        var value = input[1..];
        encoding = @base.Name;

        return strict && !@base.IsValid(value)
            ? throw new InvalidOperationException($"{value} contains invalid chars for {encoding}.")
            : @base.Decode(value);
    }

    /// <summary>
    /// Decodes an encoded string using the given encoding (without multibase prefix).
    /// </summary>
    /// <param name="encoding">Encoding to use.</param>
    /// <param name="input">Encoded string.</param>
    /// <returns>Decoded bytes.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="input"/> is null or empty.
    /// </exception>
    /// <exception cref="NotSupportedException">Thrown if the encoding is unknown.</exception>
    public static byte[] DecodeRaw(MultibaseEncoding encoding, string input)
    {
        return string.IsNullOrEmpty(input)
            ? throw new ArgumentNullException(nameof(input))
            : !_bases.TryGetValue(encoding, out var @base)
            ? throw new NotSupportedException($"{encoding} is an unknown encoding.")
            : @base.Decode(input);
    }

    /// <summary>
    /// Encodes a byte array to a multibase string using the given encoding.
    /// </summary>
    /// <param name="encoding">Encoding to use.</param>
    /// <param name="bytes">Bytes to encode.</param>
    /// <returns>Encoded string.</returns>
    /// <exception cref="NotSupportedException">Thrown if the encoding is not supported.</exception>
    public static string Encode(MultibaseEncoding encoding, byte[] bytes)
    {
        return !_bases.TryGetValue(encoding, out var @base)
            ? throw new NotSupportedException($"{encoding} is not supported.")
            : Encode(@base, bytes, true);
    }

    /// <summary>
    /// Encodes a byte array to a multibase string using the given encoding name.
    /// </summary>
    /// <param name="encoding">Encoding name to use.</param>
    /// <param name="bytes">Bytes to encode.</param>
    /// <returns>Encoded string.</returns>
    /// <exception cref="NotSupportedException">Thrown if the encoding is not supported.</exception>
    public static string Encode(string encoding, byte[] bytes)
    {
        var @base = _bases.Values.SingleOrDefault(b => b.Name.Equals(encoding));
        return @base is null ? throw new NotSupportedException($"{encoding} is not supported.") : Encode(@base, bytes, true);
    }

    /// <summary>
    /// Encodes a byte array using the given encoding (without multibase prefix).
    /// </summary>
    /// <param name="encoding">Encoding to use.</param>
    /// <param name="bytes">Bytes to encode.</param>
    /// <returns>Encoded string.</returns>
    /// <exception cref="NotSupportedException">Thrown if the encoding is not supported.</exception>
    public static string EncodeRaw(MultibaseEncoding encoding, byte[] bytes)
    {
        var @base = _bases[encoding];
        return @base is null ? throw new NotSupportedException($"{encoding} is not supported.") : Encode(@base, bytes, false);
    }

    /// <summary>
    /// Tries to decode an encoded string. If prefixed with a multibase prefix, it's guaranteed to
    /// give the correct value; if not, there's no guarantee it will pick the right encoding.
    /// </summary>
    /// <param name="input">Encoded string.</param>
    /// <param name="encoding">Guessed encoding.</param>
    /// <param name="bytes">Decoded bytes.</param>
    /// <returns>True on success (no guarantee it's correct), false on error.</returns>
    public static bool TryDecode(string input, out MultibaseEncoding encoding, out byte[]? bytes)
    {
        try
        {
            // special case for base2 without prefix
            if (input[0] == '0' && input.Length % 8 == 0)
            {
                throw new Exception();
            }

            bytes = Decode(input, out encoding);
            return true;
        }
        catch
        {
            foreach (var @base in _bases.Values.Skip(1).Where(b => b.IsValid(input)))
            {
                try
                {
                    bytes = @base.Decode(input);
                    encoding = _bases.SingleOrDefault(kv => kv.Value.Equals(@base)).Key;
                    return true;
                }
                catch
                {
                }
            }
        }

        encoding = default;
        bytes = null;
        return false;
    }

    /// <summary>
    /// Decodes the input string using the specific encoding implementation.
    /// </summary>
    /// <param name="input">Encoded string.</param>
    /// <returns>Decoded bytes.</returns>
    public abstract byte[] Decode(string input);

    /// <summary>
    /// Encodes the input bytes using the specific encoding implementation.
    /// </summary>
    /// <param name="bytes">Bytes to encode.</param>
    /// <returns>Encoded string.</returns>
    public abstract string Encode(byte[] bytes);

    /// <summary>
    /// Checks if the value contains only valid characters for the encoding's alphabet.
    /// </summary>
    /// <param name="value">String to validate.</param>
    /// <returns>True if valid; otherwise, false.</returns>
    protected virtual bool IsValid(string value) => value.Distinct().All(c => Array.IndexOf(Alphabet, c) > -1);

    /// <summary>
    /// Encodes the bytes using the specified <see cref="Multibase"/> instance, optionally including
    /// the prefix.
    /// </summary>
    /// <param name="base">Multibase instance.</param>
    /// <param name="bytes">Bytes to encode.</param>
    /// <param name="prefix">Whether to include the multibase prefix.</param>
    /// <returns>Encoded string.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="bytes"/> is null or empty.
    /// </exception>
    private static string Encode(Multibase @base, byte[] bytes, bool prefix)
    {
        return bytes is null || bytes.Length == 0
            ? throw new ArgumentNullException(nameof(bytes))
            : prefix ? @base.Prefix + @base.Encode(bytes) : @base.Encode(bytes);
    }
}
