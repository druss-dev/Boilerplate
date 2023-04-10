namespace PatientApiService.Models;

public class Insurance
{
    public long Id { get; set; }
    public int Sequence { get; set; }
    public string? PolicyHolderName { get; set; }
    public DateTime? PolicyHolderDateOfBirth { get; set; }
    public string? PolicyHolderRelationshipToPatient { get; set; }
    public string? InsuranceName { get; set; }
    public string? InsuranceMemberId { get; set; }
    public string? InsuranceGroupId { get; set; }
    public DateTime? InsuranceStartDate { get; set; }
    public DateTime? InsuranceEndDate { get; set; }
    public string? InsurancePhone { get; set; }
    public DateTime? ModifiedOn { get; set; }
}