using BinaryEncoding;using Multiformats.Base;
namespace Multiformats.Address.Protocols;

/// <summary>
/// Onion
/// </summary>
public record Onion : MultiaddressProtocol{
    /// <summary>
    /// Gets the address of the object, or an empty string if the value is null.
    /// </summary>
    public string Address => Value is null ? string.Empty : (string)Value;

    /// <summary>
    /// Constructor for Onion class which inherits from the base class.
    /// </summary>
    /// <returns>An instance of the Onion class.</returns>
    public Onion()
        : base("onion", 444, 96)    {    }

    /// <summary>
    /// Constructor for Onion class that takes a string as an argument.
    /// </summary>
    /// <param name="s">String to be used for the Onion object.</param>
    /// <returns>An Onion object with the given string as its value.</returns>
    public Onion(string s)
        : this() => Value = s;

    /// <inheritdoc />
    public override void Decode(string value)    {        var addr = value.Split(':');        if (addr.Length != 2)        {            throw new Exception("Failed to parse addr");        }        if (addr[0].Length != 16)        {            throw new Exception("Failed to parse addr");        }        if (!Multibase.TryDecode(addr[0], out var encoding, out _) || encoding != MultibaseEncoding.Base32Lower)        {            throw new InvalidOperationException($"{value} is not a valid onion address.");        }        var i = ushort.Parse(addr[1]);        if (i < 1)        {            throw new Exception("Failed to parse addr");        }        Value = value;    }

    /// <inheritdoc />
    public override void Decode(byte[] bytes)    {        var addr = Multibase.Base32.Encode(bytes.AsSpan()[..10].ToArray());        var port = Binary.BigEndian.GetUInt16(bytes, 10);        Value = $"{addr}:{port}";    }

    /// <inheritdoc />
    public override byte[] ToBytes()    {        var s = (string?)Value;        var addr = s?.Split(':') ?? [];        if (addr.Length != 2)        {            throw new Exception("Failed to parse addr");        }        if (addr[0].Length != 16)        {            throw new Exception("Failed to parse addr");        }        if (!Multibase.TryDecode(addr[0], out var encoding, out var onionHostBytes) || encoding != MultibaseEncoding.Base32Lower)        {            throw new InvalidOperationException($"{s} is not a valid onion address.");        }        var i = ushort.Parse(addr[1]);        return i < 1 ? throw new Exception("Failed to parse addr") : [.. onionHostBytes, .. Binary.BigEndian.GetBytes(i)];    }}
