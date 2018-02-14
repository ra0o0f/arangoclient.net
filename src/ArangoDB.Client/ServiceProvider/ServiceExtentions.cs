using ArangoDB.Client.Config;
using ArangoDB.Client.ServiceProvider;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArangoDB.Client.ServiceProvider
{
    public static class ServiceExtentions
    {
        public static T ScopeItem<T>(this IServiceProvider serviceProvider) where T : class, IScopeItem
        {
            var scopedService = serviceProvider.GetRequiredService<ScopeService>();

            if (typeof(T) == typeof(IDatabaseConfig))
                return scopedService.DatabaseConfig as T;

            throw new InvalidOperationException($"No scope item found for type of {typeof(T)}");
        }

        public static void SetScopeItem(this IServiceProvider serviceProvider, IScopeItem scopeItem)
        {
            var scopedService = serviceProvider.GetRequiredService<ScopeService>();

            if (scopeItem is IDatabaseConfig item)
            {
                scopedService.DatabaseConfig = item;
                return;
            }
            
            throw new InvalidOperationException($"No scope item found for type of {scopeItem.GetType()}");
        }
    }
}
