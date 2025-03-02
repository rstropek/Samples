using FluentValidation;

namespace Registration.Tests;

public class ValidationHelpersTests
{
    private class TestModel
    {
        public decimal? ReservedRatioForGirls { get; set; }
        public DateOnly? PurgeDate { get; set; }
        public List<DateOnly>? Dates { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public string? DepartmentName { get; set; }
        public short NumberOfSeats { get; set; }
        public Guid CampaignId { get; set; }
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    public void MustBeValidReservedRatioForGirls_WhenOutOfRange_ValidationFails(decimal value)
    {
        // Arrange
        var validator = new InlineValidator<TestModel>();
        validator.RuleFor(x => x.ReservedRatioForGirls).MustBeValidReservedRatioForGirls();
        var model = new TestModel { ReservedRatioForGirls = value };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.ErrorMessage == "Reserved ratio for girls must be null or between 0 and 1");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(0.5)]
    [InlineData(1)]
    public void MustBeValidReservedRatioForGirls_WhenInRange_ValidationPasses(decimal value)
    {
        // Arrange
        var validator = new InlineValidator<TestModel>();
        validator.RuleFor(x => x.ReservedRatioForGirls).MustBeValidReservedRatioForGirls();
        var model = new TestModel { ReservedRatioForGirls = value };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void MustBeValidReservedRatioForGirls_WhenNull_ValidationPasses()
    {
        // Arrange
        var validator = new InlineValidator<TestModel>();
        validator.RuleFor(x => x.ReservedRatioForGirls).MustBeValidReservedRatioForGirls();
        var model = new TestModel { ReservedRatioForGirls = null };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void MustBeFuturePurgeDate_WhenInPast_ValidationFails()
    {
        // Arrange
        var validator = new InlineValidator<TestModel>();
        validator.RuleFor(x => x.PurgeDate).MustBeFuturePurgeDate();
        var model = new TestModel { PurgeDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)) };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.ErrorMessage == "Purge date must be in the future");
    }

    [Fact]
    public void MustBeFuturePurgeDate_WhenInFuture_ValidationPasses()
    {
        // Arrange
        var validator = new InlineValidator<TestModel>();
        validator.RuleFor(x => x.PurgeDate).MustBeFuturePurgeDate();
        var model = new TestModel { PurgeDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)) };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void MustBeFuturePurgeDate_WhenNull_ValidationPasses()
    {
        // Arrange
        var validator = new InlineValidator<TestModel>();
        validator.RuleFor(x => x.PurgeDate).MustBeFuturePurgeDate();
        var model = new TestModel { PurgeDate = null };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void PurgeDateMustBeAfterAllDates_WhenPurgeDateBeforeDates_ValidationFails()
    {
        // Arrange
        var validator = new InlineValidator<TestModel>();
        validator.RuleFor(x => x.PurgeDate).PurgeDateMustBeAfterAllDates(x => x.Dates);
        
        var futureDate = DateOnly.FromDateTime(DateTime.Now.AddDays(5));
        var model = new TestModel 
        { 
            PurgeDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
            Dates = [futureDate]
        };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.ErrorMessage == "Purge date must be after all other dates");
    }

    [Fact]
    public void PurgeDateMustBeAfterAllDates_WhenPurgeDateAfterDates_ValidationPasses()
    {
        // Arrange
        var validator = new InlineValidator<TestModel>();
        validator.RuleFor(x => x.PurgeDate).PurgeDateMustBeAfterAllDates(x => x.Dates);
        
        var pastDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2));
        var model = new TestModel 
        { 
            PurgeDate = DateOnly.FromDateTime(DateTime.Now.AddDays(5)),
            Dates = [pastDate]
        };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void PurgeDateMustBeAfterAllDates_WhenDatesNull_ValidationPasses()
    {
        // Arrange
        var validator = new InlineValidator<TestModel>();
        validator.RuleFor(x => x.PurgeDate).PurgeDateMustBeAfterAllDates(x => x.Dates);
        var model = new TestModel 
        { 
            PurgeDate = DateOnly.FromDateTime(DateTime.Now.AddDays(5)),
            Dates = null
        };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void PurgeDateMustBeAfterAllDates_WhenPurgeDateNull_ValidationPasses()
    {
        // Arrange
        var validator = new InlineValidator<TestModel>();
        validator.RuleFor(x => x.PurgeDate).PurgeDateMustBeAfterAllDates(x => x.Dates);
        var model = new TestModel 
        { 
            PurgeDate = null,
            Dates = [DateOnly.FromDateTime(DateTime.Now.AddDays(2))]
        };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void MustNotHaveDuplicateDates_WhenDuplicatesExist_ValidationFails()
    {
        // Arrange
        var validator = new InlineValidator<TestModel>();
        validator.RuleFor(x => x.Dates).MustNotHaveDuplicateDates<TestModel, List<DateOnly>?>(x => x);
        
        var date = DateOnly.FromDateTime(DateTime.Now.AddDays(2));
        var model = new TestModel 
        { 
            Dates = [date, date]
        };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.ErrorMessage == "Duplicate dates found");
    }

    [Fact]
    public void MustNotHaveDuplicateDates_WhenNoDuplicates_ValidationPasses()
    {
        // Arrange
        var validator = new InlineValidator<TestModel>();
        validator.RuleFor(x => x.Dates).MustNotHaveDuplicateDates<TestModel, List<DateOnly>?>(x => x);
        
        var model = new TestModel 
        { 
            Dates =
            [
                DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
                DateOnly.FromDateTime(DateTime.Now.AddDays(3))
            ]
        };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void MustNotHaveDuplicateDates_WhenNull_ValidationPasses()
    {
        // Arrange
        var validator = new InlineValidator<TestModel>();
        validator.RuleFor(x => x.Dates).MustNotHaveDuplicateDates<TestModel, List<DateOnly>?>(x => x);
        var model = new TestModel { Dates = null };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.True(result.IsValid);
    }

    // Helper classes for non-nullable DateOnly test
    private class InnerModel
    {
        public DateOnly Date { get; set; }
    }

    [Fact]
    public void MustBeFutureDate_WhenInPast_ValidationFails()
    {
        // Arrange
        var validator = new InlineValidator<InnerModel>();
        validator.RuleFor(x => x.Date).MustBeFutureDate();
        var model = new InnerModel { Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)) };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.ErrorMessage == "Date must be in the future");
    }

    [Fact]
    public void MustBeFutureDate_WhenInFuture_ValidationPasses()
    {
        // Arrange
        var validator = new InlineValidator<InnerModel>();
        validator.RuleFor(x => x.Date).MustBeFutureDate();
        var model = new InnerModel { Date = DateOnly.FromDateTime(DateTime.Now.AddDays(1)) };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void MustBeBefore_WhenStartTimeAfterEndTime_ValidationFails()
    {
        // Arrange
        var validator = new InlineValidator<TestModel>();
        validator.RuleFor(x => x.StartTime).MustBeBefore(x => x.EndTime);
        
        var model = new TestModel 
        { 
            StartTime = new TimeOnly(14, 0),
            EndTime = new TimeOnly(13, 0)
        };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.ErrorMessage == "Date must be before end date");
    }

    [Fact]
    public void MustBeBefore_WhenStartTimeBeforeEndTime_ValidationPasses()
    {
        // Arrange
        var validator = new InlineValidator<TestModel>();
        validator.RuleFor(x => x.StartTime).MustBeBefore(x => x.EndTime);
        
        var model = new TestModel 
        { 
            StartTime = new TimeOnly(13, 0),
            EndTime = new TimeOnly(14, 0)
        };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void MustBeBefore_WhenStartTimeNull_ValidationPasses()
    {
        // Arrange
        var validator = new InlineValidator<TestModel>();
        validator.RuleFor(x => x.StartTime).MustBeBefore(x => x.EndTime);
        
        var model = new TestModel 
        { 
            StartTime = null,
            EndTime = new TimeOnly(14, 0)
        };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void MustBeBefore_WhenEndTimeNull_ValidationPasses()
    {
        // Arrange
        var validator = new InlineValidator<TestModel>();
        validator.RuleFor(x => x.StartTime).MustBeBefore(x => x.EndTime);
        
        var model = new TestModel 
        { 
            StartTime = new TimeOnly(13, 0),
            EndTime = null
        };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void MustBeValidNumberOfSeats_WhenZeroOrNegative_ValidationFails(short value)
    {
        // Arrange
        var validator = new InlineValidator<TestModel>();
        validator.RuleFor(x => x.NumberOfSeats).MustBeValidNumberOfSeats();
        var model = new TestModel { NumberOfSeats = value };
        
        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.ErrorMessage == "Number of seats must be greater than 0");
    }

    [Fact]
    public void MustBeValidNumberOfSeats_WhenPositive_ValidationPasses()
    {
        // Arrange
        var validator = new InlineValidator<TestModel>();
        validator.RuleFor(x => x.NumberOfSeats).MustBeValidNumberOfSeats();
        var model = new TestModel { NumberOfSeats = 10 };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void MustBeValidCampaignId_WhenEmpty_ValidationFails()
    {
        // Arrange
        var validator = new InlineValidator<TestModel>();
        validator.RuleFor(x => x.CampaignId).MustBeValidCampaignId();
        var model = new TestModel { CampaignId = Guid.Empty };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.ErrorMessage == "Campaign ID must be set");
    }

    [Fact]
    public void MustBeValidCampaignId_WhenNotEmpty_ValidationPasses()
    {
        // Arrange
        var validator = new InlineValidator<TestModel>();
        validator.RuleFor(x => x.CampaignId).MustBeValidCampaignId();
        var model = new TestModel { CampaignId = Guid.NewGuid() };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.True(result.IsValid);
    }
} 