using FechDataApplication;
using FechDataPresentation.Areas.MyFeature.Pages;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BlazorServerIngrastructure
{
    public class Infrastructure
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<WeatherForecastService>();
        }
        public static Assembly[] GetAssemblies()
        {
            return new[] { typeof(FechData).Assembly };
        }
    }
}
