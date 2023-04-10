using PatientApiService.Models;

namespace PatientApiService;

public interface IPatientApiClient
{
    Task<Patient?> GetPatientAsync(string clientId, Guid patientId);
    Task<Guid?> UpsertPatientAsync(string clientId, Patient patient);
}