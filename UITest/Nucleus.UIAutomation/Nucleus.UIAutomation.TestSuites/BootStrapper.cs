using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nucleus.Service;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.PageServices.HelpCenter;
using Nucleus.Service.Support.Environment;

namespace Nucleus.UIAutomation.TestSuites
{
    public static class BootStrapper
    {
        public static void Init()
        {
            DependencyInjector.Register<IStartNucleus, NewStartNucleus>();
            DependencyInjector.Register<IDataHelper, NewDataHelper>();
            DependencyInjector.Register<IEnvironmentManager, NewEnvironmentManager>();
            DependencyInjector.AddExtension<DependencyOfDependencyExtension>();
        }
    }
}
