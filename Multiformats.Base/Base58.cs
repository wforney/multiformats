using System.Collections.Concurrent;
using System.Numerics;

namespace Multiformats.Base;

/// <summary>
/// Provides Base58 encoding and decoding functionality using a specified alphabet.
/// </summary>
internal abstract class Base58 : Multibase
{
    /// <summary>
    /// A thread-safe dictionary mapping alphabets to their decode maps.
    /// </summary>
    private static readonly ConcurrentDictionary<char[], byte[]> DecodeMap = new();

    /// <summary>
    /// Decodes a Base58-encoded string to its byte array representation using the specified alphabet.
    /// </summary>
    /// <param name="b">The Base58-encoded string.</param>
    /// <param name="alphabet">The character array representing the Base58 alphabet.</param>
    /// <returns>The decoded byte array.</returns>
    protected virtual byte[] Decode(string b, char[] alphabet)
    {
        lock (DecodeMap)
        {
            var decodeMap = DecodeMap.GetOrAdd(alphabet, CreateDecodeMap);
            var len = alphabet.Length;

            return
            [
                .. b.TakeWhile(c => c == alphabet[0])
                    .Select(_ => (byte)0),
                .. b.Select(c => decodeMap[c])
                    .Aggregate<byte, BigInteger>(0, (current, c) => (current * len) + c)
                    .ToByteArray()
                    .Reverse()
                    .SkipWhile(c => c == 0),
            ];
        }
    }

    /// <summary>
    /// Encodes a byte array to a Base58 string using the specified alphabet.
    /// </summary>
    /// <param name="b">The byte array to encode.</param>
    /// <param name="alphabet">The character array representing the Base58 alphabet.</param>
    /// <returns>The Base58-encoded string.</returns>
    protected virtual string Encode(byte[] b, char[] alphabet)
    {
        return new string(
            [
                .. b.TakeWhile(c => c == 0)
                    .Select(_ => alphabet[0]),
                .. ParseBigInt(
                    b
                        .Aggregate<byte, BigInteger>(
                            0,
                            (current, t) => (current * 256) + t),
                            alphabet)
                        .Reverse(),
            ]);
    }

    /// <summary>
    /// Creates a decode map for the specified alphabet.
    /// </summary>
    /// <param name="alphabet">The character array representing the Base58 alphabet.</param>
    /// <returns>A byte array mapping each character in the alphabet to its index.</returns>
    private static byte[] CreateDecodeMap(char[] alphabet)
    {
        var map = Enumerable.Range(0, 256).Select(b => (byte)0xFF).ToArray();
        for (var i = 0; i < alphabet.Length; i++)
        {
            map[alphabet[i]] = (byte)i;
        }

        return map;
    }

    /// <summary>
    /// Parses a <see cref="BigInteger"/> value into a sequence of Base58 characters using the
    /// specified alphabet.
    /// </summary>
    /// <param name="intData">The <see cref="BigInteger"/> value to parse.</param>
    /// <param name="alphabet">The character array representing the Base58 alphabet.</param>
    /// <returns>An enumerable sequence of Base58 characters.</returns>
    private static IEnumerable<char> ParseBigInt(BigInteger intData, char[] alphabet)
    {
        var len = alphabet.Length;
        while (intData > 0)
        {
            var rem = (int)(intData % len);
            intData /= len;
            yield return alphabet[rem];
        }
    }
}
