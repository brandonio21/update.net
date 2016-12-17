using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace update.net
{
    /// <summary>
    /// An interface specification for all types of updaters. Updaters have the ability to
    /// synchronously check versions, download changelogs, and asynchronously download and
    /// run updates.
    /// </summary>
    public interface IUpdater
    {
        /// <summary>
        /// An Event indicating that the updater download has completed
        /// </summary>
        event EventHandler UpdateDownloaded;

        /// <summary>
        /// An Event indicating that the updater download has progressed
        /// </summary>
        event EventHandler UpdateDownloadProgressChanged;

        /// <summary>
        /// Checks to see if a new update is available by comparing the provided version to
        /// the one retrieved with a call to GetLatestVersion. An update is considered
        /// available if the new version is greater than the current version
        /// </summary>
        /// <param name="currentVersion">The current running version of the application</param>
        /// <returns>A boolean indicating whether the provided version is out of date</returns>
        bool IsUpdateAvailable(int currentVersion);

        /// <summary>
        /// Performs an I/O operation to get the latest version and returns it
        /// </summary>
        /// <returns>The latest version number</returns>
        int GetLatestVersion();

        /// <summary>
        /// Asyncrhonously downloads the update associated with the Updater object. The
        /// progress and completion of the update can be tracked with the UpdateDownloaded
        /// and UpdateDownloadProgressChanged events
        /// </summary>
        void DownloadUpdate();

        /// <summary>
        /// Runs the downloaded update. It is recommended to run this method only after
        /// the UpdateDownloaded event has been fired.
        /// </summary>
        /// <param name="args">Arguments that may be passed to the update executable</param>
        void RunUpdate(string args = null);

        /// <summary>
        /// Performs an I/O operation to get the changelog and returns it
        /// </summary>
        /// <returns>The downloaded changelog</returns>
        string GetChangelog();

        /// <summary>
        /// Cleans up the resources of the Updater object, usually by deleting
        /// all downloaded changelogs, version caches, and updaters.
        /// </summary>
        void Clean();
    }
}
