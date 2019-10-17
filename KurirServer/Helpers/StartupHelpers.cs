using KurirServer.Intefaces;
using KurirServer.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
namespace KurirServer.Helpers
{
    public static class StartupHelpers
    {
        
        public static void AddScopedServices(this IServiceCollection services)
        {
            services.AddScoped<IGeneralRepository, GeneralRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<IPaymentTypeRepository, PaymentTypeRepository>();
            services.AddScoped<IDeliveryTypeRepository, DeliveryTypeRepository>();
            services.AddScoped<IDeliveryRepository,DeliveryRepository>();
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddScoped<IAddressRepository,AddressRepository>();

        }
    }
}
