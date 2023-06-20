using System;
using System.Linq;
using System.Threading.Tasks;
using TasqR.Common;
using TasqR.TestProject.Test1;
using TasqR.TestProject.Test2;
using TasqR.TestProject.Test3;
using TasqR.TestProject.Test4;
using TasqR.TestProject.Test6;
using Xunit;

namespace TasqR.TestProject
{
    public static class RunAsync
    {
        public class VoidReturn
        {
            [Fact]
            public async Task AsyncHandler()
            {
                var testModel = new Test3Model { StartNumber = 10 };
                var handlerResolver = new TasqHandlerResolver();

                handlerResolver.Register<CommandWithAsyncWithoutReturnHandler>();

                var tasqR = new TasqR(handlerResolver);
                var cmd = new CommandWithAsyncWithoutReturn(testModel);

                await tasqR.RunAsync(cmd);

                Assert.Equal(11, testModel.StartNumber);
            }

            [Fact]
            public async Task NonAsyncHandler()
            {
                var testModel = new TestModel { SampleNumber = 10 };
                var handlerResolver = new TasqHandlerResolver();

                handlerResolver.Register<SampleCommandWithoutReturnHandler>();

                var tasqR = new TasqR(handlerResolver);
                var cmd = new SampleCommandWithoutReturn(testModel);

                await tasqR.RunAsync(cmd);

                Assert.Equal(11, testModel.SampleNumber);
            }
        }

        public class WithReturn
        {
            [Fact]
            public async Task AsyncHandler()
            {
                var handlerResolver = new TasqHandlerResolver();
                handlerResolver.Register<TestCmdWithReturnForAsyncHandler>();
                var tasqR = new TasqR(handlerResolver);
                var cmd = new TestCmdWithReturnForAsync(2);

                var result = await tasqR.RunAsync(cmd);

                Assert.Equal(3, result);
            }

            [Fact]
            public async Task NonAsyncHandler()
            {
                int startNumber = 8;
                var handlerResolver = new TasqHandlerResolver();

                handlerResolver.Register<SampleCommandWithReturnHandler>();

                var tasqR = new TasqR(handlerResolver);
                var cmd = new SampleCommandWithReturn(startNumber);

                var result = await tasqR.RunAsync(cmd);

                Assert.Equal(9, result);
            }
        }

        public class WithKey
        {
            [Fact]
            public async Task AsyncHandler()
            {
                var handlerResolver = new TasqHandlerResolver();

                handlerResolver.Register<CommandWithKeyAsyncHandler>();

                var tasqR = new TasqR(handlerResolver);
                var cmd = new CommandWithKeyAsync();

                int initialCount = cmd.TempData.Count;

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

                Assert.Equal(initialCount, callCount);
                Assert.True(allAreTrue);
            }

            [Fact]
            public async Task AsyncDynamicHandler_CastedToITasq()
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

                Assert.True(allAreTrue);
            }

            [Fact]
            public async Task AsyncDynamicHandler()
            {
                var handlerResolver = new TasqHandlerResolver();

                handlerResolver.Register<CommandWithKeyAsyncHandler>();

                var tasqR = new TasqR(handlerResolver);
                var instance = (ITasq)Activator.CreateInstance(typeof(CommandWithKeyAsync));

                await tasqR.RunAsync(instance);


                var instanceResult = instance as CommandWithKeyAsync;

                Assert.True(instanceResult.TempData.All(a => a.Value));
            }

            [Fact]
            public async Task NonAsyncHandler()
            {
                var handlerResolver = new TasqHandlerResolver();

                handlerResolver.Register<CommandWithKeyHandler>();

                var tasqR = new TasqR(handlerResolver);
                var cmd = new CommandWithKey();

                int callCount = 0;
                bool allAreTrue = true;
                await foreach (var item in tasqR.RunAsync(cmd))
                {
                    callCount++;
                    if (!item)
                    {
                        allAreTrue = false;
                    }
                }

                Assert.True(cmd.AllAreCorrect);
                Assert.True(allAreTrue);
            }
        }
    }
}
