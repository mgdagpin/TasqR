using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasqR.TestProject.Test5
{
    public class CmdHandlerWithNoParamlessCtor : ITasq
    {
    }

    public class CmdHandlerWithNoParamlessCtorHandler : TasqHandler<CmdHandlerWithNoParamlessCtor>
    {
        public CmdHandlerWithNoParamlessCtorHandler(string test)
        {

        }

        public override void Run(CmdHandlerWithNoParamlessCtor process)
        {
            
        }
    }
}
