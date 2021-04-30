using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Login;
using Nucleus.Service.Support.Environment;

namespace Nucleus.Service
{
    public interface IStartNucleus
    {
        LoginPage StartNucleusApplication(IEnvironmentManager environmentManager,IDataHelper dataHelper);
        void Dispose();

        void CloseDatabaseConnection();

    }
}
