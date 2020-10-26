//------------------------------------------------------------------------------
// <copyright file="ServiceInstaller.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>                                                                
//------------------------------------------------------------------------------

namespace System.ServiceProcess
{
    using System.ComponentModel;
    using System.Diagnostics;
    using System;
    using System.Collections;
    using System.Configuration.Install;
    using System.IO;
    using System.Threading;
    using System.Text;
    using System.Runtime.InteropServices;
    using System.Globalization;

    /// <summary>Installs a class that extends <see cref="T:System.ServiceProcess.ServiceBase" /> to implement a service. This class is called by the install utility when installing a service application.</summary>
    public class ServiceInstaller : ComponentInstaller
    {
        private const string NetworkServiceName = "NT AUTHORITY\\NetworkService";
        private const string LocalServiceName = "NT AUTHORITY\\LocalService";

        private EventLogInstaller eventLogInstaller;
        private string serviceName = "";
        private string displayName = "";
        private string description = "";
        private string[] servicesDependedOn = new string[0];
        private ServiceStartMode startType = ServiceStartMode.Manual;
        private bool delayedStartMode = false;
        private static bool environmentChecked = false;
        private static bool isWin9x = false;


        /// <summary>Initializes a new instance of the <see cref="T:System.ServiceProcess.ServiceInstaller" /> class.</summary>
        public ServiceInstaller() : base()
        {

            // Create an EventLogInstaller and add it to our Installers collection to take
            // care of the service's EventLog property.
            eventLogInstaller = new EventLogInstaller();
            eventLogInstaller.Log = "Application";
            // we change these two later when our own properties are set.
            eventLogInstaller.Source = "";
            eventLogInstaller.UninstallAction = UninstallAction.Remove;

            Installers.Add(eventLogInstaller);
        }

        /// <summary>Indicates the friendly name that identifies the service to the user.</summary>
        /// <returns>The name associated with the service, used frequently for interactive tools.</returns>
        [
        DefaultValue(""),
        ServiceProcessDescription(Res.ServiceInstallerDisplayName)
        ]
        public string DisplayName
        {
            get
            {
                return displayName;
            }
            set
            {
                if (value == null)
                    value = "";
                displayName = value;
            }
        }

        /// <summary>Gets or sets the description for the service.</summary>
        /// <returns>The description of the service. The default is an empty string ("").</returns>
        [DefaultValue(""),
        ComVisible(false),
        ServiceProcessDescription(Res.ServiceInstallerDescription)
        ]
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                if (value == null)
                    value = "";
                description = value;
            }
        }

        /// <summary>Indicates the services that must be running for this service to run.</summary>
        /// <returns>An array of services that must be running before the service associated with this installer can run.</returns>
        [
        ServiceProcessDescription(Res.ServiceInstallerServicesDependedOn)
        ]
        public string[] ServicesDependedOn
        {
            get
            {
                return servicesDependedOn;
            }
            set
            {
                if (value == null)
                    value = new string[0];
                servicesDependedOn = value;
            }
        }

        /// <summary>Indicates the name used by the system to identify this service. This property must be identical to the <see cref="P:System.ServiceProcess.ServiceBase.ServiceName" /> of the service you want to install.</summary>
        /// <returns>The name of the service to be installed. This value must be set before the install utility attempts to install the service.</returns>
        /// <exception cref="T:System.ArgumentException">The <see cref="P:System.ServiceProcess.ServiceInstaller.ServiceName" /> property is invalid.</exception>
        [
        DefaultValue(""),
        ServiceProcessDescription(Res.ServiceInstallerServiceName)
        ]
        public string ServiceName
        {
            get
            {
                return serviceName;
            }
            set
            {
                if (value == null)
                    value = "";

                if (!ServiceControllerValidServiceName(value))
                    throw new ArgumentException(Res.GetString(Res.ServiceName, value, ServiceBase.MaxNameLength.ToString(CultureInfo.CurrentCulture)));

                serviceName = value;
                eventLogInstaller.Source = value;
            }
        }

        // This is in ServiceController as ServiceController.ValidServiceName(), but it's internal
        internal static bool ServiceControllerValidServiceName(string serviceName)
        {
            if (serviceName == null)
            {
                return false;
            }
            if (serviceName.Length > 80 || serviceName.Length == 0)
            {
                return false;
            }
            foreach (char c in serviceName.ToCharArray())
            {
                if (c == '\\' || c == '/')
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>Indicates how and when this service is started.</summary>
		/// <returns>A <see cref="T:System.ServiceProcess.ServiceStartMode" /> that represents the way the service is started. The default is <see langword="Manual" />, which specifies that the service will not automatically start after reboot.</returns>
		/// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">The start mode is not a value of the <see cref="T:System.ServiceProcess.ServiceStartMode" /> enumeration.</exception>
        [
        DefaultValue(ServiceStartMode.Manual),
        ServiceProcessDescription(Res.ServiceInstallerStartType)
        ]
        public ServiceStartMode StartType
        {
            get
            {
                return startType;
            }
            set
            {
                if (!Enum.IsDefined(typeof(ServiceStartMode), value))
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(ServiceStartMode));

                switch (value)
                {
                    case ServiceStartMode.Boot:
                    case ServiceStartMode.System:
                        //intentional fall through
                        // These two values are reserved for device driver services.
                        throw new ArgumentException(Res.GetString(Res.ServiceStartType, value));

                    default:
                        startType = value;
                        break;
                }
            }
        }

        /// <summary>Gets or sets a value that indicates whether the service should be delayed from starting until other automatically started services are running.</summary>
		/// <returns>
		///   <see langword="true" /> to delay automatic start of the service; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
        [
        DefaultValue(false),
        ServiceProcessDescription(Res.ServiceInstallerDelayedAutoStart)
        ]
        public bool DelayedAutoStart
        {
            get
            {
                return delayedStartMode;
            }
            set
            {
                delayedStartMode = value;
            }
        }

        internal static void CheckEnvironment()
        {
            if (environmentChecked)
            {
                if (isWin9x)
                    throw new PlatformNotSupportedException(Res.GetString(Res.CantControlOnWin9x));

                return;
            }
            else
            {
                isWin9x = Environment.OSVersion.Platform != PlatformID.Win32NT;
                environmentChecked = true;

                if (isWin9x)
                    throw new PlatformNotSupportedException(Res.GetString(Res.CantInstallOnWin9x));
            }
        }


        /// <summary>Copies properties from an instance of <see cref="T:System.ServiceProcess.ServiceBase" /> to this installer.</summary>
        /// <param name="component">The <see cref="T:System.ComponentModel.IComponent" /> from which to copy.</param>
        /// <exception cref="T:System.ArgumentException">The component you are associating with this installer does not inherit from <see cref="T:System.ServiceProcess.ServiceBase" />.</exception>
        public override void CopyFromComponent(IComponent component)
        {
            if (!(component is ServiceBase))
                throw new ArgumentException(Res.GetString(Res.NotAService));

            ServiceBase service = (ServiceBase)component;

            ServiceName = service.ServiceName;
        }

        /// <summary>Installs the service by writing service application information to the registry. This method is meant to be used by installation tools, which process the appropriate methods automatically.</summary>
		/// <param name="stateSaver">An <see cref="T:System.Collections.IDictionary" /> that contains the context information associated with the installation.</param>
		/// <exception cref="T:System.InvalidOperationException">The installation does not contain a <see cref="T:System.ServiceProcess.ServiceProcessInstaller" /> for the executable.  
		///  -or-  
		///  The file name for the assembly is <see langword="null" /> or an empty string.  
		///  -or-  
		///  The service name is invalid.  
		///  -or-  
		///  The Service Control Manager could not be opened.</exception>
		/// <exception cref="T:System.ArgumentException">The display name for the service is more than 255 characters in length.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">The system could not generate a handle to the service.  
		///  -or-  
		///  A service with that name is already installed.</exception>
        public override void Install(IDictionary stateSaver)
        {
            Context.LogMessage(Res.GetString(Res.InstallingService, ServiceName));
            try
            {
                CheckEnvironment();
                string userName = null;
                string password = null;
                // find the ServiceProcessInstaller for our process. It's either the
                // parent or one of our peers in the parent's Installers collection.
                ServiceProcessInstaller processInstaller = null;
                if (Parent is ServiceProcessInstaller)
                {
                    processInstaller = (ServiceProcessInstaller)Parent;
                }
                else
                {
                    for (int i = 0; i < Parent.Installers.Count; i++)
                    {
                        if (Parent.Installers[i] is ServiceProcessInstaller)
                        {
                            processInstaller = (ServiceProcessInstaller)Parent.Installers[i];
                            break;
                        }
                    }
                }

                if (processInstaller == null)
                    throw new InvalidOperationException(Res.GetString(Res.NoInstaller));

                switch (processInstaller.Account)
                {
                    case ServiceAccount.LocalService:
                        userName = LocalServiceName;
                        break;
                    case ServiceAccount.NetworkService:
                        userName = NetworkServiceName;
                        break;
                    case ServiceAccount.User:
                        userName = processInstaller.Username;
                        password = processInstaller.Password;
                        break;
                }

                // check all our parameters

                string moduleFileName = Context.Parameters["assemblypath"];
                if (String.IsNullOrEmpty(moduleFileName))
                    throw new InvalidOperationException(Res.GetString(Res.FileName));

                // Put quotas around module file name. Otherwise a service might fail to start if there is space in the path.
                // Note: Though CreateService accepts a binaryPath allowing
                // arguments for automatic services, in /assemblypath=foo,
                // foo is simply the path to the executable.
                // Therefore, it is best to quote if there are no quotes,
                // and best to not quote if there are quotes.
                if (moduleFileName.IndexOf('\"') == -1)
                    moduleFileName = "\"" + moduleFileName + "\"";

                //Check service name
                if (!ValidateServiceName(ServiceName))
                {
                    //Event Log cannot be used here, since the service doesn't exist yet.
                    throw new InvalidOperationException(Res.GetString(Res.ServiceName, ServiceName, ServiceBase.MaxNameLength.ToString(CultureInfo.CurrentCulture)));
                }

                // Check DisplayName length. 
                if (DisplayName.Length > 255)
                {
                    // MSDN suggests that 256 is the max length, but in
                    // fact anything over 255 causes problems.  
                    throw new ArgumentException(Res.GetString(Res.DisplayNameTooLong, DisplayName));
                }

                //Build servicesDependedOn string
                string servicesDependedOn = null;
                if (ServicesDependedOn.Length > 0)
                {
                    StringBuilder buff = new StringBuilder();
                    for (int i = 0; i < ServicesDependedOn.Length; ++i)
                    {
                        // we have to build a list of the services' short names. But the user
                        // might have used long names in the ServicesDependedOn property. Try
                        // to use ServiceController's logic to get the short name.
                        string tempServiceName = ServicesDependedOn[i];
                        try
                        {
                            ServiceController svc = new ServiceController(tempServiceName, ".");
                            tempServiceName = svc.ServiceName;
                        }
                        catch
                        {
                        }
                        //The servicesDependedOn need to be separated by a null
                        buff.Append(tempServiceName);
                        buff.Append('\0');
                    }
                    // an extra null at the end indicates end of list.
                    buff.Append('\0');

                    servicesDependedOn = buff.ToString();
                }

                // Open the service manager
                IntPtr serviceManagerHandle = SafeNativeMethods.OpenSCManager(null, null, NativeMethods.SC_MANAGER_ALL);
                IntPtr serviceHandle = IntPtr.Zero;
                if (serviceManagerHandle == IntPtr.Zero)
                    throw new InvalidOperationException(Res.GetString(Res.OpenSC, "."), new Win32Exception());

                int serviceType = NativeMethods.SERVICE_TYPE_WIN32_OWN_PROCESS;
                // count the number of UserNTServiceInstallers. More than one means we set the SHARE_PROCESS flag.
                int serviceInstallerCount = 0;
                for (int i = 0; i < Parent.Installers.Count; i++)
                {
                    if (Parent.Installers[i] is ServiceInstaller)
                    {
                        serviceInstallerCount++;
                        if (serviceInstallerCount > 1)
                            break;
                    }
                }
                if (serviceInstallerCount > 1)
                {
                    serviceType = NativeMethods.SERVICE_TYPE_WIN32_SHARE_PROCESS;
                }

                try
                {
                    // Install the service
                    serviceHandle = NativeMethods.CreateService(serviceManagerHandle, ServiceName,
                        DisplayName, NativeMethods.ACCESS_TYPE_ALL, serviceType,
                        (int)StartType, NativeMethods.ERROR_CONTROL_NORMAL,
                        moduleFileName, null, IntPtr.Zero, servicesDependedOn, userName, password);

                    if (serviceHandle == IntPtr.Zero)
                        throw new Win32Exception();

                    // A local variable in an unsafe method is already fixed -- so we don't need a "fixed { }" blocks to protect 
                    // across the p/invoke calls below.

                    if (Description.Length != 0)
                    {
                        NativeMethods.SERVICE_DESCRIPTION serviceDesc = new NativeMethods.SERVICE_DESCRIPTION();
                        serviceDesc.description = Marshal.StringToHGlobalUni(Description);
                        bool success = NativeMethods.ChangeServiceConfig2(serviceHandle, NativeMethods.SERVICE_CONFIG_DESCRIPTION, ref serviceDesc);
                        Marshal.FreeHGlobal(serviceDesc.description);
                        if (!success)
                            throw new Win32Exception();
                    }

                    if (Environment.OSVersion.Version.Major > 5)
                    {
                        if (StartType == ServiceStartMode.Automatic)
                        {
                            NativeMethods.SERVICE_DELAYED_AUTOSTART_INFO serviceDelayedInfo = new NativeMethods.SERVICE_DELAYED_AUTOSTART_INFO();
                            serviceDelayedInfo.fDelayedAutostart = DelayedAutoStart;
                            bool success = NativeMethods.ChangeServiceConfig2(serviceHandle, NativeMethods.SERVICE_CONFIG_DELAYED_AUTO_START_INFO, ref serviceDelayedInfo);
                            if (!success)
                                throw new Win32Exception();
                        }
                    }

                    stateSaver["installed"] = true;
                }
                finally
                {
                    if (serviceHandle != IntPtr.Zero)
                        SafeNativeMethods.CloseServiceHandle(serviceHandle);

                    SafeNativeMethods.CloseServiceHandle(serviceManagerHandle);
                }
                Context.LogMessage(Res.GetString(Res.InstallOK, ServiceName));
            }
            finally
            {
                base.Install(stateSaver);
            }
        }

        /// <summary>Indicates whether two installers would install the same service.</summary>
		/// <param name="otherInstaller">A <see cref="T:System.Configuration.Install.ComponentInstaller" /> to which you are comparing the current installer.</param>
		/// <returns>
		///   <see langword="true" /> if calling <see cref="M:System.ServiceProcess.ServiceInstaller.Install(System.Collections.IDictionary)" /> on both of these installers would result in installing the same service; otherwise, <see langword="false" />.</returns>
        public override bool IsEquivalentInstaller(ComponentInstaller otherInstaller)
        {
            ServiceInstaller other = otherInstaller as ServiceInstaller;

            if (other == null)
                return false;

            return other.ServiceName == ServiceName;
        }

        /// <include file='doc\ServiceInstaller.uex' path='docs/doc[@for="ServiceInstaller.RemoveService"]/*' />
        /// <devdoc>
        /// Called by Rollback and Uninstall to remove the service.
        /// </devdoc>
        private void RemoveService()
        {
            //
            // SCUM deletes a service when the Service is stopped and there is no open handle to the Service.
            // Service will be deleted asynchrously, so it takes a while for the deletion to be complete.
            // The recoommended way to delete a Service is:
            // (a)  DeleteService/closehandle, 
            // (b) Stop service & wait until it is stopped & close handle
            // (c)  Wait for 5-10 secs for the async deletion to go through.
            //        
            Context.LogMessage(Res.GetString(Res.ServiceRemoving, ServiceName));
            IntPtr serviceManagerHandle = SafeNativeMethods.OpenSCManager(null, null, NativeMethods.SC_MANAGER_ALL);
            if (serviceManagerHandle == IntPtr.Zero)
                throw new Win32Exception();

            IntPtr serviceHandle = IntPtr.Zero;
            try
            {
                serviceHandle = NativeMethods.OpenService(serviceManagerHandle,
                    ServiceName, NativeMethods.STANDARD_RIGHTS_DELETE);

                if (serviceHandle == IntPtr.Zero)
                    throw new Win32Exception();

                NativeMethods.DeleteService(serviceHandle);
            }
            finally
            {
                if (serviceHandle != IntPtr.Zero)
                    SafeNativeMethods.CloseServiceHandle(serviceHandle);

                SafeNativeMethods.CloseServiceHandle(serviceManagerHandle);
            }
            Context.LogMessage(Res.GetString(Res.ServiceRemoved, ServiceName));

            // Stop the service
            try
            {
                using (ServiceController svc = new ServiceController(ServiceName))
                {
                    if (svc.Status != ServiceControllerStatus.Stopped)
                    {
                        Context.LogMessage(Res.GetString(Res.TryToStop, ServiceName));
                        svc.Stop();
                        int timeout = 10;
                        svc.Refresh();
                        while (svc.Status != ServiceControllerStatus.Stopped && timeout > 0)
                        {
                            Thread.Sleep(1000);
                            svc.Refresh();
                            timeout--;
                        }
                    }
                }
            }
            catch
            {
            }

            Thread.Sleep(5000);
        }

        /// <summary>Rolls back service application information written to the registry by the installation procedure. This method is meant to be used by installation tools, which process the appropriate methods automatically.</summary>
		/// <param name="savedState">An <see cref="T:System.Collections.IDictionary" /> that contains the context information associated with the installation.</param>
        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);

            object o = savedState["installed"];
            if (o == null || (bool)o == false)
                return;

            // remove the service
            RemoveService();

        }

        /// <include file='doc\ServiceInstaller.uex' path='docs/doc[@for="ServiceInstaller.ShouldSerializeServicesDependedOn"]/*' />
        /// <devdoc>
        /// <para> Indicates whether the <see cref='System.ServiceProcess.ServiceInstaller.ServicesDependedOn'/> property should be 
        ///    persisted, which corresponds to whether there are services that this service depends
        ///    on.</para>
        /// </devdoc>
        private bool ShouldSerializeServicesDependedOn()
        {
            if (servicesDependedOn != null && servicesDependedOn.Length > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>Uninstalls the service by removing information about it from the registry.</summary>
		/// <param name="savedState">An <see cref="T:System.Collections.IDictionary" /> that contains the context information associated with the installation.</param>
		/// <exception cref="T:System.ComponentModel.Win32Exception">The Service Control Manager could not be opened.  
		///  -or-  
		///  The system could not get a handle to the service.</exception>
        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);

            RemoveService();
        }

        //Internal routine used to validate service names
        private static bool ValidateServiceName(string name)
        {
            //Name cannot be null, have 0 length or be longer than ServiceBase.MaxNameLength
            if (name == null || name.Length == 0 || name.Length > ServiceBase.MaxNameLength)
                return false;

            char[] chars = name.ToCharArray();
            for (int i = 0; i < chars.Length; ++i)
            {
                //Invalid characters ASCII < 32, ASCII = '/', ASCII = '\'
                if (chars[i] < (char)32 || chars[i] == '/' || chars[i] == '\\')
                    return false;
            }

            return true;
        }
    }

}
