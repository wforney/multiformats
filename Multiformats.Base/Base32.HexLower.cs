namespace Multiformats.Base;

/// <summary>
/// Provides Base32 encoding and decoding using the hexadecimal alphabet in lowercase.
/// </summary>
internal class Base32HexLower : Base32
{
    /// <summary>
    /// The Base32 hexadecimal alphabet in lowercase.
    /// </summary>
    private static readonly char[] _alphabet = "0123456789abcdefghijklmnopqrstuv".ToCharArray();

    /// <inheritdoc/>
    protected override char[] Alphabet => _alphabet;

    /// <inheritdoc/>
    protected override string Name => "base32hex";

    /// <inheritdoc/>
    protected override char Prefix => 'v';

    /// <inheritdoc/>
    public override byte[] Decode(string input) => Decode(input, false, LetterCasing.Lower);

    /// <inheritdoc/>
    public override string Encode(byte[] bytes) => Encode(bytes, false);
}
