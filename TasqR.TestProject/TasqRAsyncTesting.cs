using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TasqR.Common;
using TasqR.TestProject.Test2;
using TasqR.TestProject.Test3;
using TasqR.TestProject.Test4;
using TasqR.TestProject.Test6;

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

            var tasqR = new TasqR(handlerResolver);
            var cmd = new CommandWithKeyAsync();

            int initialCount = cmd.Keys.Count;

            bool allAreTrue = true;
            int callCount = 0;

            await foreach (var item in tasqR.RunAsync(cmd))
            {
                callCount++;
                if (!item)
                {
                    allAreTrue = false;
                }
            }

            Assert.AreEqual(initialCount, callCount);
            Assert.IsTrue(allAreTrue);
        }

        [TestMethod]
        public async Task CanRunWithKeyBaseType()
        {
            var handlerResolver = new TasqHandlerResolver();

            handlerResolver.Register<CommandWithKeyAsyncHandler>();

            var tasqR = new TasqR(handlerResolver);
            var instance = (ITasq<int, bool>)Activator.CreateInstance(typeof(CommandWithKeyAsync));

            bool allAreTrue = true;

            await foreach (var item in tasqR.RunAsync(instance))
            {
                if (!item)
                {
                    allAreTrue = false;
                }
            }

            Assert.IsTrue(allAreTrue);
        }

        [TestMethod]
        public async Task CanRunWithoutReturn()
        {
            var testModel = new Test3Model { StartNumber = 10 };
            var handlerResolver = new TasqHandlerResolver();

            handlerResolver.Register<CommandWithAsyncWithoutReturnHandler>();

            var tasqR = new TasqR(handlerResolver);
            var cmd = new CommandWithAsyncWithoutReturn(testModel);

            await tasqR.RunAsync(cmd);

            Assert.AreEqual(11, testModel.StartNumber);
        }

        [TestMethod]
        public async Task CanRunWithReturn()
        {
            var handlerResolver = new TasqHandlerResolver();
            handlerResolver.Register<TestCmdWithReturnForAsyncHandler>();
            var tasqR = new TasqR(handlerResolver);
            var cmd = new TestCmdWithReturnForAsync(2);

            var result = await tasqR.RunAsync(cmd);

            Assert.AreEqual(3, result);

        }

        [TestMethod]
        public async Task CanRunWithReturnForNonAsyncHandler()
        {
            var handlerResolver = new TasqHandlerResolver();
            handlerResolver.Register<SampleCommandWithReturnHandler>();
            var tasqR = new TasqR(handlerResolver);
            var cmd = new SampleCommandWithReturn(2);

            var result = await tasqR.RunAsync(cmd);

            Assert.AreEqual(3, result);
        }
    }
}
