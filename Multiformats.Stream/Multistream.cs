namespace Multiformats.Stream;

/// <summary>
/// Represents a stream that performs a multistream handshake before reading or writing data.
/// </summary>
public class Multistream : System.IO.Stream
{
    private readonly MultistreamHandshaker _handshaker;
    private readonly System.IO.Stream _stream;

    /// <summary>
    /// Initializes a new instance of the <see cref="Multistream"/> class with the specified stream
    /// and protocols.
    /// </summary>
    /// <param name="stream">The underlying stream to wrap.</param>
    /// <param name="protocols">The protocols to use for the handshake.</param>
    protected Multistream(System.IO.Stream stream, params string[] protocols)
    {
        _stream = stream;
        _handshaker = new MultistreamHandshaker(this, /*Debugger.IsAttached ? TimeSpan.Zero :*/ TimeSpan.FromSeconds(3), protocols);
    }

    /// <inheritdoc/>
    public override bool CanRead => true;

    /// <inheritdoc/>
    public override bool CanSeek => false;

    /// <inheritdoc/>
    public override bool CanWrite => true;

    /// <inheritdoc/>
    public override long Length => 0;

    /// <inheritdoc/>
    public override long Position { get; set; }

    /// <inheritdoc/>
    public override int ReadTimeout
    {
        get => _stream.ReadTimeout; set => _stream.ReadTimeout = value;
    }

    /// <inheritdoc/>
    public override int WriteTimeout
    {
        get => _stream.WriteTimeout; set => _stream.WriteTimeout = value;
    }

    /// <summary>
    /// Creates a new <see cref="Multistream"/> instance with the specified protocol.
    /// </summary>
    /// <param name="stream">The underlying stream to wrap.</param>
    /// <param name="protocol">The protocol to use for the handshake.</param>
    /// <returns>A new <see cref="Multistream"/> instance.</returns>
    public static Multistream Create(System.IO.Stream stream, string protocol) => new(stream, protocol);

    /// <summary>
    /// Creates a new <see cref="Multistream"/> instance with the protocol selection handshake.
    /// </summary>
    /// <param name="stream">The underlying stream to wrap.</param>
    /// <param name="protocol">The protocol to use for the handshake.</param>
    /// <returns>A new <see cref="Multistream"/> instance.</returns>
    public static Multistream CreateSelect(System.IO.Stream stream, string protocol)
        => new(stream, MultistreamMuxer.ProtocolId, protocol);

    /// <inheritdoc/>
    public override void Flush() => _stream.Flush();

    /// <inheritdoc/>
    public override Task FlushAsync(CancellationToken cancellationToken) => _stream.FlushAsync(cancellationToken);

    /// <inheritdoc/>
    public override int Read(byte[] buffer, int offset, int count)
    {
        var task = ReadAsync(buffer, offset, count, CancellationToken.None);
        _ = task.Wait(ReadTimeout);
        return task.IsFaulted ? -1 : task.Result;
    }

    /// <inheritdoc/>
    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        await _handshaker.EnsureHandshakeCompleteAsync(MultistreamHandshaker.HandshakeDirection.Incoming, cancellationToken).ConfigureAwait(false);

        return buffer.Length == 0
            ? 0
            : await _stream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

    /// <inheritdoc/>
    public override void SetLength(long value) => throw new NotSupportedException();

    /// <inheritdoc/>
    public override void Write(byte[] buffer, int offset, int count) => WriteAsync(buffer, offset, count, CancellationToken.None).Wait(WriteTimeout);

    /// <inheritdoc/>
    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        await _handshaker.EnsureHandshakeCompleteAsync(MultistreamHandshaker.HandshakeDirection.Outgoing, cancellationToken).ConfigureAwait(false);
        await _stream.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _handshaker?.Dispose();
            _stream?.Dispose();
        }
    }
}