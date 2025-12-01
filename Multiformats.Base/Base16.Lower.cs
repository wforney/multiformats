namespace Multiformats.Base;

/// <summary>
/// Provides Base16 encoding and decoding using lowercase letters.
/// </summary>
internal class Base16Lower : Base16
{
    /// <summary>
    /// The Base16 alphabet using lowercase letters.
    /// </summary>
    private static readonly char[] _alphabet = "0123456789abcdef".ToCharArray();

    /// <inheritdoc/>
    protected override char[] Alphabet => _alphabet;

    /// <inheritdoc/>
    protected override string Name => "base16";

    /// <inheritdoc/>
    protected override char Prefix => 'f';

    /// <inheritdoc/>
    public override byte[] Decode(string input) => Decode(input, LetterCasing.Lower);

    /// <inheritdoc/>
    public override string Encode(byte[] bytes) => Encode(bytes, LetterCasing.Lower);
}
