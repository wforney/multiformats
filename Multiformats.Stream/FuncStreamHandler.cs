namespace Multiformats.Stream;

/// <summary>
/// Represents an asynchronous stream handler delegate.
/// </summary>
/// <param name="protocol">The protocol identifier.</param>
/// <param name="stream">The stream to handle.</param>
/// <param name="cancellationToken">A cancellation token.</param>
/// <returns>A task that resolves to <c>true</c> if handled; otherwise, <c>false</c>.</returns>
public delegate Task<bool> AsyncStreamHandlerFunc(string protocol, System.IO.Stream stream, CancellationToken cancellationToken);

/// <summary>
/// Represents a synchronous stream handler delegate.
/// </summary>
/// <param name="protocol">The protocol identifier.</param>
/// <param name="stream">The stream to handle.</param>
/// <returns><c>true</c> if handled; otherwise, <c>false</c>.</returns>
public delegate bool StreamHandlerFunc(string protocol, System.IO.Stream stream);

/// <summary>
/// Provides a stream handler implementation for a specific protocol.
/// </summary>
/// <param name="protocol">The protocol identifier.</param>
/// <param name="handle">The synchronous handler delegate.</param>
/// <param name="asyncHandle">The asynchronous handler delegate.</param>
internal class FuncStreamHandler(
    string protocol,
    StreamHandlerFunc? handle = null,
    AsyncStreamHandlerFunc? asyncHandle = null)
    : IMultistreamHandler
{
    /// <inheritdoc/>
    public string Protocol { get; } = protocol;

    /// <inheritdoc/>
    public bool Handle(string protocol, System.IO.Stream stream)
    {
        return handle is null
            ? asyncHandle is not null && asyncHandle.Invoke(protocol, stream, CancellationToken.None)
                    .ConfigureAwait(true)
                    .GetAwaiter()
                    .GetResult()
            : handle.Invoke(protocol, stream);
    }

    /// <inheritdoc/>
    public Task<bool> HandleAsync(string protocol, System.IO.Stream stream, CancellationToken cancellationToken)
    {
        return asyncHandle is null
            ? handle is null
                ? Task.FromResult(false)
                : Task.Factory.FromAsync(
                    static (p, s, cb, o) =>
                    {
                        var func = (Func<string, System.IO.Stream, bool>?)o;
                        return func!.BeginInvoke(p, s, cb, o)!;
                    },
                    static (ar) =>
                    {
                        var func = (Func<string, System.IO.Stream, bool>?)ar.AsyncState;
                        return func!.EndInvoke(ar);
                    },
                    protocol, stream, state: handle)
            : asyncHandle(protocol, stream, cancellationToken);
    }
}