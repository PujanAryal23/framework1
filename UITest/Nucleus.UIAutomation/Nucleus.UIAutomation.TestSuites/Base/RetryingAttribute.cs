using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;

namespace Nucleus.UIAutomation.TestSuites.Base
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RetryingAttribute : Attribute, IWrapSetUpTearDown
    {
        public int Times { get; set; } 

        public TestCommand Wrap(TestCommand command)
        {
            return new RetryingCommand(command, Times);
        }
    }
}
