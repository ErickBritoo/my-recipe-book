using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Application.Mappings;
using MyRecipeBook.Application.UseCases.Login.WithEmailAndPassword;
using MyRecipeBook.Application.UseCases.Recipe.DeleteById;
using MyRecipeBook.Application.UseCases.Recipe.GetById;
using MyRecipeBook.Application.UseCases.Recipe.Register;
using MyRecipeBook.Application.UseCases.Recipe.Update;
using MyRecipeBook.Application.UseCases.User.ChangePassword;
using MyRecipeBook.Application.UseCases.User.Profile;
using MyRecipeBook.Application.UseCases.User.Register;
using MyRecipeBook.Application.UseCases.User.Update;

namespace MyRecipeBook.Application;

public static class DependencyInjectionExtensions 
{
    extension(IServiceCollection services)
    {
        public void AddApplication()
        {
            MapsterConfig.Configure();
            
            services.AddScoped<IRegisterUserUseCase, RegisterUserUseCase>();
            services.AddScoped<ILoginWithEmailAndPasswordUseCase, LoginWithEmailAndPasswordUseCase>();
            services.AddScoped<IGetUserProfileUseCase, GetUserProfileUseCase>();
            services.AddScoped<IChangePasswordUseCase, ChangePasswordUseCase>();
            services.AddScoped<IUpdateUserUseCase, UpdateUserUseCase>();
            
            services.AddScoped<IRegisterRecipeUseCase, RegisterRecipeUseCase>();
            services.AddScoped<IGetByIdRecipeUseCase, GetByIdRecipeUseCase>();
            services.AddScoped<IDeleteRecipeByIdUseCase, DeleteRecipeByIdUseCase>();
            services.AddScoped<IUpdateRecipeUseCase, UpdateRecipeUseCase>();
        }
    }
}