namespace Multiformats.Base;

/// <summary>
/// Provides BASE32 encoding and decoding using uppercase letters and padding.
/// </summary>
internal class Base32PaddedUpper : Base32
{
    /// <summary>
    /// The BASE32 alphabet with padding character.
    /// </summary>
    private static readonly char[] _alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567=".ToCharArray();

    /// <inheritdoc/>
    protected override char[] Alphabet => _alphabet;

    /// <inheritdoc/>
    protected override string Name => "BASE32PAD";

    /// <inheritdoc/>
    protected override char Prefix => 'C';

    /// <inheritdoc/>
    public override byte[] Decode(string input) => Decode(input, true, LetterCasing.Upper);

    /// <inheritdoc/>
    public override string Encode(byte[] bytes) => Encode(bytes, true);
}
