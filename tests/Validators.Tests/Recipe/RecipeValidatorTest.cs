using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.Recipe;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;
using Shouldly;
using Xunit;

namespace Validators.Tests.Recipe;

public class RecipeValidatorTest
{
    [Fact]
    public void Sucess()
    {
        var request = RequestRecipeJsonBuilder.Build();

        var validator = new RecipeValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("               ")]
    public void Validate_Error_When_TitleEmpty(string? title)
    {       
        var request = RequestRecipeJsonBuilder.Build();
        request.Title = title!;

        var validator = new RecipeValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error =>
                error.ErrorMessage.Equals(ResourceMessagesExceptions.VALIDATION_TITLE_REQUIRED));
        });
    }

    [Fact]
    public void Validate_Error_When_TitleExceedsMaxLength()
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.Title = new string('e', 251);

        var validator = new RecipeValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error =>
                error.ErrorMessage.Equals(ResourceMessagesExceptions.VALIDATION_TITLE_MAX_LENGTH));
        });
    }

    [Fact]
    public void Validate_Error_When_DishTypeEmpty()
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.DishTypes = [];

        var validator = new RecipeValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error =>
                error.ErrorMessage.Equals(ResourceMessagesExceptions.VALIDATION_AT_LEAST_ONE_DISH_TYPE));
        });
    }

    [Fact]
    public void Validate_Error_When_DishTypeInvalid()
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.DishTypes = [(DishType)200];

        var validator = new RecipeValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error =>
                error.ErrorMessage.Equals(ResourceMessagesExceptions.VALIDATION_DISH_TYPE_INVALID));
        });
    }


    [Fact]
    public void Validate_Error_When_CookTimeInvalid()
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.CookTime = (CookTime)200;

        var validator = new RecipeValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error =>
                error.ErrorMessage.Equals(ResourceMessagesExceptions.VALIDATION_COOK_TIME_INVALID));
        });
    }


    [Fact]
    public void Validate_Error_When_ThereIsNoIngredient()
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.Ingredients = [];

        var validator = new RecipeValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error =>
                error.ErrorMessage.Equals(ResourceMessagesExceptions.VALIDATION_AT_LEAST_ONE_INGREDIENT));
        });
    }


    [Fact]
    public void Validate_Error_When_IngredientEmpty()
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.Ingredients =
        [
            new RequestRecipeIngredientJson() { Item = string.Empty }
        ];

        var validator = new RecipeValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error =>
                error.ErrorMessage.Equals(ResourceMessagesExceptions.VALIDATION_INGREDIENT_EMPTY));
        });
    }


    [Fact]
    public void Validate_Error_When_IngredientMaxLength()
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.Ingredients =
        [
            new RequestRecipeIngredientJson() { Item = new string('e', 251) }
        ];

        var validator = new RecipeValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error =>
                error.ErrorMessage.Equals(ResourceMessagesExceptions.VALIDATION_INGREDIENT_MAX_LENGTH));
        });
    }

    [Fact]
    public void Validate_Error_When_ThereIsNotInstruction()
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.Instructions = [];

        var validator = new RecipeValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error =>
                error.ErrorMessage.Equals(ResourceMessagesExceptions.VALIDATION_AT_LEAST_ONE_INSTRUCTION));
        });
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_Error_When_InstructionOrderIsInvalid(int order)
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.Instructions =
        [
            new RequestRecipeInstructionJson()
            {
                Order = order,
                Description = "Valid Description"
            }
        ];

        var validator = new RecipeValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error =>
                error.ErrorMessage.Equals(ResourceMessagesExceptions.VALIDATION_INSTRUCTION_ORDER_INVALID));
        });
    }

    [Fact]
    public void Validate_Error_When_InstructionsHaveDescriptionEmpty()
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.Instructions =
        [
            new RequestRecipeInstructionJson { Order = 1, Description = string.Empty },
        ];

        var validator = new RecipeValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error =>
                error.ErrorMessage.Equals(ResourceMessagesExceptions.VALIDATION_INSTRUCTION_DESCRIPTION_REQUIRED));
        });
    }

    [Fact]
    public void Validate_Error_When_InstructionDescriptionMaxLength()
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.Instructions =
        [
            new RequestRecipeInstructionJson { Order = 1, Description = new string('e', 2001)},
        ];

        var validator = new RecipeValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error =>
                error.ErrorMessage.Equals(ResourceMessagesExceptions.VALIDATION_INSTRUCTION_DESCRIPTION_MAX_LENGTH));
        });
    }
    
    [Fact]
    public void Validate_Error_When_InstructionsHaveDuplicateOrder()
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.Instructions =
        [
            new RequestRecipeInstructionJson { Order = 1, Description = "First" },
            new RequestRecipeInstructionJson { Order = 1, Description = "Second" },
        ];

        var validator = new RecipeValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error =>
                error.ErrorMessage.Equals(ResourceMessagesExceptions.VALIDATION_INSTRUCTION_ORDER_DUPLICATED));
        });
    }
}