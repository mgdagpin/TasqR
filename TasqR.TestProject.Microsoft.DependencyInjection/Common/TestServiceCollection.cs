using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TasqR.TestProject.Microsoft.DependencyInjection.Test2;

namespace TasqR.TestProject.Microsoft.DependencyInjection.Common
{
    public class TestServiceCollection : IDisposable
    {
        private ServiceCollection services = null;
        private ServiceProvider serviceProvider = null;

        public TestServiceCollection()
        {
            services = new ServiceCollection();
        }

        public void Register()
        {
            services.AddTasqR(Assembly.GetExecutingAssembly());

            serviceProvider = services.BuildServiceProvider();
        }

        public T GetService<T>()
        {
            return serviceProvider.GetService<T>();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                serviceProvider.Dispose();

            }
        }
    }
}
