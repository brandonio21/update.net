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
    public class HTTPPlaintextUpdater : Updater
    {
        public HTTPPlaintextUpdater(
            string versionURL,
            string updaterURL,
            string directory,
            string changelogURL = null,
            string username = null,
            string password = null) :
            base(versionURL, updaterURL, directory, changelogURL, username, password) { }

        public override int GetLatestVersion()
        {
            NetworkUtils.AssertNetworkIsAvailable();

            using (WebClient webclient = this.GetWebClient())
            {
                try
                {
                    // Download the version file string
                    string version = webclient.DownloadString(this.versionURL);
                    return int.Parse(version.Trim());
                }
                catch (Exception e)
                {
                    throw new VersionFileNotFoundException(e);
                }
            }
        }

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

        public override void RunUpdate(string args = null)
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