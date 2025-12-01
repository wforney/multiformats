using System.Text;
namespace Multiformats.Address.Protocols;

/// <summary>
/// DNS6
/// </summary>
public record DNS6 : MultiaddressProtocol{
    /// <summary>
    /// Constructor for DNS6 class.
    /// </summary>
    /// <returns>An instance of the DNS6 class.</returns>
    public DNS6()
        : base("dns6", 55, -1)    {    }

    /// <summary>
    /// Constructor for DNS6 class that sets the Value property to the given address.
    /// </summary>
    public DNS6(string address)
        : this() => Value = address;

    /// <inheritdoc />
    public override void Decode(string value) => Value = value;

    /// <inheritdoc />
    public override void Decode(byte[] bytes) => Value = Encoding.UTF8.GetString(bytes);

    /// <inheritdoc />
    public override byte[] ToBytes() => Value is not null ? Encoding.UTF8.GetBytes((string)Value) : [];

    /// <inheritdoc />
    public override string ToString() => (string?)Value ?? string.Empty;}
