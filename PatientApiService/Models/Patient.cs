using System.Text.Json.Serialization;

namespace PatientApiService.Models;

public class Patient
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    [JsonPropertyName("Dob")]
    public DateTime DateOfBirth { get; set; }
    public string? Sex { get; set; }
    public string? Notes { get; set; }
    public string? User { get; set; }
    public List<MedicalRecordNumber> MedicalRecordNumbers { get; set; } = new();
    public List<Address> Addresses { get; set; } = new();
    public List<PhoneNumber> PhoneNumbers { get; set; } = new();
    public List<EmailAddress> EmailAddresses { get; set; } = new();
    public List<Insurance> Insurances { get; set; } = new();
}