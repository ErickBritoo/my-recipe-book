using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.API.Filters;

public class ExceptionsFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is MyRecipeBookExceptions exception)
            ThrowException(context, exception);
        else
            ThrowUnknowException(context);
    }

    private static void ThrowUnknowException(ExceptionContext context)
    {
        Console.WriteLine(context.Exception.Message);
        context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Result = new ObjectResult(new ResponseErrorJson(ResourceMessagesExceptions.UNKNOW_ERROR));
    }

    private static void ThrowException(ExceptionContext context, MyRecipeBookExceptions exception)
    {
        context.HttpContext.Response.StatusCode = (int)exception.GetStatusCodes();
        context.Result = new ObjectResult(new ResponseErrorJson(exception.GetErrorMessages()));
    }
}