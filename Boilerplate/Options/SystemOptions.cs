using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;

namespace Boilerplate.Options;

public class SystemOptions
{
    [ConfigurationKeyName("DOTNET_ENVIRONMENT")]
    [Required(AllowEmptyStrings = false)]
    public string? Environment { get; set; }

    [ConfigurationKeyName("PATIENT_API_SQL_CONNECTION_STRING")]
    [Required(AllowEmptyStrings = false)]
    public string? PatientSqlConnectionString { get; set; }
    
    [ConfigurationKeyName("PATIENT_API_SQL_MASTER_CONNECTION_STRING")]
    [Required(AllowEmptyStrings = false)]
    public string? PatientSqlMasterConnectionString { get; set; }
    
    [ConfigurationKeyName("SQL_CONNECTION_STRING")]
    [Required(AllowEmptyStrings = false)]
    public string? SqlConnectionString { get; set; }
    
    [ConfigurationKeyName("SQL_MASTER_CONNECTION_STRING")]
    [Required(AllowEmptyStrings = false)]
    public string? SqlMasterConnectionString { get; set; }
}