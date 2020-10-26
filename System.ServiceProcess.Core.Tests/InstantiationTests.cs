using System;
using Xunit;

namespace System.ServiceProcess.Core.Tests
{
    public class InstantiationTests
    {
        [Fact]
        public void Can_Instantiate_ServiceInstaller()
        {
            // This gave: System.TypeLoadException : Could not load type 'System.ServiceProcess.ServiceInstaller' from assembly 'System.ServiceProcess, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'.
            //
            // This was before it was renamed with a .Core suffix. The .NET Core pack comes with an empty System.ServiceProcess
            // for some reason that conflicts, so we must rename to .Core.
            new ServiceInstaller();
        }
    }
}
