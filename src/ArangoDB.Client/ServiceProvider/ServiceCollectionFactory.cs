using ArangoDB.Client.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArangoDB.Client.ServiceProvider
{
    public class ServiceCollectionFactory
    {
        private readonly static object _lock = new object();
        static bool providerCreated;
        static IServiceCollection serviceCollection;

        IServiceProvider serviceProvider;

        public readonly static IServiceProvider ServiceProvider;

        static ServiceCollectionFactory()
        {
            ServiceProvider = new ServiceCollectionFactory().BuilderServiceProvider();
        }

        void AddCoreServices(IServiceCollection services)
        {
            var loggerFactory = new LoggerFactory();
            services.AddSingleton<ILoggerFactory, LoggerFactory>((s) => loggerFactory);
            DatabaseConfig.ConfigureLoggerFactory?.Invoke(loggerFactory);

            services.AddSingleton<DatabaseConfigContainer, DatabaseConfigContainer>();

            services.AddScoped<ScopeService, ScopeService>();
        }

        IServiceProvider BuilderServiceProvider()
        {
            if (providerCreated == false)
            {
                lock (_lock)
                {
                    if (providerCreated == false)
                    {
                        serviceCollection = new ServiceCollection();
                        AddCoreServices(serviceCollection);
                        serviceProvider = serviceCollection.BuildServiceProvider();
                        providerCreated = true;
                    }
                }
            }

            return serviceProvider;
        }
    }
}
