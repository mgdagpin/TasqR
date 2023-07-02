using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TasqR.Common;
using TasqR.TestProject.Microsoft.DependencyInjection.Test2;
using TasqR.TestProject.Microsoft.DependencyInjection.Test4;

namespace TasqR.TestProject.Microsoft.DependencyInjection
{
    [TestClass]
    public class ResolverTesting
    {
        [TestMethod]
        public void CanResolveMultiLevelConcreteHandler()
        {
            var assemblyCol = new TasqAssemblyCollection();
            var handlerResolver = new TasqHandlerResolver(assemblyCol);

            var handlers = assemblyCol.GetAllHandlers(Assembly.GetExecutingAssembly());

            Assert.IsTrue(handlers.Any(t => t == typeof(TestCommandWithMultipleHandlerHandler2)));
            Assert.IsFalse(handlers.Any(t => t == typeof(Base1Handler)));
        }
    }
}
