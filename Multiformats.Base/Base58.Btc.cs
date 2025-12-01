namespace Multiformats.Base;

/// <summary>
/// Provides Base58 encoding and decoding using the Bitcoin alphabet.
/// </summary>
internal class Base58Btc : Base58
{
    /// <summary>
    /// The Base58 alphabet used by Bitcoin.
    /// </summary>
    private static readonly char[] _alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz".ToCharArray();

    /// <inheritdoc/>
    protected override char[] Alphabet => _alphabet;

    /// <inheritdoc/>
    protected override string Name => "base58btc";

    /// <inheritdoc/>
    protected override char Prefix => 'z';

    /// <inheritdoc/>
    public override byte[] Decode(string input) => Decode(input, _alphabet);

    /// <inheritdoc/>
    public override string Encode(byte[] bytes) => Encode(bytes, _alphabet);
}
