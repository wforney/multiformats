namespace Multiformats.Base;

/// <summary>
/// Provides Base64 encoding and decoding with padding support.
/// </summary>
internal class Base64Padded : Base64
{
    /// <summary>
    /// The Base64 alphabet including the padding character '='.
    /// </summary>
    private static readonly char[] _alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=".ToCharArray();

    /// <inheritdoc/>
    protected override char[] Alphabet => _alphabet;

    /// <inheritdoc/>
    protected override string Name => "base64pad";

    /// <inheritdoc/>
    protected override char Prefix => 'M';

    /// <inheritdoc/>
    public override byte[] Decode(string input) => Decode(input, false, true);

    /// <inheritdoc/>
    public override string Encode(byte[] bytes) => Encode(bytes, false, true);
}
