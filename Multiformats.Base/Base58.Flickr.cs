namespace Multiformats.Base;

/// <summary>
/// Provides Base58 encoding and decoding using the Flickr alphabet.
/// </summary>
internal class Base58Flickr : Base58
{
    /// <summary>
    /// The Flickr Base58 alphabet.
    /// </summary>
    private static readonly char[] _alphabet = "123456789abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ".ToCharArray();

    /// <inheritdoc/>
    protected override char[] Alphabet => _alphabet;

    /// <inheritdoc/>
    protected override string Name => "base58flickr";

    /// <inheritdoc/>
    protected override char Prefix => 'Z';

    /// <inheritdoc/>
    public override byte[] Decode(string input) => Decode(input, _alphabet);

    /// <inheritdoc/>
    public override string Encode(byte[] bytes) => Encode(bytes, _alphabet);
}
