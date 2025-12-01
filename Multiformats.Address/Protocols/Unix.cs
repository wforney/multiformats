using BinaryEncoding;
using System.Text;

namespace Multiformats.Address.Protocols;

/// <summary>
/// Unix
/// </summary>
public record Unix : MultiaddressProtocol
{
    /// <summary>
    /// Gets the path.
    /// </summary>
    public string Path => Value is null ? string.Empty : (string)Value;

    /// <summary>
    /// Initializes a new instance of the <see cref="Unix"/> class.
    /// </summary>
    public Unix()
        : base("unix", 400, -1)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Unix"/> class.
    /// </summary>
    /// <param name="address">The address.</param>
    public Unix(string address)
        : this() => Value = address;

    /// <inheritdoc/>
    public override void Decode(string value) => Value = value;

    /// <inheritdoc/>
    public override void Decode(byte[] bytes)
    {
        var n = Binary.Varint.Read(bytes, 0, out uint size);

        if (bytes.Length - n != size)
        {
            throw new Exception("Inconsitent lengths");
        }

        if (size == 0)
        {
            throw new Exception("Invalid length");
        }

        var s = Encoding.UTF8.GetString(bytes, n, bytes.Length - n);

        Value = s[1..];
    }

    /// <inheritdoc/>
    public override byte[] ToBytes()
    {
        return
        [
            .. Binary.Varint.GetBytes((uint)Encoding.UTF8.GetByteCount((string?)Value ?? string.Empty)),
            .. Encoding.UTF8.GetBytes((string?)Value ?? string.Empty),
        ];
    }
}
