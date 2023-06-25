using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TasqR.Common;
using TasqR.TestProject.Test1;
using TasqR.TestProject.Test2;
using TasqR.TestProject.Test3;
using TasqR.TestProject.Test4;
using TasqR.TestProject.Test5;
using TasqR.TestProject.Test6;
using TasqR.TestProject.Test7;
using Xunit;

namespace TasqR.TestProject
{
    public static class Run
    {
        public class VoidReturn
        {
            [Fact]
            public void AsyncHandler()
            {
                var testModel = new Test3Model { StartNumber = 10 };
                var assemblyCol = new TasqAssemblyCollection();
                var handlerResolver = new TasqHandlerResolver(assemblyCol);

                assemblyCol.Register<CommandWithAsyncWithoutReturnHandler>();

                var tasqR = new TasqR(handlerResolver);
                var cmd = new CommandWithAsyncWithoutReturn(testModel);

                tasqR.Run(cmd);

                Assert.Equal(11, testModel.StartNumber);
            }

            [Fact]
            public void NonAsyncHandler()
            {
                var testModel = new TestModel { SampleNumber = 10 };
                var assemblyCol = new TasqAssemblyCollection();
                var handlerResolver = new TasqHandlerResolver(assemblyCol);

                assemblyCol.Register<SampleCommandWithoutReturnHandler>();

                var tasqR = new TasqR(handlerResolver);
                var cmd = new SampleCommandWithoutReturn(testModel);

                tasqR.Run(cmd);

                Assert.Equal(11, testModel.SampleNumber);
            }

            [Fact]
            public void CannotRunDefaultCmdWithHandlerWithNoParamlessConstructor()
            {
                try
                {
                    var assemblyCol = new TasqAssemblyCollection();
                    var handlerResolver = new TasqHandlerResolver(assemblyCol);

                    assemblyCol.Register<CmdHandlerWithNoParamlessCtorHandler>();
                    var tasqR = new TasqR(handlerResolver);

                    var cmd = new CmdHandlerWithNoParamlessCtor();
                    tasqR.Run(cmd);
                }
                catch (Exception ex)
                {
                    Assert.Equal(typeof(MissingMethodException), ex.GetType());

                    Assert.StartsWith("No parameterless constructor defined for type", ex.Message);
                }
            }

            [Fact]
            public void TasqReferenceObjectResolver()
            {
                var typeTaskRef = TypeTasqReference.Resolve<SampleCommandWithoutReturnHandler>();

                Assert.Equal(typeof(SampleCommandWithoutReturn), typeTaskRef.TasqProcess);
                Assert.Equal(typeof(SampleCommandWithoutReturnHandler), typeTaskRef.HandlerImplementation);
                Assert.Equal(typeof(ITasqHandler<SampleCommandWithoutReturn>), typeTaskRef.HandlerInterface);
            }
        }

        public class WithReturn
        {
            [Fact]
            public void AsyncHandler()
            {
                var assemblyCol = new TasqAssemblyCollection();
                var handlerResolver = new TasqHandlerResolver(assemblyCol);

                assemblyCol.Register<TestCmdWithReturnForAsyncHandler>();
                var tasqR = new TasqR(handlerResolver);
                var cmd = new TestCmdWithReturnForAsync(2);

                var result = tasqR.Run(cmd);

                Assert.Equal(3, result);
            }

            [Fact]
            public void NonAsyncHandler()
            {
                int startNumber = 8;
                var assemblyCol = new TasqAssemblyCollection();
                var handlerResolver = new TasqHandlerResolver(assemblyCol);

                assemblyCol.Register<SampleCommandWithReturnHandler>();

                var tasqR = new TasqR(handlerResolver);
                var cmd = new SampleCommandWithReturn(startNumber);

                var result = tasqR.Run(cmd);

                Assert.Equal(9, result);
            }
        }

        public class WithKey
        {
            [Fact]
            public void AsyncHandler()
            {
                var assemblyCol = new TasqAssemblyCollection();
                var handlerResolver = new TasqHandlerResolver(assemblyCol);

                assemblyCol.Register<CommandWithKeyAsyncHandler>();

                var tasqR = new TasqR(handlerResolver);
                var cmd = new CommandWithKeyAsync();

                int initialCount = cmd.TempData.Count;

                bool allAreTrue = true;
                int callCount = 0;

                foreach (var item in tasqR.Run(cmd))
                {
                    callCount++;
                    if (!item)
                    {
                        allAreTrue = false;
                    }
                }

                Assert.Equal(initialCount, callCount);
                Assert.True(allAreTrue);
            }

            [Fact]
            public void AsyncDynamicHandler_CastedToITasq()
            {
                var assemblyCol = new TasqAssemblyCollection();
                var handlerResolver = new TasqHandlerResolver(assemblyCol);

                assemblyCol.Register<CommandWithKeyAsyncHandler>();

                var tasqR = new TasqR(handlerResolver);
                var instance = (ITasq<int, bool>)Activator.CreateInstance(typeof(CommandWithKeyAsync));

                bool allAreTrue = true;

                foreach (var item in tasqR.Run(instance))
                {
                    if (!item)
                    {
                        allAreTrue = false;
                    }
                }

                Assert.True(allAreTrue);
            }

            [Fact]
            public void AsyncDynamicHandler()
            {
                var assemblyCol = new TasqAssemblyCollection();
                var handlerResolver = new TasqHandlerResolver(assemblyCol);

                assemblyCol.Register<CommandWithKeyAsyncHandler>();

                var tasqR = new TasqR(handlerResolver);
                var instance = (ITasq)Activator.CreateInstance(typeof(CommandWithKeyAsync));

                tasqR.Run(instance);


                var instanceResult = instance as CommandWithKeyAsync;

                Assert.True(instanceResult.TempData.All(a => a.Value));
            }

            [Fact]
            public void NonAsyncHandler()
            {
                var assemblyCol = new TasqAssemblyCollection();
                var handlerResolver = new TasqHandlerResolver(assemblyCol);

                assemblyCol.Register<CommandWithKeyHandler>();

                var tasqR = new TasqR(handlerResolver);
                var cmd = new CommandWithKey();

                int callCount = 0;
                bool allAreTrue = true;
                foreach (var item in tasqR.Run(cmd))
                {
                    callCount++;
                    if (!item)
                    {
                        allAreTrue = false;
                    }
                }

                Assert.True(cmd.Data.All(a => a.Value));
                Assert.True(allAreTrue);
            }

            [Fact]
            public void NonAsyncDynamicHandler_CastedToITasq()
            {
                var assemblyCol = new TasqAssemblyCollection();
                var handlerResolver = new TasqHandlerResolver(assemblyCol);

                assemblyCol.Register<CommandWithKeyHandler>();

                var tasqR = new TasqR(handlerResolver);
                var instance = Activator.CreateInstance(typeof(CommandWithKey));

                _ = tasqR.Run((ITasq<int, bool>)instance).ToList();

                Assert.True(((CommandWithKey)instance).Data.All(a => a.Value));
            }

            [Fact]
            public void NonAsyncDynamicHandler()
            {
                var assemblyCol = new TasqAssemblyCollection();
                var handlerResolver = new TasqHandlerResolver(assemblyCol);

                assemblyCol.Register<CommandWithKeyHandler>();

                var tasqR = new TasqR(handlerResolver);
                var instance = (ITasq)Activator.CreateInstance(typeof(CommandWithKey));

                tasqR.Run(instance);

                Assert.True(((CommandWithKey)instance).Data.All(a => a.Value));
            }

            [Fact]
            public async Task WillThrowExceptionFromAsyncHandler()
            {
                try
                {
                    var assemblyCol = new TasqAssemblyCollection();
                    var handlerResolver = new TasqHandlerResolver(assemblyCol);

                    assemblyCol.Register<TestWithErrorCmdHandler>();
                    var tasqR = new TasqR(handlerResolver);

                    var cmd = new TestWithErrorCmd();

                    var result = new List<bool>();

                    await foreach (var item in tasqR.RunAsync(cmd))
                    {
                        result.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    var baseEx = ex.GetBaseException();

                    Assert.Equal(typeof(NotImplementedException), baseEx.GetType());
                }
            }

            [Fact]
            public void CanThrowExceptionIfNoHandlerRegistered()
            {
                try
                {
                    var assemblyCol = new TasqAssemblyCollection();
                    var handlerResolver = new TasqHandlerResolver(assemblyCol);

                    var tasqR = new TasqR(handlerResolver);
                    var cmd = new CommandWithKey();

                    _ = tasqR.Run(cmd).ToList();

                }
                catch (Exception ex)
                {
                    Assert.Equal(typeof(TasqException), ex.GetType());
                    Assert.Equal("Type CommandWithKey not registered", ex.Message);
                }
            }
        }
    }
}