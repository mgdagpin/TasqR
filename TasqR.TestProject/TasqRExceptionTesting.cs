using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TasqR.Common;
using TasqR.TestProject.Test4;

namespace TasqR.TestProject
{
    [TestClass]
    public class TasqRExceptionTesting
    {
        [TestMethod]
        [ExpectedException(typeof(TasqException))]
        public void CanThrowExceptionIfNoHandlerRegistered()
        {
            try
            {
                var handlerResolver = new TasqHandlerResolver();

                var tasqR = new TasqR(handlerResolver);
                var cmd = new CommandWithKey();

                _ = tasqR.Run(cmd).ToList();

            }
            catch (Exception ex)
            {
                Assert.AreEqual("Type CommandWithKey not registered", ex.Message);

                throw;
            }
        }
    }
}
