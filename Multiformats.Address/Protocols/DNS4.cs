using System.Text;
namespace Multiformats.Address.Protocols;

/// <summary>
/// DNS4
/// </summary>
public record DNS4 : MultiaddressProtocol{
    /// <summary>
    /// Constructor for DNS4 class.
    /// </summary>
    /// <returns>An instance of the DNS4 class.</returns>
    public DNS4()
        : base("dns4", 54, -1)    {    }

    /// <summary>
    /// Constructor for DNS4 class that takes a string address as a parameter.
    /// </summary>
    /// <param name="address">The address to be used for the DNS4.</param>
    /// <returns>A new instance of the DNS4 class.</returns>
    public DNS4(string address)
            : this() => Value = address;

    /// <inheritdoc />
    public override void Decode(string value) => Value = value;

    /// <inheritdoc />
    public override void Decode(byte[] bytes) => Value = Encoding.UTF8.GetString(bytes);

    /// <inheritdoc />
    public override byte[] ToBytes() => Value is not null ? Encoding.UTF8.GetBytes((string)Value) : [];

    /// <inheritdoc />
    public override string ToString() => (string?)Value ?? string.Empty;}
