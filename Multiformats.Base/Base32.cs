namespace Multiformats.Base;

/// <summary>
/// Provides base functionality for Base32 encoding and decoding.
/// </summary>
internal abstract class Base32 : Multibase
{
    /// <summary>
    /// Decodes a Base32 encoded string to a byte array.
    /// </summary>
    /// <param name="input">The Base32 encoded string.</param>
    /// <param name="padding">Indicates whether padding should be removed.</param>
    /// <param name="casing">Specifies the letter casing to use for decoding.</param>
    /// <returns>The decoded byte array.</returns>
    protected virtual byte[] Decode(string input, bool padding, LetterCasing casing)
    {
        if (padding)
        {
            input = input.TrimEnd('=');
        }

        if (casing == LetterCasing.Lower && input.Any(char.IsUpper))
        {
            input = input.ToLower();
        }

        if (casing == LetterCasing.Upper && input.Any(char.IsLower))
        {
            input = input.ToUpper();
        }

        var bits = 0;
        var value = 0;
        var index = 0;
        var output = new byte[(input.Length * 5 / 8) | 0];

        for (var i = 0; i < input.Length; i++)
        {
            value = (value << 5) | Array.IndexOf(Alphabet, input[i]);
            bits += 5;

            if (bits >= 8)
            {
                output[index++] = (byte)(((uint)value >> (bits - 8)) & 255);
                bits -= 8;
            }
        }

        return output;
    }

    /// <summary>
    /// Encodes a byte array to a Base32 string.
    /// </summary>
    /// <param name="bytes">The byte array to encode.</param>
    /// <param name="padding">Indicates whether padding should be added.</param>
    /// <returns>The Base32 encoded string.</returns>
    protected virtual string Encode(byte[] bytes, bool padding)
    {
        var bits = 0;
        var value = 0;
        var output = "";

        for (var i = 0; i < bytes.Length; i++)
        {
            value = (value << 8) | bytes[i];
            bits += 8;

            while (bits >= 5)
            {
                output += Alphabet[(int)((uint)value >> (bits - 5)) & 31];
                bits -= 5;
            }
        }

        if (bits > 0)
        {
            output += Alphabet[(value << (5 - bits)) & 31];
        }

        if (padding)
        {
            while ((output.Length % 8) != 0)
            {
                output += '=';
            }
        }

        return output;
    }
}
