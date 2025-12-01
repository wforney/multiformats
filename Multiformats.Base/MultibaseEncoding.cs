namespace Multiformats.Base;

/// <summary>
/// Specifies the multibase encoding types.
/// </summary>
public enum MultibaseEncoding
{
    /// <summary>
    /// Identity encoding (no encoding).
    /// </summary>
    Identity,

    /// <summary>
    /// Base2 (binary) encoding.
    /// </summary>
    Base2,

    /// <summary>
    /// Base8 (octal) encoding.
    /// </summary>
    Base8,

    /// <summary>
    /// Base10 (decimal) encoding.
    /// </summary>
    Base10,

    /// <summary>
    /// Base16 (hexadecimal, lowercase) encoding.
    /// </summary>
    Base16Lower,

    /// <summary>
    /// Base16 (hexadecimal, uppercase) encoding.
    /// </summary>
    Base16Upper,

    /// <summary>
    /// Base32 (lowercase) encoding.
    /// </summary>
    Base32Lower,

    /// <summary>
    /// Base32 (uppercase) encoding.
    /// </summary>
    Base32Upper,

    /// <summary>
    /// Base32 (lowercase, padded) encoding.
    /// </summary>
    Base32PaddedLower,

    /// <summary>
    /// Base32 (uppercase, padded) encoding.
    /// </summary>
    Base32PaddedUpper,

    /// <summary>
    /// Base32Z encoding (z-base-32).
    /// </summary>
    Base32Z,

    /// <summary>
    /// Base32Hex (lowercase) encoding.
    /// </summary>
    Base32HexLower,

    /// <summary>
    /// Base32Hex (uppercase) encoding.
    /// </summary>
    Base32HexUpper,

    /// <summary>
    /// Base32Hex (lowercase, padded) encoding.
    /// </summary>
    Base32HexPaddedLower,

    /// <summary>
    /// Base32Hex (uppercase, padded) encoding.
    /// </summary>
    Base32HexPaddedUpper,

    /// <summary>
    /// Base58 encoding (Bitcoin alphabet).
    /// </summary>
    Base58Btc,

    /// <summary>
    /// Base58 encoding (Flickr alphabet).
    /// </summary>
    Base58Flickr,

    /// <summary>
    /// Base64 encoding.
    /// </summary>
    Base64,

    /// <summary>
    /// Base64 encoding with padding.
    /// </summary>
    Base64Padded,

    /// <summary>
    /// Base64 URL-safe encoding.
    /// </summary>
    Base64Url,

    /// <summary>
    /// Base64 URL-safe encoding with padding.
    /// </summary>
    Base64UrlPadded
}
