namespace Multiformats.Base;

/// <summary>
/// Provides Base32 encoding and decoding using lowercase letters and padding.
/// </summary>
internal class Base32PaddedLower : Base32
{
    /// <summary>
    /// The Base32 alphabet using lowercase letters and padding character '='.
    /// </summary>
    private static readonly char[] _alphabet = "abcdefghijklmnopqrstuvwxyz234567=".ToCharArray();

    /// <inheritdoc/>
    protected override char[] Alphabet => _alphabet;

    /// <inheritdoc/>
    protected override string Name => "base32pad";

    /// <inheritdoc/>
    protected override char Prefix => 'c';

    /// <inheritdoc/>
    public override byte[] Decode(string input) => Decode(input, true, LetterCasing.Lower);

    /// <inheritdoc/>
    public override string Encode(byte[] bytes) => Encode(bytes, true);
}
