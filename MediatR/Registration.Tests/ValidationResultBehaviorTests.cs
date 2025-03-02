using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Xunit;

namespace Registration.Tests;

public class ValidationResultBehaviorTests
{
    // Test request class
    private class TestRequest : IRequest<Result<string>>
    {
        public string? Name { get; set; }
    }

    // Test request class that returns non-Result type
    private class TestRequestNonResult : IRequest<string>
    {
        public string? Name { get; set; }
    }

    // Test validator
    private class TestValidator : AbstractValidator<TestRequest>
    {
        public TestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
        }
    }

    // Test validator for non-Result request
    private class TestValidatorNonResult : AbstractValidator<TestRequestNonResult>
    {
        public TestValidatorNonResult()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
        }
    }

    [Fact]
    public async Task Handle_WhenValidationPasses_ShouldCallNextDelegate()
    {
        // Arrange
        var validators = new List<IValidator<TestRequest>> { new TestValidator() };
        var behavior = new ValidationResultBehavior<TestRequest, Result<string>>(validators);
        var request = new TestRequest { Name = "Test" };
        var expectedResult = Result.Ok("Success");
        
        async Task<Result<string>> Next() => expectedResult;

        // Act
        var result = await behavior.Handle(request, Next, CancellationToken.None);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnFailResult()
    {
        // Arrange
        var validators = new List<IValidator<TestRequest>> { new TestValidator() };
        var behavior = new ValidationResultBehavior<TestRequest, Result<string>>(validators);
        var request = new TestRequest { Name = null };
        
        async Task<Result<string>> Next() => Result.Ok("Success");

        // Act
        var result = await behavior.Handle(request, Next, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Single(result.Errors);
        var error = result.Errors[0];
        Assert.IsType<ValidationErrors>(error);
        var validationErrors = (ValidationErrors)error;
        Assert.Single(validationErrors.Errors);
        Assert.Equal("Name is required", validationErrors.Errors.First().ErrorMessage);
    }

    [Fact]
    public async Task Handle_WhenNoValidators_ShouldCallNextDelegate()
    {
        // Arrange
        var validators = new List<IValidator<TestRequest>>();
        var behavior = new ValidationResultBehavior<TestRequest, Result<string>>(validators);
        var request = new TestRequest { Name = null };
        var expectedResult = Result.Ok("Success");
        
        async Task<Result<string>> Next() => expectedResult;

        // Act
        var result = await behavior.Handle(request, Next, CancellationToken.None);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task Handle_WhenValidationFailsWithNonResultType_ShouldThrowValidationException()
    {
        // Arrange
        var validators = new List<IValidator<TestRequestNonResult>> { new TestValidatorNonResult() };
        var behavior = new ValidationResultBehavior<TestRequestNonResult, string>(validators);
        var request = new TestRequestNonResult { Name = null };
        
        async Task<string> Next() => "Success";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() => 
            behavior.Handle(request, Next, CancellationToken.None));
        
        Assert.Single(exception.Errors);
        Assert.Equal("Name is required", exception.Errors.First().ErrorMessage);
    }

    [Fact]
    public async Task Handle_WithMultipleValidators_ShouldCollectAllErrors()
    {
        // Arrange
        var validator1 = new TestValidator();
        var validator2 = new InlineValidator<TestRequest>();
        validator2.RuleFor(x => x.Name).Must(n => n != "Test").WithMessage("Name cannot be Test");
        
        var validators = new List<IValidator<TestRequest>> { validator1, validator2 };
        var behavior = new ValidationResultBehavior<TestRequest, Result<string>>(validators);
        var request = new TestRequest { Name = null };
        
        static Task<Result<string>> Next() => Task.FromResult(Result.Ok("Success"));

        // Act
        var result = await behavior.Handle(request, Next, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Single(result.Errors);
        var error = result.Errors[0];
        Assert.IsType<ValidationErrors>(error);
        var validationErrors = (ValidationErrors)error;
        // We expect two validation errors here:
        // 1. "Name is required" from validator1
        // 2. "Name cannot be Test" from validator2
        Assert.Equal(2, validationErrors.Errors.Count());
        Assert.Contains(validationErrors.Errors, e => e.ErrorMessage == "Name is required");
        Assert.Contains(validationErrors.Errors, e => e.ErrorMessage == "Name cannot be Test");
    }

    [Fact]
    public async Task Handle_WithDifferentResultGenericType_ShouldReturnCorrectResultType()
    {
        // Arrange
        var validators = new List<IValidator<TestRequest>> { new TestValidator() };
        var behavior = new ValidationResultBehavior<TestRequest, Result<int>>(validators);
        var request = new TestRequest { Name = null };
        
        async Task<Result<int>> Next() => Result.Ok(42);

        // Act
        var result = await behavior.Handle(request, Next, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Single(result.Errors);
        Assert.IsType<Result<int>>(result);
    }
} 