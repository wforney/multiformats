namespace Multiformats.Stream;

/// <summary>
/// Represents a handler for a specific multistream protocol.
/// </summary>
public interface IMultistreamHandler
{
    /// <summary>
    /// Gets the protocol identifier that this handler supports.
    /// </summary>
    string Protocol { get; }

    /// <summary>
    /// Handles a stream for the specified protocol.
    /// </summary>
    /// <param name="protocol">The protocol identifier.</param>
    /// <param name="stream">The stream to handle.</param>
    /// <returns><c>true</c> if the stream was handled successfully; otherwise, <c>false</c>.</returns>
    bool Handle(string protocol, System.IO.Stream stream);

    /// <summary>
    /// Asynchronously handles a stream for the specified protocol.
    /// </summary>
    /// <param name="protocol">The protocol identifier.</param>
    /// <param name="stream">The stream to handle.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains <c>true</c> if
    /// the stream was handled successfully; otherwise, <c>false</c>.
    /// </returns>
    Task<bool> HandleAsync(string protocol, System.IO.Stream stream, CancellationToken cancellationToken);
}