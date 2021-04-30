using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;

namespace Nucleus.UIAutomation.TestSuites.Base
{
    public class RetryingCommand : DelegatingTestCommand
    {
        private readonly int _times;
        private int retriesLeft;

        public RetryingCommand(TestCommand innerCommand, int times)
            : base(innerCommand)
        {
            _times = times;
        }

        public override TestResult Execute(TestExecutionContext context)
        {
            retriesLeft = _times;
            //context.UpdateContextFromEnvironment();
            context.StopOnError = false;

            RunTest(context);
            
            while (TestFailed(context) && retriesLeft > 0)
            {
                ClearTestResult(context);
                retriesLeft--;
                RunTest(context);
            }

            var performedRetries = _times - retriesLeft;

            if (performedRetries > 0)
            {
                context.OutWriter.WriteLine();
                context.OutWriter.WriteLine($"Test retried {performedRetries} time/s");
            }


            return context.CurrentResult;
        }

        private void RunTest(TestExecutionContext context)
        {
            try
            {
                context.CurrentResult = innerCommand.Execute(context);
            }
            catch (Exception e)
            {
                if (retriesLeft == 0)
                {
                    var performedRetries = _times - retriesLeft;

                    context.OutWriter.WriteLine();
                    context.OutWriter.WriteLine($"Test retried {performedRetries} time/s");
                    throw new Exception(e.InnerException.Message);
                }
            }
        }

        private static void ClearTestResult(TestExecutionContext context)
        {
            context.CurrentResult = context.CurrentTest.MakeTestResult();
        }

        private static bool TestFailed(TestExecutionContext context)
        {
            return UnsuccessfulResultStates.Contains(context.CurrentResult.ResultState);
        }

        private static ResultState[] UnsuccessfulResultStates => new[]
        {
            ResultState.Failure,
            ResultState.Error,
            ResultState.Inconclusive

        };
    }
}
