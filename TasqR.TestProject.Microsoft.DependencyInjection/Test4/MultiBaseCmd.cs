using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasqR.TestProject.Microsoft.DependencyInjection.Test4
{
    public class MultiBaseCmd : ITasq
    {
        public MultiBaseCmd(int data)
        {
            Data = data;
        }

        public int Data { get; internal set; }
    }

    public abstract class Base1Handler : TasqHandler<MultiBaseCmd>
    {

    }

    public abstract class Base2Handler : Base1Handler
    {

    }

    public class FinalMultiBaseCmdHandler : Base2Handler
    {
        public override void Run(MultiBaseCmd request)
        {
            request.Data++;
        }
    }
}
