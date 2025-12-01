using BinaryEncoding;
using System.Text;

namespace Multiformats.Stream;

/// <summary>
/// Provides multiplexing functionality for multistream protocol negotiation and handler management.
/// </summary>
public class MultistreamMuxer
{
    /// <summary>
    /// The protocol identifier for multistream.
    /// </summary>
    internal const string ProtocolId = "/multistream/1.0.0";

    private const byte Delimiter = (byte)'\n';
    private static readonly byte[] BytesMessageTooLarge = Encoding.UTF8.GetBytes("Messages over 64k are not allowed");
    private static readonly byte[] BytesNotAvailable = Encoding.UTF8.GetBytes("na");
    private static readonly byte[] DelimiterArray = "\n"u8.ToArray();
    private static readonly Exception ErrIncorrectVersion = new("Incorrect version");
    private static readonly Exception ErrMessageMissingNewline = new("Message did not have trailing newline");
    private static readonly Exception ErrMessageTooLarge = new("Messages over 64k are not allowed");
    private static readonly byte[] ProtocolIdBytes = Encoding.UTF8.GetBytes(ProtocolId);

    private readonly ReaderWriterLockSlim _handlerLock = new();
    private readonly List<IMultistreamHandler> _handlers = [];

    /// <summary>
    /// Gets the list of supported protocol strings.
    /// </summary>
    public string[] Protocols => _handlerLock.Read(() => _handlers.Select(h => h.Protocol).ToArray());

    /// <summary>
    /// Reads the next token from the stream using varint length and delimiter.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <returns>The token string, or empty if none.</returns>
    /// <exception cref="Exception">Thrown if message is too large or missing newline.</exception>
    public static string ReadNextToken(System.IO.Stream stream)
    {
        _ = Binary.Varint.Read(stream, out ulong length);
        if (length == 0)
        {
            return string.Empty;
        }

        if (length > 64 * 1024)
        {
            DelimWriteBuffered(stream, BytesMessageTooLarge);

            throw ErrMessageTooLarge;
        }

        var buffer = new byte[length];
        var total = 0;
        int res;
        while ((res = stream.Read(buffer, total, buffer.Length - total)) > 0)
        {
            total += res;
            if (total == (int)length)
            {
                break;
            }

            Task.Delay(1).Wait();
        }
        return res <= 0
            ? string.Empty
            : total != buffer.Length
            ? throw new Exception("could not read token")
            : (buffer.Length == 0) || (buffer[length - 1] != Delimiter)
            ? throw ErrMessageMissingNewline
            : Encoding.UTF8.GetString(buffer, 0, buffer.Length - 1);
    }

    /// <summary>
    /// Asynchronously reads the next token from the stream using varint length and delimiter.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The token string, or empty if none.</returns>
    /// <exception cref="Exception">Thrown if message is too large or missing newline.</exception>
    public static async Task<string> ReadNextTokenAsync(System.IO.Stream stream, CancellationToken cancellationToken)
    {
        var length = await Binary.Varint.ReadUInt64Async(stream, cancellationToken).ConfigureAwait(false);
        if (length == 0)
        {
            return string.Empty;
        }

        if (length > 64 * 1024)
        {
            await DelimWriteBufferedAsync(stream, BytesMessageTooLarge, cancellationToken).ConfigureAwait(false);

            throw ErrMessageTooLarge;
        }

        var buffer = new byte[length];
        var total = 0;
        int res;
        while ((res = await stream.ReadAsync(buffer.AsMemory(total, buffer.Length - total), cancellationToken).ConfigureAwait(false)) > 0)
        {
            total += res;
            if (total == (int)length)
            {
                break;
            }

            await Task.Delay(1, cancellationToken).ConfigureAwait(false);
        }

        return res <= 0
            ? string.Empty
            : total != buffer.Length
                ? throw new Exception("could not read token")
                : (buffer.Length == 0) || (buffer[length - 1] != Delimiter)
                    ? throw ErrMessageMissingNewline
                    : Encoding.UTF8.GetString(buffer, 0, buffer.Length - 1);
    }

    /// <summary>
    /// Selects one of the given protocols by negotiating with the stream.
    /// </summary>
    /// <param name="protocols">Array of protocol strings to select from.</param>
    /// <param name="stream">The stream to negotiate with.</param>
    /// <returns>The selected protocol string.</returns>
    /// <exception cref="NotSupportedException">Thrown if no protocol is supported.</exception>
    public static string SelectOneOf(string[] protocols, System.IO.Stream stream)
    {
        Handshake(stream);

        var protocol = protocols.SingleOrDefault(p => TrySelect(p, stream));
        return protocol ?? throw new NotSupportedException($"Protocols given are not supported: {string.Join(", ", protocols)}.");
    }

    /// <summary>
    /// Asynchronously selects one of the given protocols by negotiating with the stream.
    /// </summary>
    /// <param name="protocols">Array of protocol strings to select from.</param>
    /// <param name="stream">The stream to negotiate with.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The selected protocol string.</returns>
    /// <exception cref="NotSupportedException">Thrown if no protocol is supported.</exception>
    public static async Task<string> SelectOneOfAsync(string[] protocols, System.IO.Stream stream, CancellationToken cancellationToken)
    {
        await HandshakeAsync(stream, cancellationToken).ConfigureAwait(false);

        foreach (var protocol in protocols)
        {
            if (await TrySelectAsync(protocol, stream, cancellationToken).ConfigureAwait(false))
            {
                return protocol;
            }
        }

        throw new NotSupportedException($"Protocols given are not supported: {string.Join(", ", protocols)}.");
    }

    /// <summary>
    /// Selects the given protocol or fails if not supported.
    /// </summary>
    /// <param name="proto">The protocol string to select.</param>
    /// <param name="stream">The stream to negotiate with.</param>
    public static void SelectProtoOrFail(string proto, System.IO.Stream stream)
    {
        Handshake(stream);
        _ = TrySelect(proto, stream);
    }

    /// <summary>
    /// Asynchronously selects the given protocol or fails if not supported.
    /// </summary>
    /// <param name="proto">The protocol string to select.</param>
    /// <param name="stream">The stream to negotiate with.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static async Task SelectProtoOrFailAsync(string proto, System.IO.Stream stream, CancellationToken cancellationToken)
    {
        await HandshakeAsync(stream, cancellationToken).ConfigureAwait(false);
        _ = await TrySelectAsync(proto, stream, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Adds a handler for the specified protocol.
    /// </summary>
    /// <param name="protocol">The protocol string.</param>
    /// <param name="handle">The synchronous handler delegate.</param>
    /// <param name="asyncHandle">The asynchronous handler delegate.</param>
    /// <returns>The current <see cref="MultistreamMuxer"/> instance.</returns>
    public MultistreamMuxer AddHandler(string protocol, StreamHandlerFunc? handle = null, AsyncStreamHandlerFunc? asyncHandle = null)
    {
        RemoveHandler(protocol);
        return AddHandler(new FuncStreamHandler(protocol, handle, asyncHandle));
    }

    /// <summary>
    /// Adds a handler instance.
    /// </summary>
    /// <typeparam name="THandler">The handler type.</typeparam>
    /// <param name="handler">The handler instance.</param>
    /// <returns>The current <see cref="MultistreamMuxer"/> instance.</returns>
    public MultistreamMuxer AddHandler<THandler>(THandler handler) where THandler : IMultistreamHandler
    {
        RemoveHandler(handler);

        handler ??= Activator.CreateInstance<THandler>();

        _handlerLock.Write(() => _handlers.Add(handler));

        return this;
    }

    /// <summary>
    /// Handles a stream negotiation and invokes the appropriate handler.
    /// </summary>
    /// <param name="stream">The stream to handle.</param>
    /// <returns>True if a handler was invoked; otherwise, false.</returns>
    public bool Handle(System.IO.Stream stream)
    {
        var result = Negotiate(stream);
        return result?.Handler is not null && result.Handler.Handle(result.Protocol!, stream);
    }

    /// <summary>
    /// Asynchronously handles a stream negotiation and invokes the appropriate handler.
    /// </summary>
    /// <param name="stream">The stream to handle.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>True if a handler was invoked; otherwise, false.</returns>
    public async Task<bool> HandleAsync(System.IO.Stream stream, CancellationToken cancellationToken)
    {
        var result = await NegotiateAsync(stream, cancellationToken).ConfigureAwait(false);
        return result?.Handler is not null && await result.Handler.HandleAsync(result.Protocol!, stream, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Writes the list of supported protocols to the stream.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    public void Ls(System.IO.Stream stream)
    {
        using var ms = new MemoryStream();
        var handlers = _handlerLock.Read(_handlers.ToArray);
        _ = Binary.Varint.Write(ms, (ulong)handlers.Length);

        foreach (var handler in handlers)
        {
            DelimWrite(ms, Encoding.UTF8.GetBytes(handler.Protocol));
        }

        using var ms2 = new MemoryStream();
        _ = Binary.Varint.Write(ms2, (ulong)ms.Length);
        _ = ms.Seek(0, SeekOrigin.Begin);
        ms.CopyTo(ms2);
        _ = ms2.Seek(0, SeekOrigin.Begin);
        ms2.CopyTo(stream);
    }

    /// <summary>
    /// Asynchronously writes the list of supported protocols to the stream.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public async Task LsAsync(System.IO.Stream stream, CancellationToken cancellationToken)
    {
        using var ms = new MemoryStream();
        var handlers = _handlerLock.Read(_handlers.ToArray);

        _ = Binary.Varint.Write(ms, (ulong)handlers.Length);

        foreach (var handler in handlers)
        {
            await DelimWriteAsync(ms, Encoding.UTF8.GetBytes(handler.Protocol), cancellationToken).ConfigureAwait(false);
        }

        using var ms2 = new MemoryStream();
        _ = Binary.Varint.Write(ms2, (ulong)ms.Length);
        _ = ms.Seek(0, SeekOrigin.Begin);
        await ms.CopyToAsync(ms2, cancellationToken).ConfigureAwait(false);
        _ = ms2.Seek(0, SeekOrigin.Begin);
        await ms2.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Negotiates the protocol with the stream and returns the result.
    /// </summary>
    /// <param name="stream">The stream to negotiate with.</param>
    /// <returns>The negotiation result, or null if negotiation failed.</returns>
    /// <exception cref="Exception">Thrown if protocol version is incorrect.</exception>
    public NegotiationResult? Negotiate(System.IO.Stream stream)
    {
        DelimWriteBuffered(stream, ProtocolIdBytes);

        var token = ReadNextToken(stream);
        if (token is null)
        {
            return null;
        }

        if (token != ProtocolId)
        {
            stream.Dispose();
            throw ErrIncorrectVersion;
        }

        while (true)
        {
            token = ReadNextToken(stream);
            if (token is null)
            {
                break;
            }

            switch (token)
            {
                case "ls":
                    Ls(stream);
                    break;

                default:
                    var handler = FindHandler(token);
                    if (handler is null)
                    {
                        DelimWriteBuffered(stream, BytesNotAvailable);
                        continue;
                    }

                    DelimWriteBuffered(stream, Encoding.UTF8.GetBytes(token));

                    return new NegotiationResult(token, handler);
            }
        }

        return null;
    }

    /// <summary>
    /// Asynchronously negotiates the protocol with the stream and returns the result.
    /// </summary>
    /// <param name="stream">The stream to negotiate with.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The negotiation result, or null if negotiation failed.</returns>
    /// <exception cref="Exception">Thrown if protocol version is incorrect.</exception>
    public async Task<NegotiationResult?> NegotiateAsync(System.IO.Stream stream, CancellationToken cancellationToken)
    {
        await DelimWriteBufferedAsync(stream, ProtocolIdBytes, cancellationToken).ConfigureAwait(false);

        var token = await ReadNextTokenAsync(stream, cancellationToken).ConfigureAwait(false);
        if (token is null)
        {
            return null;
        }

        if (token != ProtocolId)
        {
            stream.Dispose();
            throw ErrIncorrectVersion;
        }

        while (true)
        {
            token = await ReadNextTokenAsync(stream, cancellationToken).ConfigureAwait(false);
            if (token is null)
            {
                break;
            }

            switch (token)
            {
                case "ls":
                    await LsAsync(stream, cancellationToken).ConfigureAwait(false);
                    break;

                default:
                    var handler = FindHandler(token);
                    if (handler is null)
                    {
                        await DelimWriteBufferedAsync(stream, BytesNotAvailable, cancellationToken).ConfigureAwait(false);
                        continue;
                    }

                    await DelimWriteBufferedAsync(stream, Encoding.UTF8.GetBytes(token), cancellationToken).ConfigureAwait(false);

                    return new NegotiationResult(token, handler);
            }
        }
        return null;
    }

    /// <summary>
    /// Removes the specified handler instance.
    /// </summary>
    /// <typeparam name="THandler">The handler type.</typeparam>
    /// <param name="handler">The handler instance.</param>
    public void RemoveHandler<THandler>(THandler? handler) where THandler : IMultistreamHandler
    {
        _handlerLock.Write(
            () =>
            {
                handler ??= _handlers.OfType<THandler>().SingleOrDefault();

                if ((handler is not null) && _handlers.Contains(handler))
                {
                    _ = _handlers.Remove(handler);
                }
            });
    }

    /// <summary>
    /// Removes the handler for the specified protocol.
    /// </summary>
    /// <param name="protocol">The protocol string.</param>
    public void RemoveHandler(string protocol)
    {
        var handler = FindHandler(protocol);
        if (handler is not null)
        {
            _ = _handlerLock.Write(() => _handlers.Remove(handler));
        }
    }

    /// <summary>
    /// Writes a message to the stream with a delimiter.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="message">The message bytes.</param>
    internal static void DelimWrite(System.IO.Stream stream, byte[] message)
    {
        _ = Binary.Varint.Write(stream, (ulong)(message.Length + 1));

        stream.Write(message, 0, message.Length);
        stream.WriteByte(Delimiter);
    }

    /// <summary>
    /// Asynchronously writes a message to the stream with a delimiter.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="message">The message bytes.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    internal static async Task DelimWriteAsync(System.IO.Stream stream, byte[] message, CancellationToken cancellationToken)
    {
        _ = Binary.Varint.Write(stream, (ulong)(message.Length + 1));

        await stream.WriteAsync(message, cancellationToken).ConfigureAwait(false);
        await stream.WriteAsync(DelimiterArray.AsMemory(0, 1), cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Writes a buffered message to the stream with a delimiter.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="message">The message bytes.</param>
    private static void DelimWriteBuffered(System.IO.Stream stream, byte[] message)
    {
        using (var buffer = new MemoryStream())
        {
            DelimWrite(buffer, message);

            var bytes = buffer.ToArray();
            stream.Write(bytes, 0, bytes.Length);
        }
        stream.Flush();
    }

    /// <summary>
    /// Asynchronously writes a buffered message to the stream with a delimiter.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="message">The message bytes.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    private static async Task DelimWriteBufferedAsync(System.IO.Stream stream, byte[] message, CancellationToken cancellationToken)
    {
        using (var buffer = new MemoryStream())
        {
            await DelimWriteAsync(buffer, message, cancellationToken).ConfigureAwait(false);
            _ = buffer.Seek(0, SeekOrigin.Begin);
            await buffer.CopyToAsync(stream, 4096, cancellationToken).ConfigureAwait(false);
        }
        await stream.FlushAsync(cancellationToken);
    }

    /// <summary>
    /// Performs a handshake with the stream for protocol negotiation.
    /// </summary>
    /// <param name="stream">The stream to handshake with.</param>
    /// <exception cref="Exception">Thrown if protocol id does not match.</exception>
    private static void Handshake(System.IO.Stream stream)
    {
        var token = ReadNextToken(stream);

        if (token != ProtocolId)
        {
            throw new Exception("Received mismatch in protocol id");
        }

        DelimWrite(stream, ProtocolIdBytes);
    }

    /// <summary>
    /// Asynchronously performs a handshake with the stream for protocol negotiation.
    /// </summary>
    /// <param name="stream">The stream to handshake with.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <exception cref="Exception">Thrown if protocol id does not match.</exception>
    private static async Task HandshakeAsync(System.IO.Stream stream, CancellationToken cancellationToken)
    {
        var token = await ReadNextTokenAsync(stream, cancellationToken).ConfigureAwait(false);

        if (token != ProtocolId)
        {
            throw new Exception("Received mismatch in protocol id");
        }

        await DelimWriteAsync(stream, ProtocolIdBytes, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Attempts to select the given protocol by writing to the stream and reading the response.
    /// </summary>
    /// <param name="proto">The protocol string to select.</param>
    /// <param name="stream">The stream to negotiate with.</param>
    /// <returns>True if protocol was selected; otherwise, false.</returns>
    private static bool TrySelect(string proto, System.IO.Stream stream)
    {
        DelimWrite(stream, Encoding.UTF8.GetBytes(proto));

        var token = ReadNextToken(stream);
        return !string.IsNullOrEmpty(token) && (token == proto || (token == "na" ? false : throw new Exception($"Unrecognized response: {token}")));
    }

    /// <summary>
    /// Asynchronously attempts to select the given protocol by writing to the stream and reading
    /// the response.
    /// </summary>
    /// <param name="proto">The protocol string to select.</param>
    /// <param name="stream">The stream to negotiate with.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>True if protocol was selected; otherwise, false.</returns>
    private static async Task<bool> TrySelectAsync(string proto, System.IO.Stream stream, CancellationToken cancellationToken)
    {
        await DelimWriteAsync(stream, Encoding.UTF8.GetBytes(proto), cancellationToken).ConfigureAwait(false);

        var token = await ReadNextTokenAsync(stream, cancellationToken).ConfigureAwait(false);
        return !string.IsNullOrEmpty(token) && (token == proto || (token == "na" ? false : throw new Exception($"Unrecognized response: {token}")));
    }

    /// <summary>
    /// Finds the handler for the specified protocol.
    /// </summary>
    /// <param name="protocol">The protocol string.</param>
    /// <returns>The handler instance, or null if not found.</returns>
    private IMultistreamHandler? FindHandler(string protocol) => _handlerLock.Read(() => _handlers.SingleOrDefault(h => h.Protocol.Equals(protocol)));
}