using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TasqR.TestProject.Microsoft.DependencyInjection.Test2;

namespace TasqR.TestProject.Microsoft.DependencyInjection.Common
{
    public class TestServiceCollection : IDisposable
    {
        private readonly ServiceCollection p_Services = new ServiceCollection();
        private IServiceProvider serviceProvider = null;

        public void Register()
        {
            p_Services.AddTasqR(ServiceLifetime.Scoped, Assembly.GetExecutingAssembly());

            serviceProvider = p_Services.BuildServiceProvider();
        }

        public T GetService<T>()
        {
            return serviceProvider.CreateScope().ServiceProvider.GetService<T>();
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
                //serviceProvider.Dispose();

            }
        }
    }
}
