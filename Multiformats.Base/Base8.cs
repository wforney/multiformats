namespace Multiformats.Base;

/// <summary>
/// Provides encoding and decoding functionality for Base8 multibase.
/// </summary>
internal class Base8 : Multibase
{
    /// <summary>
    /// The Base8 alphabet.
    /// </summary>
    private static readonly char[] _alphabet = ['0', '1', '2', '3', '4', '5', '6', '7'];

    /// <inheritdoc/>
    protected override char[] Alphabet => _alphabet;

    /// <inheritdoc/>
    protected override string Name => "base8";

    /// <inheritdoc/>
    protected override char Prefix => '7';

    /// <inheritdoc/>
    public override byte[] Decode(string input)
    {
        var base2 = _bases[MultibaseEncoding.Base2];

        var bin = input.Select(ToNum8).SelectMany(FromOct).Select(FromBit);

        var modlen = input.Length % 8;

        var binstr = modlen switch
        {
            0 => new string([.. bin]),
            3 => new string([.. bin.Skip(1)]),
            6 => new string([.. bin.Skip(2)]),
            _ => string.Empty,
        };
        return base2.Decode(binstr);
    }

    /// <inheritdoc/>
    public override string Encode(byte[] bytes)
    {
        var base2 = _bases[MultibaseEncoding.Base2];
        var encoded = base2.Encode(bytes);
        var modlen = encoded.Length % 3;
        var prepad = new string('0', modlen == 0 ? 0 : 3 - modlen);

        return new string([.. BinToOct((prepad + encoded).Select(ToBit)).Select(FromNum8)]);
    }

    /// <summary>
    /// Converts a sequence of bits to octal values.
    /// </summary>
    /// <param name="b">The sequence of bits.</param>
    /// <returns>A sequence of octal values.</returns>
    private static List<byte> BinToOct(IEnumerable<byte> b)
    {
        var result = new List<byte>();
        var batch = new List<byte>();
        foreach (var x in b)
        {
            batch.Add(x);

            if (batch.Count == 3)
            {
                result.Add(ToOct(batch));
                batch.Clear();
            }
        }

        return result;
    }

    /// <summary>
    /// Converts a bit value to its character representation.
    /// </summary>
    /// <param name="b">The bit value.</param>
    /// <returns>'0' if <paramref name="b"/> is 0; otherwise, '1'.</returns>
    private static char FromBit(byte b) => b == 0 ? '0' : '1';

    /// <summary>
    /// Converts an octal value to its character representation.
    /// </summary>
    /// <param name="b">The octal value.</param>
    /// <returns>The character representation of the octal value.</returns>
    private static char FromNum8(byte b) => Convert.ToString(b, 8).First();

    /// <summary>
    /// Converts an octal value to an array of bits.
    /// </summary>
    /// <param name="o">The octal value.</param>
    /// <returns>An array of bits representing the octal value.</returns>
    private static byte[] FromOct(byte o) => [(byte)(o >> 2), (byte)((o >> 1) & 1), (byte)(o & 1)];

    /// <summary>
    /// Converts a character to its bit value.
    /// </summary>
    /// <param name="c">The character ('0' or '1').</param>
    /// <returns>The bit value.</returns>
    private static byte ToBit(char c) => c == '0' ? (byte)0 : (byte)1;

    /// <summary>
    /// Converts a character to its octal value.
    /// </summary>
    /// <param name="c">The character.</param>
    /// <returns>The octal value.</returns>
    private static byte ToNum8(char c) => Convert.ToByte($"{c}", 8);

    /// <summary>
    /// Converts a sequence of bits to an octal value.
    /// </summary>
    /// <param name="b">The sequence of bits.</param>
    /// <returns>The octal value.</returns>
    private static byte ToOct(IEnumerable<byte> b)
    {
        var bin = b.ToArray();

        return (byte)((bin[0] << 2) | (bin[1] << 1) | bin[2]);
    }
}
