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
