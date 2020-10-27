using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace System.ServiceProcess.Core.Tests
{
    public class ResourceTests
    {
        [Fact]
        public void Retrieves_Resource_String_ArgsCantBeNull()
        {
            string resString = Res.GetString("ArgsCantBeNull");

            Assert.Equal("Arguments within the 'args' array passed to Start cannot be null. ", resString);
        }
    }
}