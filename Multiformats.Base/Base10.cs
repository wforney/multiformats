using System.Globalization;
using System.Numerics;

namespace Multiformats.Base;

/// <summary>
/// Provides base10 (decimal) encoding and decoding functionality for multibase.
/// </summary>
internal class Base10 : Multibase
{
    /// <summary>
    /// The base10 alphabet.
    /// </summary>
    private static readonly char[] _alphabet = "0123456789".ToCharArray();

    /// <inheritdoc/>
    protected override char[] Alphabet => _alphabet;

    /// <inheritdoc/>
    protected override string Name => "base10";

    /// <inheritdoc/>
    protected override char Prefix => '9';

    /// <inheritdoc/>
    public override byte[] Decode(string input)
    {
        var big = BigInteger.Parse($"00{input}", NumberStyles.None);
        return [.. LeadingZeros(input), .. big.ToByteArray().Reverse().SkipWhile(b => b == 0)];
    }

    /// <inheritdoc/>
    public override string Encode(byte[] bytes)
    {
        var big = new BigInteger(bytes.Reverse().Concat(new byte[] { 0x00 }).ToArray());
        return new string([.. LeadingNulls(bytes)]) + big.ToString();
    }

    /// <summary>
    /// Returns a sequence of '0' characters for each leading null byte in the input.
    /// </summary>
    /// <param name="input">The input byte sequence.</param>
    /// <returns>A sequence of '0' characters.</returns>
    private static IEnumerable<char> LeadingNulls(IEnumerable<byte> input) =>
        Enumerable.Range(0, input.TakeWhile(b => b == 0x00).Count()).Select(_ => '0');

    /// <summary>
    /// Returns a sequence of null bytes for each leading '0' character in the input.
    /// </summary>
    /// <param name="input">The input character sequence.</param>
    /// <returns>A sequence of null bytes.</returns>
    private static IEnumerable<byte> LeadingZeros(IEnumerable<char> input) =>
        Enumerable.Range(0, input.TakeWhile(b => b == '0').Count()).Select(_ => (byte)0x00);
}
