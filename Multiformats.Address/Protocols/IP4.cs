using System.Net;using System.Net.Sockets;
namespace Multiformats.Address.Protocols;

/// <summary>
/// Represents an IP4 address. Inherits from the IP class.
/// </summary>
public record IP4 : IP{
    /// <summary>
    /// Constructor for IP4 class.
    /// </summary>
    /// <returns>An instance of IP4 class.</returns>
    public IP4()
        : base("ip4", 4, 32)    {    }

    /// <summary>
    /// Constructs an IP4 object from an IPAddress object.
    /// </summary>
    /// <param name="address">The IPAddress object to construct from.</param>
    /// <returns>An IP4 object.</returns>
    public IP4(IPAddress address)
        : this()    {        if (address.AddressFamily != AddressFamily.InterNetwork)        {            throw new Exception("Address is not IPv4");        }        Value = address;    }

    /// <inheritdoc />
    public override void Decode(string value)    {        base.Decode(value);        if (Value is not null && ((IPAddress)Value).AddressFamily != AddressFamily.InterNetwork)        {            throw new Exception("Address is not IPv4");        }    }

    /// <inheritdoc />
    public override void Decode(byte[] bytes)    {        base.Decode(bytes);        if (Value is not null && ((IPAddress)Value).AddressFamily != AddressFamily.InterNetwork)        {            throw new Exception("Address is not IPv4");        }    }}
