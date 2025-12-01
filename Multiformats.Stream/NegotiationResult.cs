namespace Multiformats.Stream;

/// <summary>
/// Represents the result of a multistream protocol negotiation.
/// </summary>
/// <param name="protocol">The negotiated protocol identifier, or <c>null</c> if negotiation failed.</param>
/// <param name="handler">
/// The handler associated with the negotiated protocol, or <c>null</c> if negotiation failed.
/// </param>
public class NegotiationResult(string? protocol = null, IMultistreamHandler? handler = null)
{
    /// <summary>
    /// Gets the handler associated with the negotiated protocol.
    /// </summary>
    public IMultistreamHandler? Handler { get; } = handler;

    /// <summary>
    /// Gets the negotiated protocol identifier.
    /// </summary>
    public string? Protocol { get; } = protocol;
}