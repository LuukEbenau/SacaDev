# SacaDev
This project contains the sourcecode of all SacaDev packages.

I've originally intended to create this project for usage between own projects for commonly used baseclasses. But i decided it is maybe for the best to make the project open source, since maybe I can help someone with it :)

The project consists of four submodules:
*  Diagnostics
This project contains an easy to use logger class to log messages/errors to a file
*  Configuration
This project contains an easy to use generic baseclass for managing configuration files.
It supports encryption of the file using sha256.
*  Util
Containers helpermethods, used by the other projects in the SacaDev environment.
for example convenient extensions for Encryption and Decryption of a string, and creating a char stream of a string.
*  Api
Contains a baseclass for communication with api's.
This class has quite room for some refactoring, like for example removing external depencencies. And quite a bit more.

You are free to Contribute, Fork or use any of the sourcecode, or just use it as inspriration. As long as you follow the licence ( give credit to this github)

I would appreciate any feedback/feature/bug request issues!

The packages at the moment be found as Prerelease in the nuget store. 
*  https://www.nuget.org/packages/SacaDev.Api/
*  https://www.nuget.org/packages/SacaDev.Configuration/
*  https://www.nuget.org/packages/SacaDev.Diagnostics/
*  https://www.nuget.org/packages/SacaDev.Util/
