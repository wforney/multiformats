namespace Multiformats.Stream;

/// <summary>
/// Provides extension methods for <see cref="ReaderWriterLockSlim"/> to simplify read and write
/// lock usage.
/// </summary>
public static class ReaderWriterLockSlimExtensions
{
    /// <summary>
    /// Executes the specified <paramref name="action"/> within a read lock.
    /// </summary>
    /// <param name="rwls">The <see cref="ReaderWriterLockSlim"/> instance.</param>
    /// <param name="action">The action to execute.</param>
    /// <param name="timeout">
    /// The timeout in milliseconds to acquire the read lock. Default is <see cref="Timeout.Infinite"/>.
    /// </param>
    /// <exception cref="TimeoutException">
    /// Thrown if the read lock cannot be acquired within the specified timeout.
    /// </exception>
    public static void Read(this ReaderWriterLockSlim rwls, Action action, int timeout = Timeout.Infinite)
    {
        if (!rwls.TryEnterReadLock(timeout))
        {
            throw new TimeoutException("Read deadlock");
        }

        try
        {
            action();
        }
        finally
        {
            rwls.ExitReadLock();
        }
    }

    /// <summary>
    /// Executes the specified <paramref name="func"/> within a read lock and returns its result.
    /// </summary>
    /// <typeparam name="T">The return type of the function.</typeparam>
    /// <param name="rwls">The <see cref="ReaderWriterLockSlim"/> instance.</param>
    /// <param name="func">The function to execute.</param>
    /// <param name="timeout">
    /// The timeout in milliseconds to acquire the read lock. Default is <see cref="Timeout.Infinite"/>.
    /// </param>
    /// <returns>The result of the function.</returns>
    /// <exception cref="TimeoutException">
    /// Thrown if the read lock cannot be acquired within the specified timeout.
    /// </exception>
    public static T Read<T>(this ReaderWriterLockSlim rwls, Func<T> func, int timeout = Timeout.Infinite)
    {
        if (!rwls.TryEnterReadLock(timeout))
        {
            throw new TimeoutException("Read deadlock");
        }

        try
        {
            return func();
        }
        finally
        {
            rwls.ExitReadLock();
        }
    }

    /// <summary>
    /// Executes the specified <paramref name="action"/> within a write lock.
    /// </summary>
    /// <param name="rwls">The <see cref="ReaderWriterLockSlim"/> instance.</param>
    /// <param name="action">The action to execute.</param>
    /// <param name="timeout">
    /// The timeout in milliseconds to acquire the write lock. Default is <see cref="Timeout.Infinite"/>.
    /// </param>
    /// <exception cref="TimeoutException">
    /// Thrown if the write lock cannot be acquired within the specified timeout.
    /// </exception>
    public static void Write(this ReaderWriterLockSlim rwls, Action action, int timeout = Timeout.Infinite)
    {
        if (!rwls.TryEnterWriteLock(timeout))
        {
            throw new TimeoutException("Write deadlock");
        }

        try
        {
            action();
        }
        finally
        {
            rwls.ExitWriteLock();
        }
    }

    /// <summary>
    /// Executes the specified <paramref name="func"/> within a write lock and returns its result.
    /// </summary>
    /// <typeparam name="T">The return type of the function.</typeparam>
    /// <param name="rwls">The <see cref="ReaderWriterLockSlim"/> instance.</param>
    /// <param name="func">The function to execute.</param>
    /// <param name="timeout">
    /// The timeout in milliseconds to acquire the write lock. Default is <see cref="Timeout.Infinite"/>.
    /// </param>
    /// <returns>The result of the function.</returns>
    /// <exception cref="TimeoutException">
    /// Thrown if the write lock cannot be acquired within the specified timeout.
    /// </exception>
    public static T Write<T>(this ReaderWriterLockSlim rwls, Func<T> func, int timeout = Timeout.Infinite)
    {
        if (!rwls.TryEnterWriteLock(timeout))
        {
            throw new TimeoutException("Write deadlock");
        }

        try
        {
            return func();
        }
        finally
        {
            rwls.ExitWriteLock();
        }
    }
}