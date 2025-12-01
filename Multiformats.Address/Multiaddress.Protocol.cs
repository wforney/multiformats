using Multiformats.Address.Protocols;

namespace Multiformats.Address;

public partial class Multiaddress
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Protocol"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="code">The code.</param>
    /// <param name="size">The size.</param>
    /// <param name="type">The type.</param>
    /// <param name="path">if set to <c>true</c> [path].</param>
    /// <param name="factory">The factory.</param>
    private class Protocol(
        string name,
        int code,
        int size,
        Type type,
        bool path,
        Func<object?, MultiaddressProtocol> factory
        )
    {
        /// <summary>
        /// Gets the code.
        /// </summary>
        /// <value>The code.</value>
        public int Code { get; } = code;

        /// <summary>
        /// Gets the factory.
        /// </summary>
        /// <value>The factory.</value>
        public Func<object?, MultiaddressProtocol> Factory { get; } = factory;

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; } = name;

        /// <summary>
        /// Gets a value indicating whether this <see cref="Protocol"/> is path.
        /// </summary>
        /// <value><c>true</c> if path; otherwise, <c>false</c>.</value>
        public bool Path { get; } = path;

        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <value>The size.</value>
        public int Size { get; } = size;

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public Type Type { get; } = type;
    }
}
