namespace Multiformats.Base;

/// <summary>
/// Provides encoding and decoding functionality for the Base32z variant.
/// </summary>
internal class Base32Z : Base32
{
    /// <summary>
    /// The Base32z alphabet.
    /// </summary>
    private static readonly char[] _alphabet = "ybndrfg8ejkmcpqxot1uwisza345h769".ToCharArray();

    /// <inheritdoc/>
    protected override char[] Alphabet => _alphabet;

    /// <inheritdoc/>
    protected override string Name => "base32z";

    /// <inheritdoc/>
    protected override char Prefix => 'h';

    /// <inheritdoc/>
    public override byte[] Decode(string input) => Decode(input, false, LetterCasing.Ignore);

    /// <inheritdoc/>
    public override string Encode(byte[] bytes) => Encode(bytes, false);
}
