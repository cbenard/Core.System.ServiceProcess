# Core.System.ServiceProcess
[![NuGet](https://img.shields.io/nuget/v/Core.System.ServiceProcess.svg)](https://www.nuget.org/packages/Core.System.ServiceProcess/)

Support System.Configuration.Install for .NET Core / .NET 5+. Used version from Microsoft [Reference Source](https://github.com/microsoft/referencesource/tree/master/System.ServiceProcess) and comments from .NET Framework DLL v4.0.0.0.

### Usage / Documentation
Documentation would be appreciated via PRs, but this library is only for backward compatibility with .NET Framework applications being brought into .NET Core and .NET 5+, so it should not be used for new projects.

The interactive service credential prompt is only available when targeting .NET Core 3.0+ since `System.Windows.Forms` is unavailable in .NET Standard 2.0. This functionality is untested (by me).

### Installation

``` dotnet add package Core.System.ServiceProcess```


### License

This software is distributed under the terms of the MIT License (MIT).

### Authors

Chris Benard / [LinkedIn](https://www.linkedin.com/in/chrisbenard)

### Special Thanks

Thank you to [@flamencist](https://github.com/flamencist) for his encouragement and work on [Core.System.Configuration.Install](https://github.com/flamencist/Core.System.Configuration.Install). This project was patterned after his and relies upon it (it is a package reference).

### Contributing 

Contributions and bugs reports are welcome.