using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace update.net
{
    interface IUpdater
    {
        event EventHandler UpdateDownloaded;
        event EventHandler UpdateDownloadProgressChanged;

        /* Methods used to download the latest version info and compare
         * it to the version provided */
        bool IsUpdateAvailable(int currentVersion);
        int GetLatestVersion();

        void DownloadUpdate();
        void RunUpdate(string args = null);

        string GetChangelog();

        void Clean();
    }
}
