using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TasqR.TestProject.Microsoft.DependencyInjection.Common;
using TasqR.TestProject.Microsoft.DependencyInjection.Test1;
using TasqR.TestProject.Microsoft.DependencyInjection.Test2;
using TasqR.TestProject.Microsoft.DependencyInjection.Test3;

namespace TasqR.TestProject.Microsoft.DependencyInjection
{
    [TestClass]
    public class DITesting
    {
        static ServiceCollection services = null;


        [TestInitialize]
        public void TestInit()
        {
            if (services == null)
            {
                services = new ServiceCollection();

                services.AddScoped(p => new TrackMeInScope
                {
                    ID = Guid.NewGuid()
                });

                services.AddTransient(p => new TrackMeInTransient
                {
                    ID = Guid.NewGuid()
                });

                services.AddSingleton(p => new TrackMeInSingleton
                {
                    ID = Guid.NewGuid()
                });

                services.AddTasqR(Assembly.GetExecutingAssembly());


            }
        }

        [TestMethod]
        public void TasqR_Is_RegisteredAs_Singleton()
        {
            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                IServiceProvider svc = scope.ServiceProvider;

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
            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                IServiceProvider svc = scope.ServiceProvider;


                var tasqr = svc.GetService<ITasqR>();

                var testModel = new TestModel { SampleNumber = 1 };
                tasqr.Run(new CommandWithoutReturn(testModel));

                Assert.AreEqual(2, testModel.SampleNumber);
            }
        }

        [TestMethod]
        public void CanDetectHandlersForCommandWithReturn()
        {
            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                IServiceProvider svc = scope.ServiceProvider;

                var tasqr = svc.GetService<ITasqR>();

                int finalNumber = tasqr.Run(new CommandWithReturn(1));

                Assert.AreEqual(2, finalNumber);
            }
        }

        [TestMethod]
        public void CanExecuteNestedProcessWithReturn()
        {
            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                IServiceProvider svc = scope.ServiceProvider;

                var tasqr = svc.GetService<ITasqR>();

                int finalNumber = tasqr.Run(new NestedCommandWithReturn(1));

                Assert.AreEqual(3, finalNumber);
            }
        }

        [TestMethod]
        public void CanRunWithMultiplePassedHandlerBase()
        {
            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                IServiceProvider svc = scope.ServiceProvider;

                var tasqr = svc.GetService<ITasqR>();

                var cmd = new TestCommandWithMultipleHandler(5);

                Assert.AreEqual(15, tasqr.Run(cmd));
            }
        }

        [TestMethod]
        public void CanRunWithMultiplePassedHandlerInherit1()
        {
            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                IServiceProvider svc = scope.ServiceProvider;

                var tasqr = svc.GetService<ITasqR>();

                var cmd = new TestCommandWithMultipleHandler(5);

                Assert.AreEqual(25, tasqr.UsingAsHandler(typeof(TestCommandWithMultipleHandlerHandler2)).Run(cmd));
            }
        }

        [TestMethod]
        public void CanRunWithMultiplePassedHandlerInherit2()
        {
            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                IServiceProvider svc = scope.ServiceProvider;

                var tasqr = svc.GetService<ITasqR>();

                var cmd = new TestCommandWithMultipleHandler(5);

                Assert.AreEqual(25, tasqr.UsingAsHandler(typeof(TestCommandWithMultipleHandlerHandler3)).Run(cmd));
            }
        }

        [TestMethod]
        public void CanHaveSameTasqRAndSameObjectInScope()
        {
            using (var serviceProvider = services.BuildServiceProvider())
            {
                ITasqR tasqR1 = null;
                TrackMeInScope trackMe1 = null;

                ITasqR tasqR2 = null;
                TrackMeInScope trackMe2 = null;

                using (var scope = serviceProvider.CreateScope())
                {
                    var scopeSvc = scope.ServiceProvider;

                    tasqR1 = scopeSvc.GetService<ITasqR>();
                    trackMe1 = scopeSvc.GetService<TrackMeInScope>();

                    var cmd = new DINestedCmd1();
                    var res = tasqR1.Run(cmd);

                    Assert.AreEqual(tasqR1.ID, res[$"{nameof(DINestedCmd1Handler)}_{nameof(DINestedCmd1Handler.p_TasqR)}"]);
                    Assert.AreEqual(tasqR1.ID, res[$"{nameof(DINestedCmd2Handler)}_{nameof(DINestedCmd2Handler.p_TasqR)}"]);
                    Assert.AreEqual(tasqR1.ID, res[$"{nameof(DINestedCmd3Handler)}_{nameof(DINestedCmd3Handler.p_TasqR)}"]);

                    Assert.AreEqual(trackMe1.ID, res[$"{nameof(DINestedCmd1Handler)}_{nameof(DINestedCmd1Handler.p_TrackMe)}"]);
                    Assert.AreEqual(trackMe1.ID, res[$"{nameof(DINestedCmd2Handler)}_{nameof(DINestedCmd2Handler.p_TrackMe)}"]);
                    Assert.AreEqual(trackMe1.ID, res[$"{nameof(DINestedCmd3Handler)}_{nameof(DINestedCmd3Handler.p_TrackMe)}"]);
                }

                using (var scope = serviceProvider.CreateScope())
                {
                    var scopeSvc = scope.ServiceProvider;

                    tasqR2 = scopeSvc.GetService<ITasqR>();
                    trackMe2 = scopeSvc.GetService<TrackMeInScope>();

                    var cmd = new DINestedCmd1();
                    var res = tasqR2.Run(cmd);

                    Assert.AreEqual(tasqR2.ID, res[$"{nameof(DINestedCmd1Handler)}_{nameof(DINestedCmd1Handler.p_TasqR)}"]);
                    Assert.AreEqual(tasqR2.ID, res[$"{nameof(DINestedCmd2Handler)}_{nameof(DINestedCmd2Handler.p_TasqR)}"]);
                    Assert.AreEqual(tasqR2.ID, res[$"{nameof(DINestedCmd3Handler)}_{nameof(DINestedCmd3Handler.p_TasqR)}"]);

                    Assert.AreEqual(trackMe2.ID, res[$"{nameof(DINestedCmd1Handler)}_{nameof(DINestedCmd1Handler.p_TrackMe)}"]);
                    Assert.AreEqual(trackMe2.ID, res[$"{nameof(DINestedCmd2Handler)}_{nameof(DINestedCmd2Handler.p_TrackMe)}"]);
                    Assert.AreEqual(trackMe2.ID, res[$"{nameof(DINestedCmd3Handler)}_{nameof(DINestedCmd3Handler.p_TrackMe)}"]);
                }

                Assert.AreNotEqual(tasqR1.ID, tasqR2.ID);
                Assert.AreNotEqual(trackMe1.ID, trackMe2.ID);
            }            
        }
    }
}
