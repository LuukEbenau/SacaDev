# SacaDev
This project contains the sourcecode of all SacaDev packages.

I've originally intended to create this project for usage between own projects for commonly used baseclasses.
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
This class has quite room for some refactoring, like for example removing external depencencies.
