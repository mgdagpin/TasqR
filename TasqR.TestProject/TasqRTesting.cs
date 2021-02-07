using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TasqR.TestProject.Test1;
using TasqR.TestProject.Test2;
using TasqR.TestProject.Test3;
using TasqR.TestProject.Test4;

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

            handlerResolver.Register<SampleCommandWithoutReturnHandler>();

            var tasqR = new TasqRObject(handlerResolver);

            tasqR.Run(new SampleCommandWithoutReturn(testModel));

            Assert.AreEqual(11, testModel.SampleNumber);
        }

        [TestMethod]
        public void CanRunCommandWithReturn()
        {
            int startNumber = 8;
            var handlerResolver = new TasqHandlerResolver();

            handlerResolver.Register<SampleCommandWithReturnHandler>();

            var tasqR = new TasqRObject(handlerResolver);

            int finalNumber = tasqR.Run(new SampleCommandWithReturn(startNumber));

            Assert.AreEqual(9, finalNumber);
        }

        [TestMethod]
        public async Task CanRunAsynchronously()
        {
            var testModel = new Test3Model { StartNumber = 10 };
            var handlerResolver = new TasqHandlerResolver();

            handlerResolver.Register<CommandWithAsyncWithoutReturnHandler>();

            var tasqR = new TasqRObject(handlerResolver);

            await tasqR.RunAsync(new CommandWithAsyncWithoutReturn(testModel));

            Assert.AreEqual(11, testModel.StartNumber);
        }

        [TestMethod]
        public void CanRunCommandWithKeys()
        {
            var handlerResolver = new TasqHandlerResolver();

            handlerResolver.Register<CommandWithKeyHandler>();

            var tasqR = new TasqRObject(handlerResolver);
            var cmd = new CommandWithKey();

            tasqR.Run(cmd);

            Assert.IsTrue(cmd.AllAreCorrect);
        }

        [TestMethod]
        public void TasqReferenceObjectResolver()
        {
            var typeTaskRef = TypeTasqReference.Resolve<SampleCommandWithoutReturnHandler>();

            Assert.AreEqual(typeof(SampleCommandWithoutReturn), typeTaskRef.TasqProcess);
            Assert.AreEqual(typeof(SampleCommandWithoutReturnHandler), typeTaskRef.HandlerImplementation);
            Assert.AreEqual(typeof(IJobTasqHandler<SampleCommandWithoutReturn>), typeTaskRef.HandlerInterface);
        }
    }
}
