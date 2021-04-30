using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nucleus.Service.Data;
using Nucleus.Service.SqlScriptObjects.Common_SQL;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Utils;
using Unity.Extension;
using Unity;
using Unity.Injection;

namespace Nucleus.Service
{
    public class DependencyOfDependencyExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container.RegisterType<INewNavigator, NewNavigator>(new InjectionConstructor(new ResolvedParameter<ISiteDriver>()));
            Container.RegisterType<ISiteDriver, NewSiteDriver>();
            Container.RegisterType<IJavaScriptExecutors, JavaScriptExecutorsNew>();
            Container.RegisterType<IBrowserOptions, NewBrowserOptions>();
            Container.RegisterType<IOracleStatementExecutor, NewOracleStatementexecutor>(new InjectionConstructor(new ResolvedParameter<IDatabaseConnection>()));
            Container.RegisterType<IDatabaseConnection, NewDatabaseConnection>();
           
        }
    }
}
