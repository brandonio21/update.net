update.net
=========

### A Quick Background ###

Something I noticed when I worked on software for clients was that I was always
writing updaters. I always was writing an updater to match the needs of the 
project I was working on. A lot of times, this just involved updating the UI.
The backend was always the same. Thus, this project serves to house the backend
for all of my updating needs. Nothing special.

### Why .NET? ###

I started writing my clients' software in VB.NET because it was so easy with the
form designer and all. Because of the results (clients really like pretty 
forms), I have stuck with using Visual Studio to make the applications. There 
have been no complaints so far. Although this library isn't written in VB.NET, 
it is still compatible with all .NET projects. 

## Project Setup? ##

To use update.net, you must meet the following guidelines:  

* The latest version number must be stored in a file online
* The updater must be stored online and donwloadable

## Some Technical Details ##
* The Version File is downloaded and read on the operating thread
* The updater file is downloaded asynchroniously on another thread
* The updater file is run on the operating thread


## TO-DO LIST ##
0) Add implementation details to the README file  
1) I developed this on Linux, thus, extensive Windows testing is needed  
2) Extensive testing is seriously needed  
3) Add in the ability to specify username/password for downloading files  
4) Add in the ability to download a changelog  
5) Add in a custom app that extracts .zip files so user doesn't have to make
   their own SFX archive.
6) Add in a method to get the latest version

User Guide
----------

### Basic Usage ###
```c#
// Create a new update object
updater u = new updater("urlToVersionFile", "urlToUpdaterFile");

// Check if an update is available
if (u.isUpdateAvailable(currentVersion))
{
	downloadUpdate();
	runUpdate();
}
else
{
	// No update available
}

// Close the updater
u.close();
```

### Catching Errors ###
```c#
updater u = new updater("urlToVersionFile", "urlToUpdaterFile");
try
{
	if (u.isUpdateAvailable(currentVersion))
	{
		downloadUpdate();
		runUpdate();
	}
}
catch (NetworkNotFoundException nnfe)
{
	// No network!
}
catch (VersionFileNotFoundException vfnfe)
{
	// Error with version file!
}
catch (UpdateFileNotFoundException ufnfe)
{
	// Error with updater file!
}
catch (Exception e)
{
	// Other error
}
finally
{
	// Close the updater
	u.close();
}
```

### A list of resources ###
Since I don't know how to generate official C#doc, here's some useful stuff:  
* `event EventHandler UpdateDownloaded`
* `event EventHandler UpdateDownladProgressChanged`
* `public bool isUpdateAvailable(string currentVersion)`
* `public void downloadUpdate(string destDir)`
* `public void downloadUpdate()`
* `public void runUpdate(string dir, string args)`
* `public void runUpdate(string directory)`
* `public void runUpdate()`
* `public void close()`




## Sample ##
To see a sample implementation, check out [this gist](https://gist.github.com/brandonio21/f6af53fa7b985b7d03f4)
