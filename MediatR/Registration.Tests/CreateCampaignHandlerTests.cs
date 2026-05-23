using DataAccess;
using FluentResults;
using MediatR;
using Moq;

namespace Registration.Tests;

public class CreateCampaignHandlerTests
{
    private readonly Mock<IJsonFileRepository> _mockRepository;

    private readonly Mock<IMediator> _mockMediator;
    
    private readonly CreateCampaignHandler _handler;

    public CreateCampaignHandlerTests()
    {
        _mockRepository = new Mock<IJsonFileRepository>();
        _mockMediator = new Mock<IMediator>();
        _handler = new CreateCampaignHandler(_mockRepository.Object, _mockMediator.Object);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldCreateCampaign()
    {
        // Arrange
        var request = new CreateCampaignRequest(
            Name: "Test Campaign",
            Organizer: "Test Organizer"
        );
        var command = new CreateCampaign(request);
        
        Item createdItem = new("test-id", "test-path");
        _mockRepository.Setup(r => r.Create(It.IsAny<string>(), It.IsAny<Campaign>()))
            .ReturnsAsync(createdItem);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
        _mockRepository.Verify(r => r.Create(It.IsAny<string>(), It.IsAny<Campaign>()), Times.Once);
        _mockMediator.Verify(m => m.Publish(It.IsAny<CampaignChangedNotification>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithDates_ShouldCreateCampaignWithDates()
    {
        // Arrange
        var departmentAssignment = new DepartmentAssignmentRequest(
            DepartmentName: "Test Department",
            NumberOfSeats: 10,
            ReservedRatioForGirls: 0.5m
        );

        var dateRequest = new CreateDateRequest(
            Date: DateOnly.FromDateTime(DateTime.Now.AddDays(7)),
            DepartmentAssignments: new[] { departmentAssignment },
            StartTime: new TimeOnly(9, 0),
            EndTime: new TimeOnly(17, 0)
        );

        var request = new CreateCampaignRequest(
            Name: "Test Campaign",
            Organizer: "Test Organizer",
            Dates: new[] { dateRequest },
            ReservedRatioForGirls: 0.3m,
            PurgeDate: DateOnly.FromDateTime(DateTime.Now.AddMonths(1))
        );
        
        var command = new CreateCampaign(request);
        
        Campaign? capturedCampaign = null;
        _mockRepository.Setup(r => r.Create(It.IsAny<string>(), It.IsAny<Campaign>()))
            .Callback<string, Campaign>((_, campaign) => capturedCampaign = campaign)
            .ReturnsAsync(new Item("test-id", "test-path"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(capturedCampaign);
        Assert.Equal(request.Name, capturedCampaign!.Name);
        Assert.Equal(request.Organizer, capturedCampaign.Organizer);
        Assert.Equal(request.ReservedRatioForGirls, capturedCampaign.ReservedRatioForGirls);
        Assert.Equal(request.PurgeDate, capturedCampaign.PurgeDate);
        Assert.Equal(CampaignStatus.Inactive, capturedCampaign.Status);
        
        // Verify dates
        Assert.Single(capturedCampaign.Dates);
        var capturedDate = capturedCampaign.Dates[0];
        Assert.Equal(dateRequest.Date, capturedDate.Date);
        Assert.Equal(dateRequest.StartTime, capturedDate.StartTime);
        Assert.Equal(dateRequest.EndTime, capturedDate.EndTime);
        Assert.Equal(CampaignDateStatus.Active, capturedDate.Status);
        
        // Verify department assignments
        Assert.Single(capturedDate.DepartmentAssignments);
        var capturedAssignment = capturedDate.DepartmentAssignments[0];
        Assert.Equal(departmentAssignment.DepartmentName, capturedAssignment.DepartmentName);
        Assert.Equal(departmentAssignment.NumberOfSeats, capturedAssignment.NumberOfSeats);
        Assert.Equal(departmentAssignment.ReservedRatioForGirls, capturedAssignment.ReservedRatioForGirls);
    }

    [Fact]
    public async Task Handle_WithNullDates_ShouldCreateCampaignWithEmptyDates()
    {
        // Arrange
        var request = new CreateCampaignRequest(
            Name: "Test Campaign",
            Organizer: "Test Organizer",
            Dates: null
        );
        
        var command = new CreateCampaign(request);
        
        Campaign? capturedCampaign = null;
        _mockRepository.Setup(r => r.Create(It.IsAny<string>(), It.IsAny<Campaign>()))
            .Callback<string, Campaign>((_, campaign) => capturedCampaign = campaign)
            .ReturnsAsync(new Item("test-id", "test-path"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(capturedCampaign);
        Assert.Empty(capturedCampaign!.Dates);
    }

    [Fact]
    public async Task Handle_WithNullDepartmentAssignments_ShouldCreateCampaignWithEmptyDepartmentAssignments()
    {
        // Arrange
        var dateRequest = new CreateDateRequest(
            Date: DateOnly.FromDateTime(DateTime.Now.AddDays(7)),
            DepartmentAssignments: null
        );

        var request = new CreateCampaignRequest(
            Name: "Test Campaign",
            Organizer: "Test Organizer",
            Dates: new[] { dateRequest }
        );
        
        var command = new CreateCampaign(request);
        
        Campaign? capturedCampaign = null;
        _mockRepository.Setup(r => r.Create(It.IsAny<string>(), It.IsAny<Campaign>()))
            .Callback<string, Campaign>((_, campaign) => capturedCampaign = campaign)
            .ReturnsAsync(new Item("test-id", "test-path"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(capturedCampaign);
        Assert.Single(capturedCampaign!.Dates);
        Assert.Empty(capturedCampaign.Dates[0].DepartmentAssignments);
    }

    [Fact]
    public async Task Handle_ShouldSetCreatedAndUpdatedTimestamps()
    {
        // Arrange
        var request = new CreateCampaignRequest(
            Name: "Test Campaign",
            Organizer: "Test Organizer"
        );
        
        var command = new CreateCampaign(request);
        
        Campaign? capturedCampaign = null;
        _mockRepository.Setup(r => r.Create(It.IsAny<string>(), It.IsAny<Campaign>()))
            .Callback<string, Campaign>((_, campaign) => capturedCampaign = campaign)
            .ReturnsAsync(new Item("test-id", "test-path"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(capturedCampaign);
        
        // Timestamps should be set to approximately now
        var now = DateTimeOffset.UtcNow;
        var fiveSecondsAgo = now.AddSeconds(-5);
        
        Assert.True(capturedCampaign!.CreatedAt >= fiveSecondsAgo);
        Assert.True(capturedCampaign.CreatedAt <= now);
        
        Assert.True(capturedCampaign.UpdatedAt >= fiveSecondsAgo);
        Assert.True(capturedCampaign.UpdatedAt <= now);
    }

    [Fact]
    public async Task Handle_ShouldUseIdStringForRepositoryKey()
    {
        // Arrange
        var request = new CreateCampaignRequest(
            Name: "Test Campaign",
            Organizer: "Test Organizer"
        );
        
        var command = new CreateCampaign(request);
        
        string? capturedId = null;
        Campaign? capturedCampaign = null;
        
        _mockRepository.Setup(r => r.Create(It.IsAny<string>(), It.IsAny<Campaign>()))
            .Callback<string, Campaign>((id, campaign) => 
            {
                capturedId = id;
                capturedCampaign = campaign;
            })
            .ReturnsAsync(new Item("test-id", "test-path"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(capturedCampaign);
        Assert.NotNull(capturedId);
        
        // The ID string used for the repository should match the campaign's IdString property
        Assert.Equal(capturedCampaign!.IdString, capturedId);
    }
} 