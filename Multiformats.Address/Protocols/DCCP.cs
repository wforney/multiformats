namespace Multiformats.Address.Protocols;

/// <summary>
/// DCCP
/// </summary>
public record DCCP : Number{
    /// <summary>
    /// Constructor for the DCCP class.
    /// </summary>
    /// <returns>An instance of the DCCP class.</returns>
    public DCCP()
        : base("dccp", 33)    {    }

    /// <summary>
    /// Constructor for the DCCP class, taking an integer port as a parameter.
    /// </summary>
    /// <returns>An instance of the DCCP class with the given port value.</returns>
    public DCCP(int port)
        : this() => Value = port;}
