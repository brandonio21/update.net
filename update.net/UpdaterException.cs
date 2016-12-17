using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace update.net
{
    /// <summary>
    /// An Exception that is specific to update.net
    /// </summary>
    public class UpdaterException : Exception
    {
        /// <summary>
        /// Construct the UpdaterException with no other info
        /// </summary>
        public UpdaterException() : base() { }

        /// <summary>
        /// Construct the UpdaterException with only an inner exception. The exception's
        /// message is inferred from the inner exception
        /// </summary>
        /// <param name="inner">The inner exception of the UpdaterException</param>
        public UpdaterException(Exception inner) : base(inner.Message, inner) { }

        /// <summary>
        /// Construct an UpdaterException with only a message
        /// </summary>
        /// <param name="message">The message to construct the UpdaterException with</param>
        public UpdaterException(string message) : base(message) { }
    }

    /// <summary>
    /// Used to alert the user that there is no network available
    /// </summary>
    public class NetworkNotFoundException : UpdaterException
    {
        /// <summary>
        /// Construct a NetworkNotFoundException with an inner exception.
        /// </summary>
        /// <param name="inner">The inner exception of the NetworkNotFoundException</param>
        public NetworkNotFoundException(Exception inner) : base(inner) { }

        /// <summary>
        /// Construct a NetworkNotFoundException with only a message
        /// </summary>
        /// <param name="message">The message of the NetworkNotFoundException</param>
        public NetworkNotFoundException(string message) : base(message) { }
    }

    /// <summary>
    /// Used to alert the user that there was an error retrieving the Version file
    /// </summary>
    public class VersionFileNotFoundException : UpdaterException
    {
        /// <summary>
        /// Construct a VersionFileNotFoundException with an inner exception.
        /// </summary>
        /// <param name="inner">The inner exception of the VersionFileNotFoundException</param>
        public VersionFileNotFoundException(Exception inner) : base(inner) {}
    }

    /// <summary>
    /// Used to alert the user that there was an error retrieving the update file
    /// </summary>
    public class UpdateFileNotFoundException : UpdaterException
    {
        /// <summary>
        /// Construct a UpdateFileNotFoundException with an inner exception.
        /// </summary>
        /// <param name="inner">The inner exception of the UpdateFileNotFoundException</param>
        public UpdateFileNotFoundException(Exception inner) : base(inner) { }
    }

    /// <summary>
    /// Used to alert the user that there was an error retrieving the changelog file
    /// </summary>
    public class ChangelogFileNotFoundException : UpdaterException
    {
        /// <summary>
        /// Construct a ChangelogFileNotFoundException with an inner exception.
        /// </summary>
        /// <param name="inner">The inner exception of the ChangelogFileNotFoundException</param>
        public ChangelogFileNotFoundException(Exception inner) : base(inner) { }
    }
}
