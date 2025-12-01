using BinaryEncoding;

namespace Multiformats.Hash;

/// <summary>
/// Provides extension methods for working with <see cref="Stream"/> and <see cref="Multihash"/>.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Reads a <see cref="Multihash"/> from the specified <see cref="Stream"/>.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <returns>
    /// The <see cref="Multihash"/> read from the stream, or <c>null</c> if reading fails.
    /// </returns>
    public static Multihash? ReadMultihash(this Stream stream)
    {
        if (Binary.Varint.Read(stream, out uint code) <= 0)
        {
            return null;
        }

        if (Binary.Varint.Read(stream, out uint length) <= 0)
        {
            return null;
        }

        var buffer = new byte[length];
        return stream.Read(buffer, 0, buffer.Length) != length
            ? null
            : Multihash.Cast(Binary.Varint.GetBytes(code).Concat(Binary.Varint.GetBytes(length), buffer));
    }

    /// <summary>
    /// Asynchronously reads a <see cref="Multihash"/> from the specified <see cref="Stream"/>.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="cancellationToken">A cancellation token to observe.</param>
    /// <returns>
    /// A task that represents the asynchronous read operation. The value of the task is the <see
    /// cref="Multihash"/> read from the stream, or <c>null</c> if reading fails.
    /// </returns>
    public static async Task<Multihash?> ReadMultihashAsync(this Stream stream, CancellationToken cancellationToken)
    {
        var code = await Binary.Varint.ReadUInt32Async(stream, cancellationToken);

        // TODO: We should check how many bytes we have read, but the method we are using does not support that.
        var length = await Binary.Varint.ReadUInt32Async(stream, cancellationToken);
        if (length == 0)
        {
            return null;
        }

        var buffer = new byte[length];
        return await stream.ReadAsync(buffer, cancellationToken) != length
            ? null
            : Multihash.Cast(Binary.Varint.GetBytes(code).Concat(Binary.Varint.GetBytes(length), buffer));
    }

    /// <summary>
    /// Writes the specified <see cref="Multihash"/> to the <see cref="Stream"/>.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="mh">The <see cref="Multihash"/> to write.</param>
    public static void Write(this Stream stream, Multihash mh)
    {
        var bytes = mh.ToBytes();
        stream.Write(bytes, 0, bytes.Length);
    }

    /// <summary>
    /// Asynchronously writes the specified <see cref="Multihash"/> to the <see cref="Stream"/>.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="mh">The <see cref="Multihash"/> to write.</param>
    /// <param name="cancellationToken">A cancellation token to observe.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    public static Task WriteAsync(this Stream stream, Multihash mh, CancellationToken cancellationToken)
    {
        var bytes = mh.ToBytes();
        return stream.WriteAsync(bytes, 0, bytes.Length, cancellationToken);
    }

    /// <summary>
    /// Concatenates the current buffer with the specified buffers.
    /// </summary>
    /// <param name="buffer">The base buffer.</param>
    /// <param name="buffers">The buffers to concatenate.</param>
    /// <returns>A new byte array containing the concatenated buffers.</returns>
    internal static byte[] Concat(this byte[] buffer, params byte[][] buffers)
    {
        var result = new byte[buffer.Length + buffers.Sum(b => b.Length)];
        Buffer.BlockCopy(buffer, 0, result, 0, buffer.Length);
        var offset = buffer.Length;
        foreach (var buf in buffers)
        {
            Buffer.BlockCopy(buf, 0, result, offset, buf.Length);
            offset += buf.Length;
        }

        return result;
    }

    /// <summary>
    /// Returns a slice of the buffer starting at the specified offset and with the specified count.
    /// </summary>
    /// <param name="buffer">The buffer to slice.</param>
    /// <param name="offset">The zero-based byte offset at which to begin the slice.</param>
    /// <param name="count">
    /// The number of bytes to include in the slice. If <c>null</c>, the slice continues to the end
    /// of the buffer.
    /// </param>
    /// <returns>A new byte array containing the specified slice.</returns>
    internal static byte[] Slice(this byte[] buffer, int offset = 0, int? count = null)
    {
        var result = new byte[Math.Min(count ?? (buffer.Length - offset), buffer.Length - offset)];
        Buffer.BlockCopy(buffer, offset, result, 0, result.Length);
        return result;
    }
}
