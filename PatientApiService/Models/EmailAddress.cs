namespace PatientApiService.Models;

public class EmailAddress
{
    public long Id { get; set; }
    public string? Email { get; set; }
    public bool IsPrimary { get; set; }
    public DateTime ModifiedOn { get; set; }
}