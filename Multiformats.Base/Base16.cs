using System.Text;

namespace Multiformats.Base;

/// <summary>
/// Provides base-16 (hexadecimal) encoding and decoding functionality.
/// </summary>
internal abstract class Base16 : Multibase
{
    /// <summary>
    /// Decodes a hexadecimal string to a byte array, applying the specified letter casing.
    /// </summary>
    /// <param name="input">The hexadecimal string to decode.</param>
    /// <param name="casing">The expected letter casing of the input.</param>
    /// <returns>The decoded byte array.</returns>
    protected virtual byte[] Decode(string input, LetterCasing casing)
    {
        if (casing == LetterCasing.Lower && input.Any(char.IsUpper))
        {
            input = input.ToLower();
        }

        if (casing == LetterCasing.Upper && input.Any(char.IsLower))
        {
            input = input.ToUpper();
        }

        return [.. Enumerable
            .Range(0, input.Length / 2)
            .Select(i => (byte)Convert.ToInt32(input.Substring(i * 2, 2), 16))];
    }

    /// <summary>
    /// Encodes a byte array to a hexadecimal string using the specified letter casing.
    /// </summary>
    /// <param name="input">The byte array to encode.</param>
    /// <param name="casing">The letter casing to use for the output string.</param>
    /// <returns>The encoded hexadecimal string.</returns>
    protected virtual string Encode(byte[] input, LetterCasing casing)
    {
        var format = casing == LetterCasing.Lower ? "{0:x2}" : "{0:X2}";

        return input.Aggregate(new StringBuilder(), (sb, b) => sb.AppendFormat(format, b)).ToString();
    }
}
