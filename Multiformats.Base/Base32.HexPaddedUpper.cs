namespace Multiformats.Base;

/// <summary>
/// Provides BASE32HEXPAD encoding and decoding using the uppercase hexadecimal alphabet with padding.
/// </summary>
internal class Base32HexPaddedUpper : Base32
{
    /// <summary>
    /// The BASE32HEXPAD alphabet (uppercase hexadecimal digits and padding).
    /// </summary>
    private static readonly char[] _alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUV=".ToCharArray();

    /// <inheritdoc/>
    protected override char[] Alphabet => _alphabet;

    /// <inheritdoc/>
    protected override string Name => "BASE32HEXPAD";

    /// <inheritdoc/>
    protected override char Prefix => 'T';

    /// <inheritdoc/>
    public override byte[] Decode(string input) => Decode(input, true, LetterCasing.Upper);

    /// <inheritdoc/>
    public override string Encode(byte[] bytes) => Encode(bytes, true);
}
