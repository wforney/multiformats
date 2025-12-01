namespace Multiformats.Base;

/// <summary>
/// Provides Base32 encoding and decoding using the lowercase alphabet.
/// </summary>
internal class Base32Lower : Base32
{
    /// <summary>
    /// The Base32 lowercase alphabet.
    /// </summary>
    private static readonly char[] _alphabet = "abcdefghijklmnopqrstuvwxyz234567".ToCharArray();

    /// <inheritdoc/>
    protected override char[] Alphabet => _alphabet;

    /// <inheritdoc/>
    protected override string Name => "base32";

    /// <inheritdoc/>
    protected override char Prefix => 'b';

    /// <inheritdoc/>
    public override byte[] Decode(string input) => Decode(input, false, LetterCasing.Lower);

    /// <inheritdoc/>
    public override string Encode(byte[] bytes) => Encode(bytes, false);
}
