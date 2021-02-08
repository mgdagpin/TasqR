using Microsoft.VisualStudio.TestTools.UnitTesting;
using TasqR.TestProject.Test1;
using TasqR.TestProject.Test2;
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
            var cmd = new SampleCommandWithoutReturn(testModel);

            tasqR.Run(cmd);

            Assert.AreEqual(11, testModel.SampleNumber);
        }

        [TestMethod]
        public void CanRunCommandWithReturn()
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
            Assert.AreEqual(typeof(ITasqHandler<SampleCommandWithoutReturn>), typeTaskRef.HandlerInterface);
        }

        //[TestMethod]
        //public void MyTestMethod()
        //{
        //    ICage<Dog> a = new CageOfDog();

        //    INiceCage<IAnimal> b = (RedCage<Dog>)a;
        //}


        //public interface IAnimal
        //{
        //}

        //public class Dog : IAnimal
        //{
        //}

        //public class Cat : IAnimal
        //{
        //}

        //public interface ICage<in TAnimal> where TAnimal : IAnimal
        //{

        //}

        //public interface INiceCage<in TAnimal> : ICage<TAnimal> where TAnimal : IAnimal
        //{

        //}

        //public class RedCage<TAnimal> : ICage<TAnimal> where TAnimal : IAnimal
        //{

        //}

        //public class CageOfDog : RedCage<Dog>
        //{
        //}
    }
}
