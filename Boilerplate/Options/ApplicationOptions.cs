using System.ComponentModel.DataAnnotations;

namespace Boilerplate.Options;

// secrets specific to this application
public class ApplicationOptions
{
    public const string Application = nameof(Application);
    
    
    [Required(AllowEmptyStrings = false)]
    public string? CaerusApiBaseUrl { get; set; }
    
    [Required(AllowEmptyStrings = false)]
    public string? CaerusClientUsername { get; set; }
    
    [Required(AllowEmptyStrings = false)]
    public string? CaerusClientPassword { get; set; }
    
    [Required(AllowEmptyStrings = false)]
    public string InternalApiBaseUrl { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false)]
    public string InternalServiceUserName { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false)]
    public string InternalServicePassword { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false)]
    public string? Name { get; set; }
}