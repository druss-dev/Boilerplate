using System.ComponentModel.DataAnnotations;

namespace Boilerplate.Options;

// secrets for azure encryption configuration
public class AzureOptions
{
    public const string Azure = nameof(Azure);

    [Required(AllowEmptyStrings = false)]
    public string? CmkClientId { get; set; }

    [Required(AllowEmptyStrings = false)] 
    public string? CmkClientSecret { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string? CmkTenantId { get; set; }
}