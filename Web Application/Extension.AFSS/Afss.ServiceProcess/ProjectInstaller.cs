using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;

namespace FuelsManager.Afss.ServiceProcess
{
    using System.ServiceProcess;

    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            Installers.Add(
                new ServiceInstaller()
                    {
                        StartType = ServiceStartMode.Automatic,
                        ServiceName = AfssServiceProcess.WindowsServiceName,
                        Description = AfssServiceProcess.WindowsServiceDescription
                    });

            Installers.Add(new ServiceProcessInstaller { Account = ServiceAccount.LocalSystem });
        }
    }
}
