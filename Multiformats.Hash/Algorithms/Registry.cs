using System.Collections.Concurrent;
using System.Composition;
using System.Composition.Hosting;
using System.Reflection;

namespace Multiformats.Hash.Algorithms;

/// <summary>
/// Provides a registry for multihash algorithms, supporting retrieval, usage, and disposal.
/// </summary>
internal class Registry : IDisposable
{
    /// <summary>
    /// The collection of available multihash algorithm factories and their metadata.
    /// </summary>
    private readonly IEnumerable<ExportFactory<IMultihashAlgorithm, MultihashAlgorithmMetadata>> _algorithms;

    /// <summary>
    /// A cache of active algorithm exports, keyed by their hash code.
    /// </summary>
    private readonly ConcurrentDictionary<int, Export<IMultihashAlgorithm>> _cache;

    /// <summary>
    /// The MEF composition host container.
    /// </summary>
    private readonly CompositionHost _container;

    /// <summary>
    /// Initializes a new instance of the <see cref="Registry"/> class.
    /// </summary>
    public Registry()
    {
        _container = new ContainerConfiguration()
            .WithAssembly(typeof(Registry).GetTypeInfo().Assembly)
            .CreateContainer();

        _algorithms = _container.GetExports<ExportFactory<IMultihashAlgorithm, MultihashAlgorithmMetadata>>();
        _cache = new ConcurrentDictionary<int, Export<IMultihashAlgorithm>>();
    }

    /// <summary>
    /// Gets the supported hash types provided by the registry.
    /// </summary>
    public HashType[] SupportedHashTypes => [.. _algorithms.Select(a => a.Metadata.Code).OrderBy(x => x.ToString())];

    /// <summary>
    /// Gets the <see cref="HashType"/> code for a given algorithm type.
    /// </summary>
    /// <typeparam name="TAlgorithm">The algorithm type.</typeparam>
    /// <returns>The hash type code, or null if not found.</returns>
    public static HashType? GetHashType<TAlgorithm>()
        where TAlgorithm : IMultihashAlgorithm =>
        typeof(TAlgorithm).GetTypeInfo().GetCustomAttribute<MultihashAlgorithmExportAttribute>()?.Code;

    /// <inheritdoc/>
    public void Dispose() => _container?.Dispose();

    /// <summary>
    /// Gets an instance of the algorithm for the specified hash type.
    /// </summary>
    /// <param name="type">The hash type code.</param>
    /// <returns>An instance of <see cref="IMultihashAlgorithm"/>.</returns>
    /// <exception cref="NotSupportedException">Thrown if the hash type is not supported.</exception>
    public IMultihashAlgorithm Get(HashType type)
    {
        var algo = _algorithms.SingleOrDefault(a => a.Metadata.Code.Equals(type))
            ?? throw new NotSupportedException($"{type} is not supported.");

        var export = algo.CreateExport();

        _ = _cache.TryAdd(algo.GetHashCode(), export);

        return export.Value;
    }

    /// <summary>
    /// Gets an instance of the specified algorithm type.
    /// </summary>
    /// <typeparam name="TAlgorithm">The algorithm type.</typeparam>
    /// <returns>An instance of <see cref="IMultihashAlgorithm"/>.</returns>
    /// <exception cref="NotSupportedException">Thrown if the algorithm type is not supported.</exception>
    public IMultihashAlgorithm Get<TAlgorithm>()
        where TAlgorithm : IMultihashAlgorithm
    {
        var code = GetHashType<TAlgorithm>();
        return !code.HasValue || !Enum.IsDefined(code.Value)
            ? throw new NotSupportedException($"{typeof(TAlgorithm)} is not supported.")
            : Get(code.Value);
    }

    /// <summary>
    /// Returns the algorithm instance to the registry, disposing its export.
    /// </summary>
    /// <param name="algo">The algorithm instance.</param>
    public void Return(IMultihashAlgorithm algo)
    {
        if (_cache.TryRemove(algo.GetHashCode(), out var export))
        {
            export?.Dispose();
        }
    }

    /// <summary>
    /// Uses the specified algorithm by hash type, executing the provided function.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="code">The hash type code.</param>
    /// <param name="func">The function to execute with the algorithm.</param>
    /// <returns>The result of the function.</returns>
    public T Use<T>(HashType code, Func<IMultihashAlgorithm, T> func)
    {
        var algorithm = Get(code);
        try
        {
            return func(algorithm);
        }
        finally
        {
            Return(algorithm);
        }
    }

    /// <summary>
    /// Uses the specified algorithm type, executing the provided function.
    /// </summary>
    /// <typeparam name="TAlgorithm">The algorithm type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="func">The function to execute with the algorithm.</param>
    /// <returns>The result of the function.</returns>
    public TResult Use<TAlgorithm, TResult>(Func<IMultihashAlgorithm, TResult> func)
        where TAlgorithm : IMultihashAlgorithm
    {
        var algorithm = Get<TAlgorithm>();
        try
        {
            return func(algorithm);
        }
        finally
        {
            Return(algorithm);
        }
    }

    /// <summary>
    /// Asynchronously uses the specified algorithm by hash type, executing the provided function.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="code">The hash type code.</param>
    /// <param name="func">The asynchronous function to execute with the algorithm.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task<T> UseAsync<T>(HashType code, Func<IMultihashAlgorithm, Task<T>> func)
    {
        var algorithm = Get(code);
        try
        {
            return await func(algorithm).ConfigureAwait(false);
        }
        finally
        {
            Return(algorithm);
        }
    }

    /// <summary>
    /// Asynchronously uses the specified algorithm type, executing the provided function.
    /// </summary>
    /// <typeparam name="TAlgorithm">The algorithm type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="func">The asynchronous function to execute with the algorithm.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task<TResult> UseAsync<TAlgorithm, TResult>(Func<IMultihashAlgorithm, Task<TResult>> func)
        where TAlgorithm : IMultihashAlgorithm
    {
        var algorithm = Get<TAlgorithm>();
        try
        {
            return await func(algorithm);
        }
        finally
        {
            Return(algorithm);
        }
    }
}
