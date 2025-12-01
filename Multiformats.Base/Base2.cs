namespace Multiformats.Base;

/// <summary>
/// Provides encoding and decoding functionality for Base2 (binary) multibase.
/// </summary>
internal class Base2 : Multibase
{
    /// <summary>
    /// The alphabet used for Base2 encoding.
    /// </summary>
    private static readonly char[] _alphabet = ['0', '1'];

    /// <inheritdoc/>
    protected override char[] Alphabet => _alphabet;

    /// <inheritdoc/>
    protected override string Name => "base2";

    /// <inheritdoc/>
    protected override char Prefix => '0';

    /// <inheritdoc/>
    public override byte[] Decode(string input)
    {
        var bytes = new byte[input.Length / 8];
        for (var index = 0; index < input.Length / 8; index++)
        {
            for (var i = 0; i < 8; i++)
            {
                if (input[(index * 8) + i] == '1')
                {
                    bytes[index] |= (byte)(1 << (7 - i));
                }
            }
        }

        return bytes;
    }

    /// <inheritdoc/>
    public override string Encode(byte[] bytes) =>
        new([.. bytes
            .Select(
                b =>
                    Enumerable
                        .Range(0, 8)
                        .Select(i => (b & (1 << i)) != 0 ? '1' : '0')
                        .Reverse())
            .SelectMany(b => b)]);
}
