namespace Multiformats.Base;

/// <summary>
/// Provides Base32 encoding and decoding using the hexadecimal alphabet with padding, in lower case.
/// </summary>
internal class Base32HexPaddedLower : Base32
{
    /// <summary>
    /// The Base32 hexadecimal alphabet in lower case, including the padding character '='.
    /// </summary>
    private static readonly char[] _alphabet = "0123456789abcdefghijklmnopqrstuv=".ToCharArray();

    /// <inheritdoc/>
    protected override char[] Alphabet => _alphabet;

    /// <inheritdoc/>
    protected override string Name => "base32hexpad";

    /// <inheritdoc/>
    protected override char Prefix => 't';

    /// <inheritdoc/>
    public override byte[] Decode(string input) => Decode(input, true, LetterCasing.Lower);

    /// <inheritdoc/>
    public override string Encode(byte[] bytes) => Encode(bytes, true);
}
