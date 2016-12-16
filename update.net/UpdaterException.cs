using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace update.net
{
    public class UpdaterException : Exception
    {
        public UpdaterException() : base() { }
        public UpdaterException(Exception inner) : base(inner.Message, inner) { }
        public UpdaterException(string message) :
            base(message) { }
    }

    public class NetworkNotFoundException : UpdaterException
    {
        public NetworkNotFoundException(Exception inner) : base(inner) { }
        public NetworkNotFoundException(string message) :
            base(message) { }
    }

    public class VersionFileNotFoundException : UpdaterException
    {
        public VersionFileNotFoundException(Exception inner) : base(inner) {}
    }

    public class UpdateFileNotFoundException : UpdaterException
    {
        public UpdateFileNotFoundException(Exception inner) : base(inner) { }
    }

    public class ChangelogFileNotFoundException : UpdaterException
    {
        public ChangelogFileNotFoundException(Exception inner) : base(inner) { }
    }
}
