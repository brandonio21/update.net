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

namespace update.net
{
	public class updater
	{
		// Event handlers that can be delegated by the user
		public event EventHandler UpdateDownloaded;
		public event EventHandler UpdateDownloadProgressChanged;

		// Private vars to keep track of locations
		private string versionURL;
		private string updaterURL;
        private string changelogURL;

        // Private vars to keep track of username and passwords for DLs
        private string downloadUsername;
        private string downloadPassword;

		// Keeps track of custom updater directory (if specified)
		private string updaterDirectory;

		// Keeps track of the latest version number
		private string latestVersion = "";


		/** Updater Constructor
		 * This constructor initializes a new updater object for use.
		 *
		 * Parameters: versionURL - The location of the version file
		 *             updaterURL - The location of the update file
		 * Return:     A new instance of an updater object
		 */
		public updater(string versionURL, string updaterURL, string changelogURL)
		{
			// Set the private fields
			this.versionURL = versionURL;
			this.updaterURL = updaterURL;
            this.changelogURL = changelogURL;
		}

		/** Update Availability Check Method
		 * This method checks to see whether or not an update is available by 
		 * comparing the contents of the version file (version url) with the 
		 * provided parameter version.
		 *
		 * Parameters: currentVersion - The version of the software running
		 * Return:     A boolean indicating whether or not the software is out of 
		 *             date.
		 */
		public bool isUpdateAvailable(string currentVersion)
		{
			// First, check to see if the network is available
			if (!(System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable()))
			{
				// The network is not available, throw an exception
				throw new NetworkNotFoundException("Initial network not found");
			}
			else
			{
				// The network is properly functioning, move onto version file:
				System.Net.WebClient wclient = new System.Net.WebClient();
                wclient.Credentials = new System.Net.NetworkCredential(
                    this.downloadUsername, this.downloadPassword);

				string version; // will hold the version from the web
				try
				{
					// get the version from the web
					version = wclient.DownloadString(versionURL);


					// Remove any newline chars caused by some OS's
					if (version.Contains(Environment.NewLine))
						version = version.Replace(Environment.NewLine, "");

					this.latestVersion = version;
				}
				catch (Exception e)
				{
					// Some error occurred while DLing the version file.
					throw new VersionFileNotFoundException(e.Message);
				}

				// Return true if the versions are different
				// Return false if the versions are the same
				return !version.Equals(currentVersion);
			}
		}

        /** Download Update Method (With changelog switch)
         * This method downloads the update file asynchronously to a customly
         * defined directory. The file retains its name from the updaterURL. 
         * Once the update has downloaded, the changelog will also download
         * to a customly defined directory if the user says so.
         * 
         * Parameters: updaterDirectory   - The directory to download the update to
         *             downloadChangelog  - A boolean indicating whether or not to download
         *                                  the changelog after the update
         *             changelogDirectory - The directory where the changelog is downloaded
         * Return:     None
         */
        public void downloadUpdate(string updaterDirectory, bool downloadChangelog,
            string changelogDirectory)
        {
            // First, check to see if the network is available
            if (!(System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable()))
            {
                // The network is not available, throw an exception
                throw new NetworkNotFoundException("Initial Network Not Found");
            }
            else
            {
                // The network is available, let's download the update file! 
                try
                {
                    // Create the webclient and useful strings
                    System.Net.WebClient wclient = new System.Net.WebClient();
                    wclient.Credentials = new System.Net.NetworkCredential(this.downloadUsername,
                       this.downloadPassword);

                    // Create the entire destination string and the directory
                    string dest = updaterDirectory + "/" + System.IO.Path.GetFileName(updaterURL);
                    string dir = System.IO.Path.GetDirectoryName(dest);
                    this.updaterDirectory = updaterDirectory;

                    // Check to see if the destination exists. If not, create it
                    System.IO.Directory.CreateDirectory(dir);

                    // Create the URL object, assign the event, and download the file
                    System.Uri url = new System.Uri(updaterURL);

                    wclient.DownloadFileCompleted += downloadUpdateComplete; //delegate
                    wclient.DownloadProgressChanged += downloadProgressChanged;
                   

                    wclient.DownloadFileAsync(url, dest); // download the file
                }
                catch (Exception e)
                {
                    // Some strange error when getting the updater file!
                    throw new UpdateFileNotFoundException(e.Message);
                }
            }

            // Now download the changelog

            if (downloadChangelog)
            {
                // Get the changelog contents and write them to the changelog file
                string changelogContents = getChangelog(this.changelogURL);
                try
                {
                    System.IO.StreamWriter changelogWriter = new System.IO.StreamWriter(
                        changelogDirectory + "/" + System.IO.Path.GetFileName(this.changelogURL));
                    changelogWriter.Write(changelogContents);
                    changelogWriter.Close();
                }
                catch (Exception ex)
                {
                    throw new ChangelogFileCantWriteException(ex.Message);
                }
            }
        }

        /** Download update method (With default changelog download location)
         * This method allows the user to download an update to the specified location
         * and afterwards download the changelog without worrying about specifying where
         * the changelog belongs.
         * 
         * Parameters: updaterDirectory  - The directory to download the update to
         *             downloadChangelog - A boolean indicating whether or not to download
         *                                 the changelog after the update.
         * Return: None.
         */
        public void downloadUpdate(string updaterDirectory, bool downloadChangelog)
        {
            downloadUpdate(updaterDirectory, downloadChangelog,
                System.IO.Directory.GetCurrentDirectory());
        }

        /** Download update method (with default changelog and updater download location)
         * This method allows the user to download an update and the changelog
         * afterwards without worrying about their locations. 
         * 
         * Parameters: downloadChangelog - A boolean indicating whether or not to download
         *                                 the changelog after the update.
         * Return: None.
         */
        public void downloadUpdate(bool downloadChangelog)
        {
            downloadUpdate(System.IO.Directory.GetCurrentDirectory(), downloadChangelog,
                System.IO.Directory.GetCurrentDirectory());
        }

		/** Download Update Method (with custom location specs)
		 * This method downloads the update file asynchroniously to a customly
		 * defined directory. The file retains its name from the updaterURL.
		 *
		 * Parameters: destinationDirectory - The directory to download to
		 * Return:     None
		 */
		public void downloadUpdate(string destinationDirectory)
		{
            downloadUpdate(destinationDirectory, false);
		}

		/** Download Update Method (without custom location specs)
		 * This method downloads the update file asynchroniously to the same
		 * directory that the program is in. The file retains its name from the URL.
		 *
		 * Parameters: None
		 * Return:     None
		 */
		public void downloadUpdate()
		{
            downloadUpdate(System.IO.Directory.GetCurrentDirectory(), false);
		}

		/** Run Update File (with arguments in a custom directory)
		 * This method runs the update file that was downloaded to a custom directory
		 * and runs it with the provided argument string.
		 *
		 * Parameters: directory - The directory where the updater is located
		 *             args      - The argument string to send to the updater
		 * Return:     None
		 */
		public void runUpdate(string directory, string args)
		{
			// First, make sure that the update exists
			string dest = directory + "/" + System.IO.Path.GetFileName(updaterURL);
			if (!(System.IO.File.Exists(dest)))
				throw new UpdateFileNotFoundException("Cannot Run Update: File not Found");

			// Set the custom update directory var to the custom dir
			this.updaterDirectory = directory;

			// Create the file running object
			System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo();

			//Enter arguments
			start.Arguments = args;
			start.FileName = dest;

			// Run the file
			System.Diagnostics.Process.Start(start);
		}

		/** Run Update File (with no arguments in a custom directory)
		 * This method runs the update file that was downloaded to a custom directory
		 * but does not run it with any arguments
		 *
		 * Parameters: directory - The directory where the updater is located
		 * Return:     None
		 */
		public void runUpdate(string directory)
		{
			// First, make sure that the update exists
			string dest = directory + "/" + System.IO.Path.GetFileName(updaterURL);
			if (!(System.IO.File.Exists(dest)))
				throw new UpdateFileNotFoundException("Cannot Run Update: File not Found");

			// Set the custom update directory variable to the custom directry
			this.updaterDirectory = directory;

			// Setup the file runner process thingy
			System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo();

			// Enter arguments (no args)
			start.Arguments = "";
			start.FileName = dest;

			// Run the file
			System.Diagnostics.Process.Start(start);
		}

		/** Run the Update File (without arguments nor custom directory)
		 * This method runs the update file that was downloaded to the same directory
		 * without any specific arguments.
		 *
		 * Parameters: None
		 * Return:     None
		 */
		public void runUpdate()
		{
			// First, make sure that the update exists
			string dest = System.IO.Directory.GetCurrentDirectory() + "/" +
				System.IO.Path.GetFileName(updaterURL);
			if (!(System.IO.File.Exists(dest)))
				throw new UpdateFileNotFoundException("Cannot Run Update: File not Found");

			// Now we create the file runner
			System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo();

			// Enter arguments
			start.Arguments = ""; // (no args)
			start.FileName = dest;

			// Run the update
			System.Diagnostics.Process.Start(start);
		}


		/** Close Method
		 * This method attempts to close the updater by deleting all of its 
		 * resources. Thus, without the close method being called, there could be
		 * file-overwrite issues.
		 *
		 * Parameters: None
		 * Return:     None
		 */
		public void close()
		{
			// This method attempts to close the updater by deleting the update

			// First, let's delete the version file
			if (System.IO.File.Exists(System.IO.Path.GetFileName(versionURL)))
				System.IO.File.Delete(System.IO.Path.GetFileName(versionURL));

			// Now, let's delete the update itself
			if (this.updaterDirectory != null)
				System.IO.File.Delete(this.updaterDirectory + "/" + System.IO.Path.GetFileName(updaterURL));
			else
				System.IO.File.Delete(System.IO.Path.GetFileName(updaterURL));
		}

		/** Refresh Method
		 * This method serves the purpose of being identical to the close method,
		 * but it is meant to be called when the updater is created just in case
		 * the close method could not be called.
		 *
		 * Parameters: None
		 * Return:     None
		 **/
		public void refresh()
		{
			// First, let's delete the version file
			if (System.IO.File.Exists(System.IO.Path.GetFileName(versionURL)))
				System.IO.File.Delete(System.IO.Path.GetFileName(versionURL));

			// Now, let's delete the update itself
			System.IO.File.Delete(System.IO.Path.GetFileName(updaterURL));
		}

		/** Refresh Method
		 * This method serves the purpose of being identical to the close method,
		 * but it is meant to be called when the updater is created just in case
		 * the close method could not be called. This version may be called when the 
		 * update has its own download directory.
		 *
		 * Parameters: updateDirectory - The directory the updater was downloaded to
		 * Return:     None
		 **/
		public void refresh(string updateDirectory)
		{
			// First, let's delete the version file
			if (System.IO.File.Exists(System.IO.Path.GetFileName(versionURL)))
				System.IO.File.Delete(System.IO.Path.GetFileName(versionURL));

			// Now, let's delete the update itself
			System.IO.File.Delete(updateDirectory + "/" + System.IO.Path.GetFileName(updaterURL));
			this.updaterDirectory = updateDirectory;
		}

        /** Set Credentials Function
         * This method sets the username and password for all downloads that this
         * updater object uses. It should be called before any other function
         * of the update.net object.
         * 
         * Parameters: username - The username for the downloads
         *             password - The password for the downloads
         * Return:     None.
         */
        public void setCredentials(string username, string password)
        {
            this.downloadUsername = username;
            this.downloadPassword = password;
        }

        /** Clear Credentials Function
         * This method clears the username and password for all downloads that this
         * updater object uses. It should be called before any other function of the
         * update.net object if no password is required.
         * 
         * Return: None.
         */
        public void clearCredentials()
        {
            setCredentials("", "");
        }

		/** Download Progress Changed Handler Method
		 * This method handles the progression of the download by raising the update
		 * object's own custom event. This is not for public consumption.
		 *
		 * Parameters: sender - The object that raised the event
		 *             e      - The arguments to the event
		 * Return:     None
		 */
		private void downloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
		{
			// Switch the event from the webclient to the updater
			EventHandler handler = UpdateDownloadProgressChanged;
			if (handler != null)
				handler(this, e);
		}

		/** Download Complete Handler Method
		 * This method handles the completion of the download by raising the update
		 * object's own custom event. This is not for public consumption.
		 *
		 * Parameters: sender - The object that raised the vent
		 *             e      - The arguments to the event
		 * Return:     None
		 */
		private void downloadUpdateComplete(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
		{
			// Switch the event from the webclient to the updater
			EventHandler handler = UpdateDownloaded;
			if (handler != null)
				handler(this, e);
		}

		/** Get Latest Version Number
		 * This method returns the latest version of the program that was used
		 * to compare the current version. If the isUpdateAvailable() method has
		 * not been called, the latest version cannot be grabbed.
		 *
		 * Parameters: None
		 * Return:     A String containing the latest version
		 **/
		public string getLatestVersion()
		{
			return this.latestVersion;
		}

        /** Get Changelog
         * This method reads the changelog from the server and returns it in
         * a string so that it can be used to display the changelog to the user.
         * This method uses the changelog URL specified in the constructor.
         * 
         * Parameters: None
         * Return:     The contents of the changelog.
         */
        public string getChangelog()
        {
           return getChangelog(this.changelogURL);
        }

		/** Get ChangeLog
		 * This method reads the changelog from the server and returns it in
		 * a string so that it can be used to display the changelog to the 
		 * user.
		 *
		 * Parameters: changelogURL - A String containing the URL of the changelog file
		 * Return:     A string with the contents of the changelog.
		 **/
		public string getChangelog(string changelogURL)
		{
			if (!(System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable()))
			{
				// The network is not available, throw an exception
				throw new NetworkNotFoundException("Initial network not found");
			}
			else
			{
				try
				{
					System.Net.WebClient wc = new System.Net.WebClient();
                    wc.Credentials = new System.Net.NetworkCredential(this.downloadUsername,
                        this.downloadPassword);
					return wc.DownloadString(changelogURL);
				}
				catch (Exception e)
				{
					throw new ChangelogFileNotFoundException(e.Message);
				}
			}

		}
	}

	/** Exception Classes To Handle This Object's Exceptions **/

	// Exception to handle when there is no network available
	public class NetworkNotFoundException: Exception
	{
		public NetworkNotFoundException()
		{
		}

		public NetworkNotFoundException(string message): base(message)
		{
		}

		public NetworkNotFoundException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}

	// Exception to handle when there are problems with the version file
	public class VersionFileNotFoundException: Exception
	{
		public VersionFileNotFoundException()
		{
		}

		public VersionFileNotFoundException(string message): base(message)
		{
		}

		public VersionFileNotFoundException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}

	// Exception to handle when there are problems with the update file
	public class UpdateFileNotFoundException: Exception
	{
		public UpdateFileNotFoundException()
		{
		}

		public UpdateFileNotFoundException(string message): base(message)
		{
		}

		public UpdateFileNotFoundException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}

	// Exception to handle when there are problems with the changelog file
	public class ChangelogFileNotFoundException : Exception
	{
		public ChangelogFileNotFoundException()
		{
		}

		public ChangelogFileNotFoundException(string message)
			: base(message)
		{
		}

		public ChangelogFileNotFoundException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}

    // Exception to handle when there are problems writing the changlog file
    public class ChangelogFileCantWriteException : Exception
    {
        public ChangelogFileCantWriteException()
        {
        }

        public ChangelogFileCantWriteException(string message)
            : base(message)
        { 
        }

        public ChangelogFileCantWriteException(String message, Exception inner)
            : base(message, inner)
        {
        }
    }

}

