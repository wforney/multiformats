using BinaryEncoding;
using System.Globalization;

namespace Multiformats.Address.Protocols;

/// <summary>
/// The number record
/// </summary>
public abstract record Number : MultiaddressProtocol
{
    /// <summary>
    /// Constructor for the Number class.
    /// </summary>
    /// <param name="name">The name of the number.</param>
    /// <param name="code">The code of the number.</param>
    /// <returns>A new instance of the Number class.</returns>
    protected Number(string name, int code)
        : base(name, code, 16)
    {
    }

    /// <summary>
    /// Gets the port value of the Value property, or 0 if Value is null.
    /// </summary>
    public ushort Port => (ushort?)Value ?? 0;

    /// <inheritdoc />
    public override void Decode(string value) => Value = ushort.Parse(value, NumberStyles.Number);

    /// <inheritdoc />
    public override void Decode(byte[] bytes) => Value = Binary.BigEndian.GetUInt16(bytes, 0);

    /// <inheritdoc />
    public override byte[] ToBytes() => Value is not null ? Binary.BigEndian.GetBytes((ushort)Value) : [];
}
