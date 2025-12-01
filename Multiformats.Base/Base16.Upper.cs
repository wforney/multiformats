namespace Multiformats.Base;

/// <summary>
/// Provides BASE16 encoding and decoding using uppercase hexadecimal characters.
/// </summary>
internal class Base16Upper : Base16
{
    /// <summary>
    /// The uppercase hexadecimal alphabet used for encoding.
    /// </summary>
    private static readonly char[] _alphabet = "0123456789ABCDEF".ToCharArray();

    /// <inheritdoc/>
    protected override char[] Alphabet => _alphabet;

    /// <inheritdoc/>
    protected override string Name => "BASE16";

    /// <inheritdoc/>
    protected override char Prefix => 'F';

    /// <inheritdoc/>
    public override byte[] Decode(string input) => Decode(input, LetterCasing.Upper);

    /// <inheritdoc/>
    public override string Encode(byte[] bytes) => Encode(bytes, LetterCasing.Upper);
}
