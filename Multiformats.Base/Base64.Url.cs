namespace Multiformats.Base;

/// <summary>
/// Provides Base64Url encoding and decoding functionality.
/// </summary>
internal class Base64Url : Base64
{
    /// <summary>
    /// The Base64Url alphabet.
    /// </summary>
    private static readonly char[] _alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_".ToCharArray();

    /// <inheritdoc/>
    protected override char[] Alphabet => _alphabet;

    /// <inheritdoc/>
    protected override string Name => "base64url";

    /// <inheritdoc/>
    protected override char Prefix => 'u';

    /// <inheritdoc/>
    public override byte[] Decode(string input) => Decode(input, true, false);

    /// <inheritdoc/>
    public override string Encode(byte[] bytes) => Encode(bytes, true, false);
}
