using Microsoft.VisualStudio.TestTools.UnitTesting;
using TasqR.TestProject.Microsoft.DependencyInjection.Common;
using TasqR.TestProject.Microsoft.DependencyInjection.Test1;

namespace TasqR.TestProject.Microsoft.DependencyInjection
{
    [TestClass]
    public class DITesting
    {
        [TestMethod]
        public void TasqR_Is_RegisteredAs_Singleton()
        {
            using (var svc = new TestServiceCollection())
            {
                svc.Register();

                var tasqr1 = svc.GetService<ITasqR>();
                var tasqr2 = svc.GetService<ITasqR>();
                var tasqr3 = svc.GetService<ITasqR>();
                var tasqr4 = svc.GetService<ITasqR>();

                Assert.AreEqual(tasqr1.ID, tasqr2.ID);
                Assert.AreEqual(tasqr2.ID, tasqr3.ID);
                Assert.AreEqual(tasqr3.ID, tasqr4.ID);
                Assert.AreEqual(tasqr4.ID, tasqr1.ID);
            }
        }

        [TestMethod]
        public void CanDetectHandlersForCommandWithoutReturn()
        {
            using (var svc = new TestServiceCollection())
            {
                svc.Register();

                var tasqr = svc.GetService<ITasqR>();

                var testModel = new TestModel { SampleNumber = 1 };
                tasqr.Run(new CommandWithoutReturn(testModel));

                Assert.AreEqual(2, testModel.SampleNumber);
            }
        }

        [TestMethod]
        public void CanDetectHandlersForCommandWithReturn()
        {
            using (var svc = new TestServiceCollection())
            {
                svc.Register();

                var tasqr = svc.GetService<ITasqR>();

                int finalNumber = tasqr.Run(new CommandWithReturn(1));

                Assert.AreEqual(2, finalNumber);
            }
        }

        [TestMethod]
        public void CanExecuteNestedProcessWithReturn()
        {
            using (var svc = new TestServiceCollection())
            {
                svc.Register();

                var tasqr = svc.GetService<ITasqR>();

                int finalNumber = tasqr.Run(new NestedCommandWithReturn(1));

                Assert.AreEqual(3, finalNumber);
            }
        }

        [TestMethod]
        public void CanDetectDerivedHandlers()
        {
            using (var svc = new TestServiceCollection())
            {
                svc.Register();

                var tasqr = svc.GetService<ITasqR>();

                int finalNumber = tasqr.Run(new HandlerDerivedFromAnotherHandlerCmd(1));

                Assert.AreEqual(3, finalNumber);
            }
        }
    }
}
