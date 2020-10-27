# Core.System.ServiceProcess
[![NuGet](https://img.shields.io/nuget/v/Core.System.ServiceProcess.svg)](https://www.nuget.org/packages/Core.System.ServiceProcess/)

Support System.Configuration.Install for .NET Core / .NET 5+. Used version from Microsoft [Reference Source](https://github.com/microsoft/referencesource/tree/master/System.ServiceProcess) and comments from .NET Framework DLL v4.0.0.0.

## Usage / Documentation
### Fixing the ImagePath in the Registry
Somewhere in your parent installer, you will need to correct the path for the `ServiceInstaller`'s `assemblyPath` (non-case sensitive). With .NET Framework, the entry assembly was always an exe. However, even when running the exe dropped out with a publish, it just bootstraps the DLL and your `ImagePath` gets written to the registry with a `.dll` suffix, which will not work on startup (crashes in `KERNELBASE.dll`).

Use something like this after whatever is setting your `assemblyPath` today:

```csharp
private void CorrectAssemblyPath(InstallContext context)
{
    string possibleDll = context.Parameters["assemblyPath"];

    if (possibleDll == null)
    {
        return;
    }

    if (possibleDll.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
    {
        var exe = possibleDll.Substring(0, possibleDll.Length - 4) + ".exe";
        context.Parameters["assemblyPath"] = exe;
    }
}
```

### Assembly Filename
The assembly had to be renamed to `System.ServiceProcess.Core.dll` because the .NET Core pack comes with an empty (based on analysis with dnSpy) `System.ServiceProcess.dll` which causes the build to fail by saying none of the types are valid (assembly not referenced error). The compiler does not complain about the two DLLs and just silently ignores the new one.

I renamed it, but all the types are still in the original namespace so this should not affect your code.

---

More documentation would be appreciated via PRs, but this library is only for backward compatibility with .NET Framework applications being brought into .NET Core and .NET 5+, so it should not be used for new projects.

The interactive service credential prompt is only available when targeting .NET Core 3.0+ since `System.Windows.Forms` is unavailable in .NET Standard 2.0. This functionality is untested (by me).

---

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
