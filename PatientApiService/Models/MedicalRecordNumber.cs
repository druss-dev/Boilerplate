namespace PatientApiService.Models;

public class MedicalRecordNumber
{
    public long Id { get; set; }
    public string? Source { get; set; }
    public string? Value { get; set; }
    public bool IsPrimary { get; set; }
    public DateTime ModifiedOn { get; set; }
}