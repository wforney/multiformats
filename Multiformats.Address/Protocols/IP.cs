using System.Net;
namespace Multiformats.Address.Protocols;
/// <summary>
/// IP
/// </summary>
public abstract record IP : MultiaddressProtocol{
    /// <summary>
    /// Gets the IP address associated with the current instance of the IPAddressValue class.
    /// </summary>
    /// <returns>
    /// An IPAddress object that contains the IP address associated with the current instance of the
    /// IPAddressValue class.
    /// </returns>
    public IPAddress Address => Value != null ? (IPAddress)Value : IPAddress.None;

    /// <summary>
    /// Constructor for the IP class which inherits from the base class.
    /// </summary>
    /// <param name="name">Name of the IP.</param>
    /// <param name="code">Code of the IP.</param>
    /// <param name="size">Size of the IP.</param>
    /// <returns>No return value.</returns>
    protected IP(string name, int code, int size)
            : base(name, code, size)    {    }

    /// <inheritdoc />
    public override void Decode(string value) => Value = IPAddress.Parse(value);

    /// <inheritdoc />
    public override void Decode(byte[] bytes) => Value = new IPAddress(bytes);

    /// <inheritdoc />
    public override byte[] ToBytes() => Address.GetAddressBytes();

    /// <inheritdoc />
    public override string ToString() => Address.ToString();}
