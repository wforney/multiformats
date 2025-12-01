namespace Multiformats.Base;

/// <summary>
/// Provides BASE32HEX encoding and decoding using uppercase letters.
/// </summary>
internal class Base32HexUpper : Base32
{
    /// <summary>
    /// The BASE32HEX alphabet (uppercase).
    /// </summary>
    private static readonly char[] _alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUV".ToCharArray();

    /// <inheritdoc/>
    protected override char[] Alphabet => _alphabet;

    /// <inheritdoc/>
    protected override string Name => "BASE32HEX";

    /// <inheritdoc/>
    protected override char Prefix => 'V';

    /// <inheritdoc/>
    public override byte[] Decode(string input) => Decode(input, false, LetterCasing.Upper);

    /// <inheritdoc/>
    public override string Encode(byte[] bytes) => Encode(bytes, false);
}
