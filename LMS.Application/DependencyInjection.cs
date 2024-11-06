using LMS.Application.Interfaces;
using LMS.Application.Services;
using LMS.Infrastructure.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            return services
                 .AddTransient<IBookService, BookService>()
                 .AddScoped<IUnitOfWork, UnitOfWork>()
                 .AddTransient<ITokenService, TokenService>();
        }
    }
}
