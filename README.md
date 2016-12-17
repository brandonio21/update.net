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
* Version numbers must be integers
* The updater must be stored online and donwloadable

## Some Technical Details ##
* The Version File is downloaded and read on the operating thread
* The updater file is downloaded asynchroniously on another thread
* The updater file is run on the operating thread

User Guide
----------

### Basic Usage ###
```c#
// Create a new update object
var u = new HTTPPlaintextUpdater("urlToVersionFile", "urlToUpdaterFile", "dir");

// Check if an update is available
if (u.IsUpdateAvailable(currentVersion))
{
	downloadUpdate();
	runUpdate();
}
else
{
	// No update available
}
```

### Catching Errors ###
```c#
var u = new HTTPPlaintextUpdater("urlToVersionFile", "urlToUpdaterFile", "dir");
try
{
	if (u.IsUpdateAvailable(currentVersion))
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
