using Multiformats.Base;

namespace Multiformats.Hash.CLI;

/// <summary>
/// Represents the options for the Multiformats.Hash CLI application.
/// </summary>
public class Options
{
    /// <summary>
    /// Gets or sets the hash algorithm to use.
    /// </summary>
    public HashType Algorithm { get; set; } = HashType.SHA2_256;

    /// <summary>
    /// Gets or sets the checksum value.
    /// </summary>
    public string Checksum { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the multibase encoding to use.
    /// </summary>
    public MultibaseEncoding Encoding { get; set; } = MultibaseEncoding.Base58Btc;

    /// <summary>
    /// Gets or sets the length of the hash output. Default is -1 (no limit).
    /// </summary>
    public int Length { get; set; } = -1;

    /// <summary>
    /// Gets or sets a value indicating whether to suppress output.
    /// </summary>
    public bool Quiet { get; set; } = false;

    /// <summary>
    /// Gets or sets the source stream for input data.
    /// </summary>
    public Stream? Source { get; set; } = null;

    /// <inheritdoc/>
    public override string ToString() =>
        $" Algorithm: {Algorithm}\n Checksum: {Checksum}\n Encoding: {Encoding}\n Length: {Length}\n Quiet: {Quiet}\n Source: {Source != null}";
}
