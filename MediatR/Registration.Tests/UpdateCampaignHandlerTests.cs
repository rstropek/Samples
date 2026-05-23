using System.Text.Json;
using DataAccess;
using MediatR;
using Moq;

namespace Registration.Tests;

public class UpdateCampaignHandlerTests
{
    private readonly Mock<IJsonFileRepository> _mockRepository;

    private readonly Mock<IMediator> _mockMediator;
    
    private readonly UpdateCampaignHandler _handler;

    public UpdateCampaignHandlerTests()
    {
        _mockRepository = new Mock<IJsonFileRepository>();
        _mockMediator = new Mock<IMediator>();
        _handler = new UpdateCampaignHandler(_mockRepository.Object, _mockMediator.Object);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldUpdateCampaign()
    {
        // Arrange
        var campaignId = Guid.NewGuid();
        var originalCampaign = new Campaign
        {
            Id = campaignId,
            Name = "Original Name",
            Organizer = "Original Organizer",
            Status = CampaignStatus.Inactive,
            UpdatedAt = DateTimeOffset.UtcNow.AddMinutes(-10)
        };

        var updateRequest = new UpdateCampaignRequest(
            Name: "Updated Name",
            Organizer: "Updated Organizer",
            UpdatedAt: originalCampaign.UpdatedAt
        );

        var command = new UpdateCampaign(campaignId, updateRequest);

        var mockStream = new Mock<Stream>();
        Campaign? capturedCampaign = null;

        _mockRepository.Setup(r => r.Open(It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(mockStream.Object);

        _mockRepository.Setup(r => r.Get<Campaign>(mockStream.Object))
            .ReturnsAsync(JsonSerializer.Deserialize<Campaign>(JsonSerializer.Serialize(originalCampaign)));

        _mockRepository.Setup(r => r.Update(mockStream.Object, It.IsAny<Campaign>()))
            .Callback<Stream, Campaign>((_, campaign) => capturedCampaign = campaign);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(capturedCampaign);
        Assert.Equal(updateRequest.Name, capturedCampaign!.Name);
        Assert.Equal(updateRequest.Organizer, capturedCampaign.Organizer);
        Assert.Equal(originalCampaign.Status, capturedCampaign.Status);
        Assert.True(capturedCampaign.UpdatedAt > originalCampaign.UpdatedAt);
        
        _mockRepository.Verify(r => r.Update(mockStream.Object, It.IsAny<Campaign>()), Times.Once);
        _mockMediator.Verify(m => m.Publish(It.IsAny<CampaignChangedNotification>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentCampaign_ShouldReturnNotFound()
    {
        // Arrange
        var campaignId = Guid.NewGuid();
        var updateRequest = new UpdateCampaignRequest(
            Name: "Updated Name",
            Organizer: "Updated Organizer"
        );

        var command = new UpdateCampaign(campaignId, updateRequest);

        _mockRepository.Setup(r => r.Open(campaignId.ToString("N"), true))
            .ReturnsAsync((Stream?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Single(result.Errors);
        Assert.IsType<NotFound>(result.Errors[0]);
        Assert.Contains(campaignId.ToString(), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_WithConcurrencyConflict_ShouldReturnConcurrencyError()
    {
        // Arrange
        var campaignId = Guid.NewGuid();
        var originalCampaign = new Campaign
        {
            Id = campaignId,
            Name = "Original Name",
            Organizer = "Original Organizer",
            Status = CampaignStatus.Inactive,
            UpdatedAt = DateTimeOffset.UtcNow.AddMinutes(-10)
        };

        var updateRequest = new UpdateCampaignRequest(
            Name: "Updated Name",
            Organizer: "Updated Organizer",
            UpdatedAt: DateTimeOffset.UtcNow.AddMinutes(-20) // Different timestamp than the original
        );

        var command = new UpdateCampaign(campaignId, updateRequest);

        var mockStream = new Mock<Stream>();

        _mockRepository.Setup(r => r.Open(campaignId.ToString("N"), true))
            .ReturnsAsync(mockStream.Object);

        _mockRepository.Setup(r => r.Get<Campaign>(mockStream.Object))
            .ReturnsAsync(JsonSerializer.Deserialize<Campaign>(JsonSerializer.Serialize(originalCampaign)));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Single(result.Errors);
        Assert.IsType<Concurrency>(result.Errors[0]);
        Assert.Contains("concurrency", result.Errors[0].Message.ToLower());
        
        _mockRepository.Verify(r => r.Update(It.IsAny<FileStream>(), It.IsAny<Campaign>()), Times.Never);
    }

    [Fact]
    public async Task Handle_AttemptingToActivateInactiveCampaign_ShouldReturnForbidden()
    {
        // Arrange
        var campaignId = Guid.NewGuid();
        var originalCampaign = new Campaign
        {
            Id = campaignId,
            Name = "Original Name",
            Organizer = "Original Organizer",
            Status = CampaignStatus.Inactive,
            UpdatedAt = DateTimeOffset.UtcNow.AddMinutes(-10)
        };

        var updateRequest = new UpdateCampaignRequest(
            Name: "Updated Name",
            Organizer: "Updated Organizer",
            Status: CampaignStatus.Active,
            UpdatedAt: originalCampaign.UpdatedAt
        );

        var command = new UpdateCampaign(campaignId, updateRequest);

        var mockStream = new Mock<Stream>();

        _mockRepository.Setup(r => r.Open(campaignId.ToString("N"), true))
            .ReturnsAsync(mockStream.Object);

        _mockRepository.Setup(r => r.Get<Campaign>(mockStream.Object))
            .ReturnsAsync(JsonSerializer.Deserialize<Campaign>(JsonSerializer.Serialize(originalCampaign)));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Single(result.Errors);
        Assert.IsType<Forbidden>(result.Errors[0]);
        Assert.Contains("activate", result.Errors[0].Message.ToLower());
        
        _mockRepository.Verify(r => r.Update(It.IsAny<FileStream>(), It.IsAny<Campaign>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithDates_ShouldUpdateCampaignDates()
    {
        // Arrange
        var campaignId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Now.AddDays(7));
        
        var originalCampaign = new Campaign
        {
            Id = campaignId,
            Name = "Original Name",
            Organizer = "Original Organizer",
            Status = CampaignStatus.Inactive,
            UpdatedAt = DateTimeOffset.UtcNow.AddMinutes(-10),
            Dates = []
        };

        var departmentAssignment = new UpdateDepartmentAssignmentRequest(
            DepartmentName: "Test Department",
            NumberOfSeats: 10,
            ReservedRatioForGirls: 0.5m
        );

        var dateRequest = new UpdateDateRequest(
            Date: date,
            DepartmentAssignments: [departmentAssignment],
            StartTime: new TimeOnly(9, 0),
            EndTime: new TimeOnly(17, 0),
            Status: CampaignDateStatus.Active
        );

        var updateRequest = new UpdateCampaignRequest(
            Name: "Updated Name",
            Organizer: "Updated Organizer",
            Dates: [dateRequest],
            UpdatedAt: originalCampaign.UpdatedAt
        );

        var command = new UpdateCampaign(campaignId, updateRequest);

        var mockStream = new Mock<Stream>();
        Campaign? capturedCampaign = null;

        _mockRepository.Setup(r => r.Open(campaignId.ToString("N"), true))
            .ReturnsAsync(mockStream.Object);

        _mockRepository.Setup(r => r.Get<Campaign>(mockStream.Object))
            .ReturnsAsync(JsonSerializer.Deserialize<Campaign>(JsonSerializer.Serialize(originalCampaign)));

        _mockRepository.Setup(r => r.Update(mockStream.Object, It.IsAny<Campaign>()))
            .Callback<Stream, Campaign>((_, campaign) => capturedCampaign = campaign);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(capturedCampaign);
        
        // Verify dates were added
        Assert.Single(capturedCampaign!.Dates);
        var capturedDate = capturedCampaign.Dates[0];
        Assert.Equal(dateRequest.Date, capturedDate.Date);
        Assert.Equal(dateRequest.StartTime, capturedDate.StartTime);
        Assert.Equal(dateRequest.EndTime, capturedDate.EndTime);
        Assert.Equal(dateRequest.Status, capturedDate.Status);
        
        // Verify department assignments
        Assert.Single(capturedDate.DepartmentAssignments);
        var capturedAssignment = capturedDate.DepartmentAssignments[0];
        Assert.Equal(departmentAssignment.DepartmentName, capturedAssignment.DepartmentName);
        Assert.Equal(departmentAssignment.NumberOfSeats, capturedAssignment.NumberOfSeats);
        Assert.Equal(departmentAssignment.ReservedRatioForGirls, capturedAssignment.ReservedRatioForGirls);
    }

    [Fact]
    public async Task Handle_WithExistingDates_ShouldUpdateExistingDates()
    {
        // Arrange
        var campaignId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Now.AddDays(7));
        
        var existingDepartmentAssignment = new DepartmentAssignment
        {
            DepartmentName = "Existing Department",
            NumberOfSeats = 5,
            ReservedRatioForGirls = 0.3m,
            Registrations = []
        };
        
        var existingDate = new CampaignDate
        {
            Date = date,
            StartTime = new TimeOnly(8, 0),
            EndTime = new TimeOnly(16, 0),
            Status = CampaignDateStatus.Hidden,
            DepartmentAssignments = [existingDepartmentAssignment]
        };
        
        var originalCampaign = new Campaign
        {
            Id = campaignId,
            Name = "Original Name",
            Organizer = "Original Organizer",
            Status = CampaignStatus.Inactive,
            UpdatedAt = DateTimeOffset.UtcNow.AddMinutes(-10),
            Dates = [existingDate]
        };

        var departmentAssignment = new UpdateDepartmentAssignmentRequest(
            DepartmentName: "Existing Department",
            NumberOfSeats: 10,
            ReservedRatioForGirls: 0.5m
        );

        var dateRequest = new UpdateDateRequest(
            Date: date,
            DepartmentAssignments: [departmentAssignment],
            StartTime: new TimeOnly(9, 0),
            EndTime: new TimeOnly(17, 0),
            Status: CampaignDateStatus.Active
        );

        var updateRequest = new UpdateCampaignRequest(
            Name: "Updated Name",
            Organizer: "Updated Organizer",
            Dates: new[] { dateRequest },
            UpdatedAt: originalCampaign.UpdatedAt
        );

        var command = new UpdateCampaign(campaignId, updateRequest);

        var mockStream = new Mock<Stream>();
        Campaign? capturedCampaign = null;

        _mockRepository.Setup(r => r.Open(campaignId.ToString("N"), true))
            .ReturnsAsync(mockStream.Object);

        _mockRepository.Setup(r => r.Get<Campaign>(mockStream.Object))
            .ReturnsAsync(JsonSerializer.Deserialize<Campaign>(JsonSerializer.Serialize(originalCampaign)));

        _mockRepository.Setup(r => r.Update(mockStream.Object, It.IsAny<Campaign>()))
            .Callback<Stream, Campaign>((_, campaign) => capturedCampaign = campaign);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(capturedCampaign);
        
        // Verify dates were updated
        Assert.Single(capturedCampaign!.Dates);
        var capturedDate = capturedCampaign.Dates[0];
        Assert.Equal(dateRequest.Date, capturedDate.Date);
        Assert.Equal(dateRequest.StartTime, capturedDate.StartTime);
        Assert.Equal(dateRequest.EndTime, capturedDate.EndTime);
        Assert.Equal(dateRequest.Status, capturedDate.Status);
        
        // Verify department assignments were updated
        Assert.Single(capturedDate.DepartmentAssignments);
        var capturedAssignment = capturedDate.DepartmentAssignments[0];
        Assert.Equal(departmentAssignment.DepartmentName, capturedAssignment.DepartmentName);
        Assert.Equal(departmentAssignment.NumberOfSeats, capturedAssignment.NumberOfSeats);
        Assert.Equal(departmentAssignment.ReservedRatioForGirls, capturedAssignment.ReservedRatioForGirls);
    }

    [Fact]
    public async Task Handle_RemovingDateWithRegistrations_ShouldReturnBadRequest()
    {
        // Arrange
        var campaignId = Guid.NewGuid();
        var date1 = DateOnly.FromDateTime(DateTime.Now.AddDays(7));
        var date2 = DateOnly.FromDateTime(DateTime.Now.AddDays(14));
        
        var departmentAssignment = new DepartmentAssignment
        {
            DepartmentName = "Test Department",
            NumberOfSeats = 5,
            ReservedRatioForGirls = 0.3m,
            Registrations = [new AttendeeRegistration
            {
                Id = "reg1",
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                CurrentSchool = "Test School",
                YearsOfSchooling = 12,
                Gender = Gender.Male
            }]
        };
        
        var existingDate1 = new CampaignDate
        {
            Date = date1,
            StartTime = new TimeOnly(8, 0),
            EndTime = new TimeOnly(16, 0),
            Status = CampaignDateStatus.Hidden,
            DepartmentAssignments = [departmentAssignment]
        };
        
        var existingDate2 = new CampaignDate
        {
            Date = date2,
            StartTime = new TimeOnly(8, 0),
            EndTime = new TimeOnly(16, 0),
            Status = CampaignDateStatus.Hidden,
            DepartmentAssignments = []
        };
        
        var originalCampaign = new Campaign
        {
            Id = campaignId,
            Name = "Original Name",
            Organizer = "Original Organizer",
            Status = CampaignStatus.Inactive,
            UpdatedAt = DateTimeOffset.UtcNow.AddMinutes(-10),
            Dates = [existingDate1, existingDate2]
        };

        // Only include date2 in the update, effectively removing date1 which has registrations
        var dateRequest = new UpdateDateRequest(
            Date: date2,
            StartTime: new TimeOnly(9, 0),
            EndTime: new TimeOnly(17, 0),
            Status: CampaignDateStatus.Active
        );

        var updateRequest = new UpdateCampaignRequest(
            Name: "Updated Name",
            Organizer: "Updated Organizer",
            Dates: [dateRequest],
            UpdatedAt: originalCampaign.UpdatedAt
        );

        var command = new UpdateCampaign(campaignId, updateRequest);

        var mockStream = new Mock<Stream>();

        _mockRepository.Setup(r => r.Open(campaignId.ToString("N"), true))
            .ReturnsAsync(mockStream.Object);

        _mockRepository.Setup(r => r.Get<Campaign>(mockStream.Object))
            .ReturnsAsync(JsonSerializer.Deserialize<Campaign>(JsonSerializer.Serialize(originalCampaign)));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Single(result.Errors);
        Assert.IsType<BadRequest>(result.Errors[0]);
        Assert.Contains("registrations", result.Errors[0].Message.ToLower());
        
        _mockRepository.Verify(r => r.Update(It.IsAny<FileStream>(), It.IsAny<Campaign>()), Times.Never);
    }

    [Fact]
    public async Task Handle_RemovingDepartmentWithRegistrations_ShouldReturnBadRequest()
    {
        // Arrange
        var campaignId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Now.AddDays(7));
        
        var departmentAssignment1 = new DepartmentAssignment
        {
            DepartmentName = "Department 1",
            NumberOfSeats = 5,
            ReservedRatioForGirls = 0.3m,
            Registrations = [new AttendeeRegistration
            {
                Id = "reg1",
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                CurrentSchool = "Test School",
                YearsOfSchooling = 12,
                Gender = Gender.Male
            }]
        };
        
        var departmentAssignment2 = new DepartmentAssignment
        {
            DepartmentName = "Department 2",
            NumberOfSeats = 5,
            ReservedRatioForGirls = 0.3m,
            Registrations = []
        };
        
        var existingDate = new CampaignDate
        {
            Date = date,
            StartTime = new TimeOnly(8, 0),
            EndTime = new TimeOnly(16, 0),
            Status = CampaignDateStatus.Hidden,
            DepartmentAssignments = [departmentAssignment1, departmentAssignment2]
        };
        
        var originalCampaign = new Campaign
        {
            Id = campaignId,
            Name = "Original Name",
            Organizer = "Original Organizer",
            Status = CampaignStatus.Inactive,
            UpdatedAt = DateTimeOffset.UtcNow.AddMinutes(-10),
            Dates = [existingDate]
        };

        // Only include Department 2 in the update, effectively removing Department 1 which has registrations
        var departmentRequest = new UpdateDepartmentAssignmentRequest(
            DepartmentName: "Department 2",
            NumberOfSeats: 10,
            ReservedRatioForGirls: 0.5m
        );

        var dateRequest = new UpdateDateRequest(
            Date: date,
            DepartmentAssignments: [departmentRequest],
            StartTime: new TimeOnly(9, 0),
            EndTime: new TimeOnly(17, 0),
            Status: CampaignDateStatus.Active
        );

        var updateRequest = new UpdateCampaignRequest(
            Name: "Updated Name",
            Organizer: "Updated Organizer",
            Dates: [dateRequest],
            UpdatedAt: originalCampaign.UpdatedAt
        );

        var command = new UpdateCampaign(campaignId, updateRequest);

        var mockStream = new Mock<Stream>();

        _mockRepository.Setup(r => r.Open(campaignId.ToString("N"), true))
            .ReturnsAsync(mockStream.Object);

        _mockRepository.Setup(r => r.Get<Campaign>(mockStream.Object))
            .ReturnsAsync(JsonSerializer.Deserialize<Campaign>(JsonSerializer.Serialize(originalCampaign)));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Single(result.Errors);
        Assert.IsType<BadRequest>(result.Errors[0]);
        Assert.Contains("registrations", result.Errors[0].Message.ToLower());
        
        _mockRepository.Verify(r => r.Update(It.IsAny<FileStream>(), It.IsAny<Campaign>()), Times.Never);
    }
} 