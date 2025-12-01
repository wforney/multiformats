using System.Text;

namespace Multiformats.Stream;

/// <summary>
/// Handles the multistream handshake process for protocol negotiation.
/// </summary>
/// <param name="ms">The multistream instance to use for communication.</param>
/// <param name="timeout">The timeout for handshake operations.</param>
/// <param name="protocols">The list of protocols to negotiate.</param>
internal class MultistreamHandshaker(Multistream ms, TimeSpan timeout, IEnumerable<string> protocols) : IDisposable
{
    private readonly ReaderWriterLockSlim _lock = new();
    private readonly SemaphoreSlim _readLock = new(1, 1);
    private readonly SemaphoreSlim _writeLock = new(1, 1);
    private bool _hasReceived;
    private bool _hasSent;

    /// <summary>
    /// Specifies the direction of the handshake.
    /// </summary>
    public enum HandshakeDirection
    {
        /// <summary>
        /// Outgoing handshake (initiator).
        /// </summary>
        Outgoing,

        /// <summary>
        /// Incoming handshake (responder).
        /// </summary>
        Incoming
    }

    /// <summary>
    /// Gets a value indicating whether the handshake has been received.
    /// </summary>
    public bool HasReceived => _lock.Read(() => _hasReceived, (int)timeout.TotalMilliseconds);

    /// <summary>
    /// Gets a value indicating whether the handshake has been sent.
    /// </summary>
    public bool HasSent => _lock.Read(() => _hasSent, (int)timeout.TotalMilliseconds);

    /// <summary>
    /// Gets a value indicating whether the handshake is complete.
    /// </summary>
    public bool IsComplete => _lock.Read(() => _hasSent && _hasReceived, (int)timeout.TotalMilliseconds);

    /// <inheritdoc />
    public void Dispose()
    {
        _lock?.Dispose();
        _readLock?.Dispose();
        _writeLock?.Dispose();
    }

    /// <summary>
    /// Ensures the handshake process is complete in the specified direction.
    /// </summary>
    /// <param name="direction">The handshake direction.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task EnsureHandshakeCompleteAsync(HandshakeDirection direction, CancellationToken cancellationToken)
    {
        return IsComplete || _writeLock.CurrentCount == 0
            ? Task.CompletedTask
            : direction switch
            {
                HandshakeDirection.Outgoing => Task.WhenAll(ReadHandshakeAsync(cancellationToken), WriteHandshakeAsync(cancellationToken)),
                HandshakeDirection.Incoming => Task.WhenAll(WriteHandshakeAsync(cancellationToken), ReadHandshakeAsync(cancellationToken)),
                _ => Task.FromResult(true),
            };
    }

    /// <summary>
    /// Reads the handshake from the stream and validates the protocol.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task ReadHandshakeAsync(CancellationToken cancellationToken)
    {
        if (HasReceived)
        {
            return;
        }

        if (!await _readLock.WaitAsync(timeout, cancellationToken).ConfigureAwait(false))
        {
            throw new TimeoutException("Receiving handshake timed out.");
        }

        try
        {
            _ = _lock.Write(() => _hasReceived = true, (int)timeout.TotalMilliseconds);

            foreach (var protocol in protocols)
            {
                var token = await MultistreamMuxer.ReadNextTokenAsync(ms, cancellationToken).ConfigureAwait(false);

                if (token != protocol)
                {
                    throw new Exception($"Protocol mismatch, {token} != {protocol}");
                }
            }
        }
        finally
        {
            _ = _readLock.Release();
        }
    }

    /// <summary>
    /// Writes the handshake to the stream for protocol negotiation.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task WriteHandshakeAsync(CancellationToken cancellationToken)
    {
        if (HasSent)
        {
            return;
        }

        if (!await _writeLock.WaitAsync(timeout, cancellationToken).ConfigureAwait(false))
        {
            throw new TimeoutException("Sending handshake timed out.");
        }

        try
        {
            _ = _lock.Write(() => _hasSent = true, (int)timeout.TotalMilliseconds);

            foreach (var protocol in protocols)
            {
                await MultistreamMuxer.DelimWriteAsync(ms, Encoding.UTF8.GetBytes(protocol), cancellationToken).ConfigureAwait(false);
            }

            await ms.FlushAsync(cancellationToken);
        }
        finally
        {
            _ = _writeLock.Release();
        }
    }
}