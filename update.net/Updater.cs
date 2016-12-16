using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace update.net
{
    /* An Updater implementation of the interface that contains some
     * common functionality that may be utilized for all updaters */
    public abstract class Updater : IUpdater
    {
        protected string versionURL;
        protected string updaterURL;
        protected string changelogURL;
        protected string username;
        protected string password;
        protected string updaterPath;

        public event EventHandler UpdateDownloaded;
        public event EventHandler UpdateDownloadProgressChanged;

        public Updater()
        {
            this.versionURL = String.Empty;
            this.updaterURL = String.Empty;
            this.changelogURL = String.Empty;
            this.username = String.Empty;
            this.password = String.Empty;
        }

        public Updater(
            string versionURL,
            string updaterURL,
            string directory,
            string changelogURL = null,
            string username = null,
            string password = null)
        {
            versionURL.AssertNotNullNotEmpty("versionURL");
            updaterURL.AssertNotNullNotEmpty("updaterURL");
            directory.AssertNotNullNotEmpty("directory");

            this.versionURL = versionURL;
            this.updaterURL = updaterURL;
            this.changelogURL = changelogURL;
            this.username = username == null ? String.Empty : username;
            this.password = password == null ? String.Empty : password;

            // Create the update directory if it doesn't exist already
            System.IO.Directory.CreateDirectory(directory);

            string updaterName = Path.GetFileName(updaterURL);
            this.updaterPath = Path.Combine(directory, updaterName);
        }

        protected WebClient GetWebClient(
            Encoding encoding = null)
        {
            Encoding webClientEncoding = encoding != null ? encoding : System.Text.Encoding.UTF8;
            var webclient = new WebClient {
                Encoding = webClientEncoding,
            };
            webclient.Credentials = new NetworkCredential(
                this.username, this.password);

            return webclient;
        }

        public bool IsUpdateAvailable(int currentVersion)
        {
            int latestVersion = this.GetLatestVersion();
            return latestVersion > currentVersion;
        }

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

        public void Clean()
        {
            if (File.Exists(this.updaterPath))
            {
                File.Delete(this.updaterPath);
            }
        }

        public abstract int GetLatestVersion();
        public abstract void DownloadUpdate();
        public abstract void RunUpdate(string args = null);
        public abstract string GetChangelog();
    }
}
