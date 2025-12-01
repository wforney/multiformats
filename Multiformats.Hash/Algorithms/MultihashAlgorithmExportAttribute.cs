using System.Composition;

namespace Multiformats.Hash.Algorithms;

/// <summary>
/// Attribute used to export a multihash algorithm implementation for composition.
/// </summary>
/// <remarks>This attribute should be applied to classes implementing a multihash algorithm.</remarks>
[AttributeUsage(AttributeTargets.Class)]
[MetadataAttribute]
public class MultihashAlgorithmExportAttribute(HashType code, string name, int defaultLength) : Attribute, IMultihashAlgorithmMetadata
{
    /// <inheritdoc/>
    public HashType Code { get; } = code;

    /// <inheritdoc/>
    public int DefaultLength { get; } = defaultLength;

    /// <inheritdoc/>
    public string Name { get; } = name;
}
