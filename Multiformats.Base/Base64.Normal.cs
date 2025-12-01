namespace Multiformats.Base;

/// <summary>
/// Represents the standard Base64 encoding implementation.
/// </summary>
internal class Base64Normal : Base64
{
    /// <summary>
    /// The Base64 alphabet used for encoding and decoding.
    /// </summary>
    private static readonly char[] _alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/".ToCharArray();

    /// <inheritdoc/>
    protected override char[] Alphabet => _alphabet;

    /// <inheritdoc/>
    protected override string Name => "base64";

    /// <inheritdoc/>
    protected override char Prefix => 'm';

    /// <inheritdoc/>
    public override byte[] Decode(string input) => Decode(input, false, false);

    /// <inheritdoc/>
    public override string Encode(byte[] bytes) => Encode(bytes, false, false);
}
