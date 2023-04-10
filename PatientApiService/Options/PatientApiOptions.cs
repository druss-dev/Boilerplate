using System.ComponentModel.DataAnnotations;

namespace PatientApiService.Options;

public class PatientApiOptions
{
    public const string PatientApi = nameof(PatientApi);
    
    [Required(AllowEmptyStrings = false)] 
    public string BaseUrl { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false)] 
    public string Username { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false)] 
    public string Password { get; set; } = string.Empty;
}