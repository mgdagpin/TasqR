using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TasqR.Common;
using TasqR.TestProject.Test1;
using TasqR.TestProject.Test2;
using TasqR.TestProject.Test4;

namespace TasqR.TestProject
{
    [TestClass]
    public class TasqRTesting
    {
        [TestMethod]
        public void CanRunWithoutReturn()
        {
            var testModel = new TestModel { SampleNumber = 10 };
            var handlerResolver = new TasqHandlerResolver();

            handlerResolver.Register<SampleCommandWithoutReturnHandler>();

            var tasqR = new TasqRObject(handlerResolver);
            var cmd = new SampleCommandWithoutReturn(testModel);

            tasqR.Run(cmd);

            Assert.AreEqual(11, testModel.SampleNumber);
        }

        [TestMethod]
        public void CanRunWithReturn()
        {
            int startNumber = 8;
            var handlerResolver = new TasqHandlerResolver();

            handlerResolver.Register<SampleCommandWithReturnHandler>();

            var tasqR = new TasqRObject(handlerResolver);
            var cmd = new SampleCommandWithReturn(startNumber);

            int finalNumber = tasqR.Run(cmd);

            Assert.AreEqual(9, finalNumber);
        }


        [TestMethod]
        public void CanRunWithKey()
        {
            var handlerResolver = new TasqHandlerResolver();

            handlerResolver.Register<CommandWithKeyHandler>();

            var tasqR = new TasqRObject(handlerResolver);
            var cmd = new CommandWithKey();

            tasqR.Run(cmd);

            Assert.IsTrue(cmd.AllAreCorrect);
        }

        [TestMethod]
        public void CanRunWithKeyBaseType()
        {
            var handlerResolver = new TasqHandlerResolver();

            handlerResolver.Register<CommandWithKeyHandler>();

            var tasqR = new TasqRObject(handlerResolver);
            var instance = Activator.CreateInstance(typeof(CommandWithKey));

            tasqR.Run((ITasq<int, bool>)instance);

            Assert.IsTrue(((CommandWithKey)instance).AllAreCorrect);
        }

        [TestMethod]
        public void TasqReferenceObjectResolver()
        {
            var typeTaskRef = TypeTasqReference.Resolve<SampleCommandWithoutReturnHandler>();

            Assert.AreEqual(typeof(SampleCommandWithoutReturn), typeTaskRef.TasqProcess);
            Assert.AreEqual(typeof(SampleCommandWithoutReturnHandler), typeTaskRef.HandlerImplementation);
            Assert.AreEqual(typeof(ITasqHandler<SampleCommandWithoutReturn>), typeTaskRef.HandlerInterface);
        }
    }
}
