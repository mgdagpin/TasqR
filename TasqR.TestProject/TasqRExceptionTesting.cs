using System;
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

                var tasqR = new TasqRObject(handlerResolver);
                var cmd = new CommandWithKey();

                tasqR.Run(cmd);

            }
            catch (Exception ex)
            {
                Assert.AreEqual("Type CommandWithKey not registered", ex.Message);

                throw;
            }
        }
    }
}
