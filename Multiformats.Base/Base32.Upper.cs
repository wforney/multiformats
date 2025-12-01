namespace Multiformats.Base;

/// <summary>
/// Provides BASE32 encoding and decoding using the upper-case alphabet.
/// </summary>
internal class Base32Upper : Base32
{
    /// <summary>
    /// The BASE32 upper-case alphabet.
    /// </summary>
    private static readonly char[] _alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567".ToCharArray();

    /// <inheritdoc/>
    protected override char[] Alphabet => _alphabet;

    /// <inheritdoc/>
    protected override string Name => "BASE32";

    /// <inheritdoc/>
    protected override char Prefix => 'B';

    /// <inheritdoc/>
    public override byte[] Decode(string input) => Decode(input, false, LetterCasing.Upper);

    /// <inheritdoc/>
    public override string Encode(byte[] bytes) => Encode(bytes, false);
}
