using System.ComponentModel.DataAnnotations;

namespace MinimalApiAot;

public class AppFeatures
{
    public bool ProvideMasterdata { get; set; } = true;
}

public class GroupLimits
{
    [Required]
    [Range(2, 50)]
    public int MaximumGroupSizeRegular { get; set; }

    [Required]
    [Range(2, 50)]
    public int MaximumGroupSizeSchool { get; set; }

}

// Options validator implement validation logic for configuration options.
// To reduce startup overhead, a source generate is available.
// Read more at https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8#options-validation
//
// Note that the OptionsValidator attribute does not work in RC 1.
// At the time of writing, you need the nightly version of 
// Microsoft.Extensions.Options. However, it will be fixed in RC 2.
[OptionsValidator]
public partial class GroupLimitsValidator : IValidateOptions<GroupLimits> { }
