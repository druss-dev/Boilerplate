namespace PatientApiService.Models;

public class PhoneNumber
{
    public long Id { get; set; }
    public string? Phone { get; set; }
    public bool CanCall { get; set; }
    public bool CanText { get; set; }
    public bool IsPrimary { get; set; }
    public DateTime ModifiedOn { get; set; }
}