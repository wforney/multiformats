namespace Multiformats.Base;

/// <summary>
/// Provides base functionality for Base64 encoding and decoding with support for URL-safe and
/// padded variants.
/// </summary>
internal abstract class Base64 : Multibase
{
    /// <summary>
    /// Decodes a Base64-encoded string to a byte array.
    /// </summary>
    /// <param name="input">The Base64-encoded string.</param>
    /// <param name="urlSafe">Indicates if the input uses URL-safe Base64 encoding.</param>
    /// <param name="padded">Indicates if the input is padded.</param>
    /// <returns>The decoded byte array.</returns>
    protected virtual byte[] Decode(string input, bool urlSafe, bool padded)
    {
        if (urlSafe)
        {
            input = input.Replace('-', '+').Replace('_', '/');
        }

        input = Pad(input.TrimEnd('='));

        return Convert.FromBase64String(input);
    }

    /// <summary>
    /// Encodes a byte array to a Base64 string.
    /// </summary>
    /// <param name="input">The byte array to encode.</param>
    /// <param name="urlSafe">Indicates if the output should use URL-safe Base64 encoding.</param>
    /// <param name="padded">Indicates if the output should be padded.</param>
    /// <returns>The Base64-encoded string.</returns>
    protected virtual string Encode(byte[] input, bool urlSafe, bool padded)
    {
        var result = Convert.ToBase64String(input);
        if (urlSafe)
        {
            result = result.Replace('+', '-').Replace('/', '_');
        }

        if (!padded)
        {
            result = result.TrimEnd('=');
        }

        return result;
    }

    /// <summary>
    /// Pads a Base64 string to a length that is a multiple of 4.
    /// </summary>
    /// <param name="input">The Base64 string to pad.</param>
    /// <returns>The padded Base64 string.</returns>
    private static string Pad(string input)
    {
        var diff = input.Length % 4;

        return diff > 0 ? input + new string('=', 4 - diff) : input;
    }
}
