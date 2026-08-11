using Mapster;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Application.UseCases.User.Register;

namespace MyRecipeBook.Application;

public static class DependencyInjectionExtensions 
{
    extension(IServiceCollection services)
    {
        public void AddApplication()
        {
            services.AddScoped<IRegisterUserUseCase, RegisterUserUseCase>();
        }
    }
}