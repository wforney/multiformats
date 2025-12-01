using Multiformats.Address.Protocols;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Multiformats.Address.Net;

/// <summary>
/// The multiaddress extensions class
/// </summary>
public static class MultiaddressExtensions
{
    /// <summary>
    /// Gets the local multiaddress.
    /// </summary>
    /// <param name="socket">The socket.</param>
    public static Multiaddress GetLocalMultiaddress(this Socket socket) => socket.LocalEndPoint!.ToMultiaddress(socket.ProtocolType);

    /// <summary>
    /// Gets the remote multiaddress.
    /// </summary>
    /// <param name="socket">The socket.</param>
    public static Multiaddress GetRemoteMultiaddress(this Socket socket) => socket.RemoteEndPoint!.ToMultiaddress(socket.ProtocolType);

    /// <summary>
    /// Converts to multiaddress.
    /// </summary>
    /// <param name="ep">The ep.</param>
    /// <param name="protocolType">Type of the protocol.</param>
    public static Multiaddress ToMultiaddress(this EndPoint ep, ProtocolType protocolType)
    {
        ArgumentNullException.ThrowIfNull(ep);

        Multiaddress ma = new();

        var ip = (IPEndPoint)ep;
        if (ip is not null)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                _ = ma.Add<IP4>(ip.Address);
            }

            if (ip.AddressFamily == AddressFamily.InterNetworkV6)
            {
                _ = ma.Add<IP6>(ip.Address);
            }

            if (protocolType == ProtocolType.Tcp)
            {
                _ = ma.Add<TCP>((ushort)ip.Port);
            }

            if (protocolType == ProtocolType.Udp)
            {
                _ = ma.Add<UDP>((ushort)ip.Port);
            }
        }

        return ma;
    }

    /// <summary>
    /// Converts the specified <paramref name="ip"/> to a <see cref="Multiaddress"/>.
    /// </summary>
    /// <param name="ip">The IP address.</param>
    /// <returns>The <see cref="Multiaddress"/>.</returns>
    public static Multiaddress ToMultiaddress(this IPAddress ip)
    {
        Multiaddress ma = new();
        if (ip.AddressFamily == AddressFamily.InterNetwork)
        {
            _ = ma.Add<IP4>(ip);
        }

        if (ip.AddressFamily == AddressFamily.InterNetworkV6)
        {
            _ = ma.Add<IP6>(ip);
        }

        return ma;
    }

    /// <summary>
    /// Converts the specified <paramref name="ma"/> to a <see cref="IPEndPoint"/>.
    /// </summary>
    /// <param name="ma">The <see cref="Multiaddress"/>.</param>
    /// <returns>The <see cref="IPEndPoint"/>.</returns>
    public static IPEndPoint ToEndPoint(this Multiaddress ma) => ToEndPoint(ma, out _);

    /// <summary>
    /// Converts the specified <paramref name="ma"/> to a <see cref="IPEndPoint"/>.
    /// </summary>
    /// <param name="ma">The <see cref="Multiaddress"/>.</param>
    /// <param name="protocolType">The type of protocol.</param>
    /// <returns>The <see cref="IPEndPoint"/>.</returns>
    public static IPEndPoint ToEndPoint(this Multiaddress ma, out ProtocolType protocolType) => ToEndPoint(ma, out protocolType, out _);

    /// <summary>
    /// Converts the specified <paramref name="ma"/> to a <see cref="IPEndPoint"/>.
    /// </summary>
    /// <param name="ma">The <see cref="Multiaddress"/>.</param>
    /// <param name="protocolType">The type of protocol.</param>
    /// <param name="socketType">The type of socket.</param>
    /// <returns>The <see cref="IPEndPoint"/>.</returns>
    public static IPEndPoint ToEndPoint(this Multiaddress ma, out ProtocolType protocolType, out SocketType socketType)
    {
        IPAddress? addr = null;
        IP? ip = ma.Protocols.OfType<IP4>().SingleOrDefault();
        if (ip is not null)
        {
            addr = (IPAddress?)ip.Value;
        }
        else
        {
            ip = ma.Protocols.OfType<IP6>().SingleOrDefault();
            if (ip is not null)
            {
                addr = (IPAddress?)ip.Value;
            }
        }

        int? port = null;
        Number? n = ma.Protocols.OfType<TCP>().SingleOrDefault();
        if (n is not null)
        {
            port = (ushort?)n.Value;
            protocolType = ProtocolType.Tcp;
            socketType = SocketType.Stream;
        }
        else
        {
            n = ma.Protocols.OfType<UDP>().SingleOrDefault();
            if (n is not null)
            {
                port = (ushort?)n.Value;
                protocolType = ProtocolType.Udp;
                socketType = SocketType.Dgram;
            }
            else
            {
                protocolType = ProtocolType.Unknown;
                socketType = SocketType.Unknown;
            }
        }

        return new IPEndPoint(addr ?? IPAddress.Any, port ?? 0);
    }

    /// <summary>
    /// Creates a socket from the specified <paramref name="ma"/>.
    /// </summary>
    /// <param name="ma">The <see cref="Multiaddress"/>.</param>
    /// <returns>The socket.</returns>
    public static Socket CreateSocket(this Multiaddress ma) => CreateSocket(ma, out _);

    /// <summary>
    /// Creates a socket from the specified <paramref name="ma"/>.
    /// </summary>
    /// <param name="ma">The <see cref="Multiaddress"/>.</param>
    /// <param name="ep">The endpoint.</param>
    /// <returns>The socket.</returns>
    public static Socket CreateSocket(this Multiaddress ma, out IPEndPoint ep)
    {
        ep = ma.ToEndPoint(out ProtocolType pt, out SocketType st);

        return new Socket(ep.AddressFamily, st, pt);
    }

    /// <summary>
    /// Creates a connection from the specified <paramref name="ma"/>.
    /// </summary>
    /// <param name="ma">The <see cref="Multiaddress"/>.</param>
    /// <returns>The socket.</returns>
    public static Socket CreateConnection(this Multiaddress ma)
    {
        Socket socket = CreateSocket(ma, out IPEndPoint ep);
        socket.Connect(ep);
        return socket;
    }

    /// <summary>
    /// Creates a connection from the specified <paramref name="ma"/>.
    /// </summary>
    /// <param name="ma">The <see cref="Multiaddress"/>.</param>
    /// <returns>The socket.</returns>
    public static Task<Socket> CreateConnectionAsync(this Multiaddress ma)
    {
        Socket socket = CreateSocket(ma, out IPEndPoint ep);

#if NETSTANDARD1_6
        return socket.ConnectAsync(ep)
            .ContinueWith(_ => socket);
#else
        TaskCompletionSource<Socket> tcs = new();

        try
        {
            socket.BeginConnect(
                ep,
                ar =>
                {
                    try
                    {
                        socket.EndConnect(ar);
                        tcs.TrySetResult(socket);
                    }
                    catch (Exception e)
                    {
                        tcs.TrySetException(e);
                    }
                },
                null);
        }
        catch (Exception e)
        {
            tcs.TrySetException(e);
        }

        return tcs.Task;
#endif
    }

    /// <summary>
    /// Creates a listener from the specified <paramref name="ma"/>.
    /// </summary>
    /// <param name="ma">The <see cref="Multiaddress"/>.</param>
    /// <param name="backlog">The backlog.</param>
    /// <returns>The socket.</returns>
    public static Socket CreateListener(this Multiaddress ma, int backlog = 10)
    {
        Socket socket = CreateSocket(ma, out IPEndPoint ep);
        socket.Bind(ep);
        socket.Listen(backlog);
        return socket;
    }

    /// <summary>
    /// Returns a value indicating whether the specified <paramref name="ma"/> is thin waist.
    /// </summary>
    /// <param name="ma">The <see cref="Multiaddress"/>.</param>
    /// <returns>A value indicating whether the specified <paramref name="ma"/> is thin waist.</returns>
    public static bool IsThinWaist(this Multiaddress ma) =>
        ma.Protocols.Count != 0 &&
        !(ma.Protocols[0] is not IP4 and not IP6) &&
        (ma.Protocols.Count == 1 ||
        ma.Protocols[1] is TCP ||
        ma.Protocols[1] is UDP ||
        ma.Protocols[1] is IP4 ||
        ma.Protocols[1] is IP6);

    /// <summary>
    /// Gets the <see cref="IEnumerable{Multiaddress}"/> for the specified <see cref="NetworkInterface"/>.
    /// </summary>
    /// <param name="nic">The network interface.</param>
    /// <returns>The <see cref="IEnumerable{Multiaddress}"/>.</returns>
    public static IEnumerable<Multiaddress> GetMultiaddresses(this NetworkInterface nic) =>
        nic
            .GetIPProperties()
            .UnicastAddresses
            .Select(addr => addr.Address.ToMultiaddress());

    /// <summary>
    /// Returns the <see cref="IEnumerable{Multiaddress}"/> where the protocols match.
    /// </summary>
    /// <param name="match">The <see cref="Multiaddress"/>.</param>
    /// <param name="addrs">The addresses to match.</param>
    /// <returns>The <see cref="IEnumerable{Multiaddress}"/>.</returns>
    public static IEnumerable<Multiaddress> Match(this Multiaddress match, params Multiaddress[] addrs)
    {
        foreach (Multiaddress a in addrs.Where(x => match.Protocols.Count == x.Protocols.Count))
        {
            int i = 0;

            if (a.Protocols.All(p2 => match.Protocols[i++].Code == p2.Code))
            {
                yield return a;
            }
        }
    }
}
