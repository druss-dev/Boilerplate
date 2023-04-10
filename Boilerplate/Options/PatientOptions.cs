using System.ComponentModel.DataAnnotations;

namespace Boilerplate.Options;

public class PatientOptions
{
    public const string Patient = nameof(Patient);

    [Required(AllowEmptyStrings = false)]
    public string? BaseUrl { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string? Password { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string? Username { get; set; }
}