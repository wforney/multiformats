namespace Multiformats.Base;

/// <summary>
/// Specifies the casing behavior for letter characters.
/// </summary>
internal enum LetterCasing
{
    /// <summary>
    /// Letter casing is ignored.
    /// </summary>
    Ignore,

    /// <summary>
    /// Letters are converted to lower case.
    /// </summary>
    Lower,

    /// <summary>
    /// Letters are converted to upper case.
    /// </summary>
    Upper
}
