using FluentValidation;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;

namespace MyRecipeBook.Application.UseCases.Recipe;

public class RecipeValidator : AbstractValidator<RequestRecipeJson>
{
    public RecipeValidator()
    {
        RuleFor(recipe => recipe.Title)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ResourceMessagesExceptions.VALIDATION_TITLE_REQUIRED)
            .MaximumLength(250).WithMessage(ResourceMessagesExceptions.VALIDATION_TITLE_MAX_LENGTH);

        RuleFor(recipe => recipe.DishTypes).NotEmpty()
            .WithMessage(ResourceMessagesExceptions.VALIDATION_AT_LEAST_ONE_DISH_TYPE);
        
        RuleForEach(recipe => recipe.DishTypes).IsInEnum()
            .WithMessage(ResourceMessagesExceptions.VALIDATION_DISH_TYPE_INVALID);

        RuleFor(recipe => recipe.CookTime).NotEmpty()
            .WithMessage(ResourceMessagesExceptions.VALIDATION_COOK_TIME_REQUIRED);
        
        RuleFor(recipe => recipe.CookTime).IsInEnum()
            .WithMessage(ResourceMessagesExceptions.VALIDATION_COOK_TIME_INVALID);
        
        RuleForEach(recipe => recipe.Ingredients).ChildRules(requestIngredient =>
        {
            requestIngredient.RuleFor(ingredient => ingredient.Item)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(ResourceMessagesExceptions.VALIDATION_INGREDIENT_EMPTY)
                .MaximumLength(250).WithMessage(ResourceMessagesExceptions.VALIDATION_INGREDIENT_MAX_LENGTH);
        });


        RuleFor(recipe => recipe.Ingredients).NotEmpty().WithMessage(ResourceMessagesExceptions.VALIDATION_AT_LEAST_ONE_INSTRUCTION);
        RuleForEach(recipe => recipe.Instructions).ChildRules(requestInstruction =>
        {
            requestInstruction.RuleFor(instruction => instruction.Order)
                .GreaterThan(0).WithMessage(ResourceMessagesExceptions.VALIDATION_INSTRUCTION_ORDER_INVALID);

            requestInstruction.RuleFor(instruction => instruction.Description)
                .NotEmpty().WithMessage(ResourceMessagesExceptions.VALIDATION_INSTRUCTION_DESCRIPTION_REQUIRED)
                .MaximumLength(2000).WithMessage(ResourceMessagesExceptions.VALIDATION_INSTRUCTION_DESCRIPTION_MAX_LENGTH);
        });
        
    }
}