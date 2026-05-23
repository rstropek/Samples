using DataAccess;
using FluentResults;
using FluentValidation;
using MediatR;

namespace Registration;

public record GetCampaign(Guid CampaignId) : IRequest<Result<Campaign>>;

public class GetCampaignValidator : AbstractValidator<GetCampaign>
{
    public GetCampaignValidator()
    {
        RuleFor(x => x.CampaignId).MustBeValidCampaignId();
    }
}

public class GetCampaignHandler(IJsonFileRepository repository) : IRequestHandler<GetCampaign, Result<Campaign>>
{
    public async Task<Result<Campaign>> Handle(GetCampaign getCampaign, CancellationToken cancellationToken)
    {
        var campaignId = getCampaign.CampaignId;

        await using var campaignStream = await repository.Open(campaignId.ToString("N"), true);
        if (campaignStream is null)
        {
            return Result.Fail(new NotFound($"Campaign with ID {campaignId} not found"));
        }

        var campaign = await repository.Get<Campaign>(campaignStream);
        if (campaign is null) { return Result.Fail(new NotFound($"Could not read campaign with id {campaignId} from repository")); }

        return Result.Ok(campaign);
    }
}
