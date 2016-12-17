using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace update.net
{
    /// <summary>
    /// An IUpdater implementation that contains many non-implementation
    /// specific helper functions and default implementations.
    /// </summary>
    public abstract class Updater : IUpdater
    {
        /// <summary>
        /// The URL of the version file to read from
        /// </summary>
        protected string versionURL;

        /// <summary>
        /// The URL of the updater executable to download
        /// </summary>
        protected string updaterURL;

        /// <summary>
        /// The URL of the changelog file to read from
        /// </summary>
        protected string changelogURL;

        /// <summary>
        /// The username to use when performing network operations
        /// (such as reading version file, downloading updater, reading changelog)
        /// </summary>
        protected string username;

        /// <summary>
        /// The password to use when performing network operations
        /// (such as reading version file, downloading updater, reading changelog)
        /// </summary>
        protected string password;

        /// <summary>
        /// The local path to download the updater to (inferred from the
        /// URL of the updater)
        /// </summary>
        protected string updaterPath;

        /// <summary>
        /// An Event signalling that the Updater has finished downloading
        /// </summary>
        public event EventHandler UpdateDownloaded;

        /// <summary>
        /// An event signalling that the Updater download has made progress
        /// </summary>
        public event EventHandler UpdateDownloadProgressChanged;

        /// <summary>
        /// Constructs an updater with empty strings in place of all data fields.
        /// When using this constructor, no subsequent method calls should function
        /// as expected.
        /// </summary>
        public Updater()
        {
            this.versionURL = String.Empty;
            this.updaterURL = String.Empty;
            this.changelogURL = String.Empty;
            this.username = String.Empty;
            this.password = String.Empty;
        }

        /// <summary>
        /// Constructs an Updater with the specified information, ensuring that all
        /// required fields are non-null. Also creates the specified directory if it
        /// does not exist.
        /// </summary>
        /// <param name="versionURL">The URL Path to the version file</param>
        /// <param name="updaterURL">The URL Path to the Updater executable</param>
        /// <param name="directory">The local directory to store updater information</param>
        /// <param name="changelogURL">The URL Path to the changelog file</param>
        /// <param name="username">A username to use when downloading files</param>
        /// <param name="password">A password to use when downloading files</param>
        public Updater(
            string versionURL,
            string updaterURL,
            string directory,
            string changelogURL = null,
            string username = null,
            string password = null)
        {
            /* Ensure required fields are provided */
            versionURL.AssertNotNullNotEmpty("versionURL");
            updaterURL.AssertNotNullNotEmpty("updaterURL");
            directory.AssertNotNullNotEmpty("directory");

            this.versionURL = versionURL;
            this.updaterURL = updaterURL;
            this.changelogURL = changelogURL;
            this.username = username == null ? String.Empty : username;
            this.password = password == null ? String.Empty : password;

            /* Create the update directory if it doesn't exist already */
            System.IO.Directory.CreateDirectory(directory);

            /* Infer the updater path from the directory and its URL */
            string updaterName = Path.GetFileName(updaterURL);
            this.updaterPath = Path.Combine(directory, updaterName);
        }

        /// <summary>
        /// Creates a WebClient for this updater
        /// </summary>
        /// <param name="encoding">The Encoding to use in the WebClient when downloading files</param>
        /// <returns>WebClient to use in network operations</returns>
        protected WebClient GetWebClient(Encoding encoding = null)
        {
            Encoding webClientEncoding = encoding != null ? encoding : System.Text.Encoding.UTF8;
            var webclient = new WebClient {
                Encoding = webClientEncoding,
            };
            webclient.Credentials = new NetworkCredential(
                this.username, this.password);

            return webclient;
        }

        
        /// <inheritdoc />
        public bool IsUpdateAvailable(int currentVersion)
        {
            int latestVersion = this.GetLatestVersion();
            return latestVersion > currentVersion;
        }

        /// <summary>
        /// A helper method which handles the WebClient's DownloadComplete for the updater
        /// executable. Simply asserts that the UpdateDownloded event is non-null and calls it.
        /// </summary>
        protected void HandleDownloadUpdateComplete(
            object sender,
            System.ComponentModel.AsyncCompletedEventArgs e)
        {
            var handler = UpdateDownloaded;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// A helper method which handles the WebClient's DownloadProgressChanged for the updater
        /// executable. Simply asserts that the UpdateDownloadProgressChanged event is non-null and calls it.
        /// </summary>
        protected void HandleDownloadUpdateProgressChange(
            object sender,
            System.Net.DownloadProgressChangedEventArgs e)
        {
            var handler = UpdateDownloadProgressChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <inheritdoc/>
        /// <throws>UpdateFileNotFoundException if a problem occurred while running the updater</throws>
        public void RunUpdate(string args = null)
        {
            try
            {
                var updaterProcessInfo = new System.Diagnostics.ProcessStartInfo(
                    this.updaterPath, args);

                System.Diagnostics.Process.Start(updaterProcessInfo);
            }
            catch (Exception e)
            {
                throw new UpdateFileNotFoundException(e);
            }
        }

        /// <inheritdoc/>
        public void Clean()
        {
            if (File.Exists(this.updaterPath))
            {
                File.Delete(this.updaterPath);
            }
        }

        /// <inheritdoc/>
        public abstract int GetLatestVersion();

        /// <inheritdoc/>
        public abstract void DownloadUpdate();

        /// <inheritdoc/>
        public abstract string GetChangelog();
    }
}
