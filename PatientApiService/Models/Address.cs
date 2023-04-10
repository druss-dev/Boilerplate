namespace PatientApiService.Models;

public class Address
{
    public long Id { get; set; }
    public string? Street1 { get; set; } 
    public string? Street2 { get; set; } 
    public string? City { get; set; } 
    public string? State { get; set; } 
    public string? Country { get; set; } 
    public string? Zip { get; set; } 
    public bool IsPrimary { get; set; } 
    public DateTime ModifiedOn { get; set; }
}