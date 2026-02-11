using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIDemo.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DIDemo.Infrastructures
{
    public static class ServiceExtensions
    {
        public static void AddCustomServices(this IServiceCollection services)
        {
            services.AddSingleton<IMessageService, MessageService>();

            // Singleton: le DataService garde son état entre les vues
            services.AddSingleton<IDataService, DataService>();

            // Transient: nouvelle instance à chaque résolution
            services.AddTransient<INavigationService, NavigationService>();
            //L'ajout dans le conteneurs de dépendances des services
        }
    }
}
