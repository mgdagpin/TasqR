using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TasqR.TestProject.Test3;
using TasqR.TestProject.Test4;

namespace TasqR.TestProject
{
    [TestClass]
    public class TasqRAsyncTesting
    {
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
    }
}
