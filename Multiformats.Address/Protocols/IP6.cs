using System.Net;using System.Net.Sockets;
namespace Multiformats.Address.Protocols;

/// <summary>
/// Represents an IPv6 address.
/// </summary>
public record IP6 : IP{
    /// <summary>
    /// Constructor for IP6 class.
    /// </summary>
    /// <returns>An instance of the IP6 class.</returns>
    public IP6()
        : base("ip6", 41, 128)    {    }

    /// <summary>
    /// Constructor for IP6 class that takes an IPAddress as a parameter.
    /// </summary>
    /// <param name="address">IPAddress to be used for the IP6 object.</param>
    /// <returns>IP6 object with the given IPAddress.</returns>
    public IP6(IPAddress address)
        : this()    {        if (address.AddressFamily != AddressFamily.InterNetworkV6)        {            throw new Exception("Address is not IPv6");        }        Value = address;    }

    /// <inheritdoc />
    public override void Decode(string value)    {        base.Decode(value);        if (Value != null && ((IPAddress)Value).AddressFamily != AddressFamily.InterNetworkV6)        {            throw new Exception("Address is not IPv6");        }    }

    /// <inheritdoc />
    public override void Decode(byte[] bytes)    {        base.Decode(bytes);        if (Value != null && ((IPAddress)Value).AddressFamily != AddressFamily.InterNetworkV6)        {            throw new Exception("Address is not IPv6");        }    }}
