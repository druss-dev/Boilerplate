using System.ComponentModel.DataAnnotations;

namespace Boilerplate.Options;

// secrets specific to this application
public class ApplicationOptions
{
    public const string Application = nameof(Application);
    
    [Required(AllowEmptyStrings = false)]
    public string? Name { get; set; }
}