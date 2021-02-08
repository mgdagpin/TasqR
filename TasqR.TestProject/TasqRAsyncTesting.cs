using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TasqR.Common;
using TasqR.TestProject.Test3;
using TasqR.TestProject.Test4;

namespace TasqR.TestProject
{
    [TestClass]
    public class TasqRAsyncTesting
    {
        [TestMethod]
        public async Task CanRunWithKey()
        {
            var handlerResolver = new TasqHandlerResolver();

            handlerResolver.Register<CommandWithKeyAsyncHandler>();

            var tasqR = new TasqRObject(handlerResolver);
            var cmd = new CommandWithKeyAsync();

            await tasqR.RunAsync(cmd);

            Assert.IsTrue(cmd.AllAreCorrect);
        }

        [TestMethod]
        public async Task CanRunWithKeyBaseType()
        {
            var handlerResolver = new TasqHandlerResolver();

            handlerResolver.Register<CommandWithKeyAsyncHandler>();

            var tasqR = new TasqRObject(handlerResolver);
            var instance = (ITasq<int, bool>)Activator.CreateInstance(typeof(CommandWithKeyAsync));

            await tasqR.RunAsync(instance);

            Assert.IsTrue(((CommandWithKeyAsync)instance).AllAreCorrect);
        }

        [TestMethod]
        [ExpectedException(typeof(TasqException))]
        public async Task ExceptionWillThrownIfInstanceWith3GenericParamInDefaultRun()
        {
            try
            {
                var handlerResolver = new TasqHandlerResolver();

                handlerResolver.Register<CommandWithKeyHandler>();

                var tasqR = new TasqRObject(handlerResolver);
                var instance = (ITasq)Activator.CreateInstance(typeof(CommandWithKey));

                await tasqR.RunAsync(instance);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Cast your Tasq with ITasq<Int32,Boolean>", ex.Message);

                throw;
            }
        }

        [TestMethod]
        public async Task CanRunWithoutReturn()
        {
            var testModel = new Test3Model { StartNumber = 10 };
            var handlerResolver = new TasqHandlerResolver();

            handlerResolver.Register<CommandWithAsyncWithoutReturnHandler>();

            var tasqR = new TasqRObject(handlerResolver);
            var cmd = new CommandWithAsyncWithoutReturn(testModel);

            await tasqR.RunAsync(cmd);

            Assert.AreEqual(11, testModel.StartNumber);
        }


    }
}
