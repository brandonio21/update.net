/******************************************************************************
 * Updater Library
 * Created by Brandon Milton
 * http://brandonsoft.com
 * http://brandonio21.com
 * http://github.com/brandonio21
 *
 * Project github: https://github.com/brandonio21/update.net
 *
 * June 23, 2014
 *
 * The purpose of this project is to provide a functional update library for
 * use in .NET applications without having to mess with dealing with webclients,
 * etc. It was inspired by my constant need to redesign update systems entirely
 * because of UI upgrades, but the backend never changed. Thus, this will
 * serve as a constant backend.
 ******************************************************************************/
using System;
using System.Net;
using System.IO;

namespace update.net
{
    /// <summary>
    /// An Updater implementation that uses HTTP WebClients to download the
    /// version and changelogs from plaintext files and download the updater
    /// directly.
    /// </summary>
    public class HTTPPlaintextUpdater : Updater
    {
        /// <inheritdoc/>
        public HTTPPlaintextUpdater(
            string versionURL,
            string updaterURL,
            string directory,
            string changelogURL = null,
            string username = null,
            string password = null) :
            base(versionURL, updaterURL, directory, changelogURL, username, password) { }

        /// <summary>
        /// Gets the latest version by reading the version string from the version URL,
        /// trimming whitespace from either side, and casting to int
        /// </summary>
        /// <returns>The latest version</returns>
        /// <throws>NetworkNotFoundException if no network is available</throws>
        /// <throws>VersionFileNotFoundException if a problem occured reading the version file</throws>
        public override int GetLatestVersion()
        {
            NetworkUtils.AssertNetworkIsAvailable();

            using (WebClient webclient = this.GetWebClient())
            {
                try
                {
                    /* Download the version file string */
                    string version = webclient.DownloadString(this.versionURL);
                    return int.Parse(version.Trim());
                }
                catch (Exception e)
                {
                    throw new VersionFileNotFoundException(e);
                }
            }
        }

        /// <summary>
        /// Asynchronously starts the download of the updater executable. Monitoring of the
        /// download is delegated to the UpdateDownloaded and UpdateDownloadProgressChanged events
        /// </summary>
        /// <throws>NetworkNotFoundException if there is no network connection</throws>
        /// <throws>UpdateFileNotFoundException if there is an error getting the update file</throws>
        public override void DownloadUpdate()
        {
            NetworkUtils.AssertNetworkIsAvailable();

            using (WebClient webclient = this.GetWebClient())
            {
                try
                {
                    var updaterUri = new System.Uri(this.updaterURL);

                    webclient.DownloadFileCompleted += this.HandleDownloadUpdateComplete;
                    webclient.DownloadProgressChanged += this.HandleDownloadUpdateProgressChange;
                    webclient.DownloadFileAsync(updaterUri, this.updaterPath);
                }
                catch (Exception e)
                {
                    throw new UpdateFileNotFoundException(e);
                }
            }
        }

        /// <summary>
        /// Downloads the changelog text from the changelog file URL and returns it
        /// after trimming its beginning and end.
        /// </summary>
        /// <returns>The trimmed changelog text</returns>
        /// <throws>ArgumentNullException if the updater was constructed without a changelog url</throws>
        /// <throws>NetworkNotFoundException if no network was found</throws>
        /// <throws>ChangelogFileNotFoundException if a problem occurred while getting the changelog</throws>
        public override string GetChangelog()
        {
            this.changelogURL.AssertNotNullNotEmpty("ChangelogURL");
            NetworkUtils.AssertNetworkIsAvailable();

            using (WebClient webclient = this.GetWebClient())
            {
                try
                {
                    return webclient.DownloadString(this.changelogURL).Trim();
                }
                catch (Exception e)
                {
                    throw new ChangelogFileNotFoundException(e);
                }
            }
        }

     
    }
}