using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TasqR.TestProject.Test1;
using TasqR.TestProject.Test2;
using TasqR.TestProject.Test3;

namespace TasqR.TestProject
{
    [TestClass]
    public class TasqRTesting
    {
        [TestMethod]
        public void CanRunCommandWithoutReturn()
        {
            var testModel = new TestModel { SampleNumber = 10 };
            var handlerResolver = new TasqHandlerResolver();

            handlerResolver.Register(typeof(SampleCommandWithoutReturn), typeof(SampleCommandWithoutReturnHandler));

            var tasqR = new TasqRObject(handlerResolver);

            tasqR.Run(new SampleCommandWithoutReturn(testModel));

            Assert.AreEqual(11, testModel.SampleNumber);
        }

        [TestMethod]
        public void CanRunCommandWithReturn()
        {
            int startNumber = 8;
            var handlerResolver = new TasqHandlerResolver();

            handlerResolver.Register(typeof(SampleCommandWithReturn), typeof(SampleCommandWithReturnHandler));

            var tasqR = new TasqRObject(handlerResolver);

            int finalNumber = tasqR.Run(new SampleCommandWithReturn(startNumber));

            Assert.AreEqual(9, finalNumber);
        }

        [TestMethod]
        public async Task CanRunAsynchronously()
        {
            var testModel = new Test3Model { StartNumber = 10 };
            var handlerResolver = new TasqHandlerResolver();

            handlerResolver.Register(typeof(CommandWithAsyncWithoutReturn), typeof(CommandWithAsyncWithoutReturnHandler));

            var tasqR = new TasqRObject(handlerResolver);

            await tasqR.RunAsync(new CommandWithAsyncWithoutReturn(testModel));

            Assert.AreEqual(11, testModel.StartNumber);
        }
    }
}
