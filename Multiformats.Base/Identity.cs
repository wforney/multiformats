namespace Multiformats.Base;

/// <summary>
/// Represents the identity multibase encoding, which performs no transformation.
/// </summary>
internal class Identity : Multibase
{
    /// <inheritdoc/>
    protected override char[] Alphabet => [];

    /// <inheritdoc/>
    protected override string Name => "identity";

    /// <inheritdoc/>
    protected override char Prefix => '\0';

    /// <inheritdoc/>
    public override byte[] Decode(string input) => [.. input.Select(Convert.ToByte)];

    /// <inheritdoc/>
    public override string Encode(byte[] bytes) => new([.. bytes.Select(Convert.ToChar)]);

    /// <inheritdoc/>
    protected override bool IsValid(string value) => true;
}
