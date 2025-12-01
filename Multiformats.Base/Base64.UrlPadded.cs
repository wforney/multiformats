namespace Multiformats.Base;

/// <summary>
/// Provides Base64 encoding and decoding using the URL-safe alphabet with padding.
/// </summary>
internal class Base64UrlPadded : Base64
{
    /// <summary>
    /// The URL-safe Base64 alphabet with padding character.
    /// </summary>
    private static readonly char[] _alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_=".ToCharArray();

    /// <inheritdoc/>
    protected override char[] Alphabet => _alphabet;

    /// <inheritdoc/>
    protected override string Name => "base64urlpad";

    /// <inheritdoc/>
    protected override char Prefix => 'U';

    /// <inheritdoc/>
    public override byte[] Decode(string input) => Decode(input, true, true);

    /// <inheritdoc/>
    public override string Encode(byte[] bytes) => Encode(bytes, true, true);
}
