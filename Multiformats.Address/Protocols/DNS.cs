using System.Text;

namespace Multiformats.Address.Protocols;

/// <summary>
/// DNS
/// </summary>
public record DNS : MultiaddressProtocol{    /// <summary>
     /// Constructor for DNS class. </summary> <returns> An instance of the DNS class. </returns>
    public DNS()
        : base("dns", 53, -1)    {    }

    /// <summary>
    /// Constructor for DNS class that takes a string address as a parameter.
    /// </summary>
    /// <param name="address">The address to be used for the DNS.</param>
    /// <returns>A new instance of the DNS class.</returns>
    public DNS(string address)
        : this() => Value = address;

    /// <inheritdoc />
    public override void Decode(string value) => Value = value;

    /// <inheritdoc />
    public override void Decode(byte[] bytes) => Value = Encoding.UTF8.GetString(bytes);

    /// <inheritdoc />
    public override byte[] ToBytes() => Encoding.UTF8.GetBytes((string?)Value ?? string.Empty);

    /// <inheritdoc />
    public override string ToString() => (string?)Value ?? string.Empty;}
